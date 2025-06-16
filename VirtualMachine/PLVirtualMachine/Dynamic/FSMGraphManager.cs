using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.FSM;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using PLVirtualMachine.Time;

namespace PLVirtualMachine.Dynamic
{
  public class FSMGraphManager
  {
    protected DynamicFSM fsm;
    private IState debugCurrState;
    private ulong debugCurrStepStateGuid;
    private bool circularingAlarm;
    private StateStack mainStateStack = new StateStack();
    private StateStack localStateStack = new StateStack(EStateStackType.STATESTACK_TYPE_LOCAL);
    private StateStack currentStateStack;
    private StateStack lastSubGraphStateStack;
    private int currHistoryLevel;
    private Dictionary<string, EventMessage> contextMessages;
    private Dictionary<ulong, int> flipFlopBranchCurrentIndexes;
    private Dictionary<ulong, IState> eventProcessedStateSequence;
    private Dictionary<ulong, int> eventProcessedStateSequenceCount;
    private Stack<LoopInfo> loopStack;
    private Dictionary<string, object> loopLocalVariableValuesDict;
    private Dictionary<string, object> subgraphLocalVariableValuesDict;
    private DynamicFSM lockingOwnerFSM;
    private Guid lockingOwnerGuid = Guid.Empty;
    private RaisedEventInfo currentGraphExecEvent;
    private bool previosStateReturningMode;
    private bool isStateValid = true;
    protected static IGraphObject debugCurrentState;
    private static int CIRCULARING_CHECK_MAX_ITERATIONS_COUNT = 100;
    private static int ACTION_LOOP_CHECK_MAX_ITERATIONS_COUNT = 100000;

    public FSMGraphManager(DynamicFSM fsm)
    {
      this.fsm = fsm;
      previosStateReturningMode = false;
      SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
    }

    public virtual void StateSave(IDataWriter writer)
    {
      bool flag = IsSaveStateValid();
      if (!flag)
        Logger.AddError(string.Format("SaveLoad error: saving entity {0} is in invalid state {1}", fsm.Entity.Name, CurrentState.Name));
      SaveManagerUtility.Save(writer, "IsStateValid", flag);
      SaveManagerUtility.SaveDynamicSerializable(writer, "MainStateStack", mainStateStack);
      SaveManagerUtility.SaveDynamicSerializable(writer, "LocalStateStack", localStateStack);
      SaveManagerUtility.SaveDynamicSerializable(writer, "LastSubgraphStateStack", lastSubGraphStateStack);
      if (currentStateStack == mainStateStack)
        SaveManagerUtility.Save(writer, "CurrentStateStackName", "Main");
      else if (currentStateStack == localStateStack)
      {
        SaveManagerUtility.Save(writer, "CurrentStateStackName", "Local");
      }
      else
      {
        Logger.AddError(string.Format("Invalid current state stack at {0} fsm", fsm.FSMStaticObject.Name));
        SaveManagerUtility.Save(writer, "CurrentStateStackName", "Main");
      }
      if (flipFlopBranchCurrentIndexes != null)
        SaveManagerUtility.SaveDictionary(writer, "FlipFlopBranchCurrentIndexesData", flipFlopBranchCurrentIndexes);
      SaveManagerUtility.Save(writer, "LockingFSM", lockingOwnerFSM != null ? lockingOwnerFSM.EngineGuid : Guid.Empty);
      if (subgraphLocalVariableValuesDict == null)
        return;
      SaveManagerUtility.SaveDictionaryCommon(writer, "SubgraphLocalVariablesData", subgraphLocalVariableValuesDict);
    }

    public virtual void LoadFromXML(XmlElement xmlNode)
    {
      if (xmlNode.Name == "IsStateValid")
        isStateValid = VMSaveLoadManager.ReadBool(xmlNode);
      else if (xmlNode.Name == "MainStateStack")
      {
        if (mainStateStack == null)
          mainStateStack = new StateStack();
        mainStateStack.LoadFromXML(xmlNode);
      }
      else if (xmlNode.Name == "LocalStateStack")
      {
        if (localStateStack == null)
          localStateStack = new StateStack(EStateStackType.STATESTACK_TYPE_LOCAL);
        localStateStack.LoadFromXML(xmlNode);
      }
      else if (xmlNode.Name == "LastSubgraphStateStack")
      {
        if (lastSubGraphStateStack == null)
          lastSubGraphStateStack = new StateStack();
        lastSubGraphStateStack.LoadFromXML(xmlNode);
      }
      else if (xmlNode.Name == "CurrentStateStackName")
      {
        if (xmlNode.InnerText == "Local")
          currentStateStack = localStateStack;
        else
          currentStateStack = mainStateStack;
      }
      else if (xmlNode.Name == "LockingFSM")
        lockingOwnerGuid = VMSaveLoadManager.ReadGuid(xmlNode);
      else if (xmlNode.Name == "FlipFlopBranchCurrentIndexesData")
      {
        LoadFlipFlopBranchCurrentIndexesData(xmlNode);
      }
      else
      {
        if (!(xmlNode.Name == "SubgraphLocalVariablesData"))
          return;
        LoadSubgraphLocalVariablesData(xmlNode);
      }
    }

    public void OnSaveLoad()
    {
      if (CurrentState != null)
      {
        try
        {
          if (subgraphLocalVariableValuesDict != null)
          {
            List<string> list = subgraphLocalVariableValuesDict.Keys.ToList();
            for (int index = 0; index < list.Count; ++index)
            {
              string str = list[index];
              IVariable localContextVariable = CurrentState.GetLocalContextVariable(str);
              if (localContextVariable != null)
              {
                object obj = StringSerializer.ReadValue((string) subgraphLocalVariableValuesDict[str], localContextVariable.Type.BaseType);
                subgraphLocalVariableValuesDict[str] = obj;
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("SaveLoad error: cannot load local variables dict for fsm in entity {0}, error: {1}", fsm.Entity.Name, ex));
        }
      }
      if (isStateValid)
        return;
      string str1 = "not defined";
      if (CurrentState != null)
        str1 = CurrentState.Name;
      Logger.AddError(string.Format("SaveLoad error: entity {0} has saved in invalid state {1}", fsm.Entity.Name, str1));
    }

    public virtual void AfterSaveLoading()
    {
      if (fsm.Entity.SaveLoadInstantiated)
      {
        try
        {
          if (CurrentState != null)
          {
            OnStateIn(CurrentState);
            OnChangeState(CurrentState);
            List<StatePoint> list = CurrentStateStack.Stack.ToList();
            for (int index = list.Count - 1; index >= 0; --index)
            {
              IState currentState = list[index].currentState;
              if (currentState != null)
              {
                if (currentState.Parent != null && typeof (IFiniteStateMachine).IsAssignableFrom(currentState.Parent.GetType()))
                  SubscribeToNullSourceGraphEvents((FiniteStateMachine) currentState.Parent);
                if (typeof (VMState).IsAssignableFrom(currentState.GetType()))
                {
                  if (currentState.IsStable || typeof (IFiniteStateMachine).IsAssignableFrom(currentState.GetType()))
                    SubscribeToEvents((VMState) currentState);
                  else
                    SubscribeToOnLoadEvent((VMState) currentState);
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("SaveLoad error at {0} fsm afterloading, error: {1}", fsm.FSMStaticObject.Name, ex));
        }
      }
      if (!(Guid.Empty != lockingOwnerGuid))
        return;
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(lockingOwnerGuid);
      if (entityByEngineGuid == null)
        return;
      lockingOwnerFSM = entityByEngineGuid.GetFSM();
    }

    public void Clear()
    {
      mainStateStack = null;
      localStateStack = null;
      if (contextMessages != null)
        contextMessages.Clear();
      if (flipFlopBranchCurrentIndexes != null)
        flipFlopBranchCurrentIndexes.Clear();
      if (eventProcessedStateSequence != null)
        eventProcessedStateSequence.Clear();
      if (loopStack != null)
        loopStack.Clear();
      if (loopLocalVariableValuesDict != null)
        loopLocalVariableValuesDict.Clear();
      if (subgraphLocalVariableValuesDict == null)
        return;
      subgraphLocalVariableValuesDict.Clear();
    }

    public IState CurrentState => CurrentStateStack.Peek()?.currentState;

    public IState PreviousState
    {
      get
      {
        if (CurrentStateStack.Count == 0)
          return null;
        return CurrentStateStack.Peek().prevState == null ? null : CurrentStateStack.Peek().prevState.State;
      }
    }

    public StateStack PreviousStack
    {
      get
      {
        if (CurrentStateStack.Count == 0)
          return null;
        return CurrentStateStack.Peek().prevState == null ? null : CurrentStateStack.Peek().prevState.Stack;
      }
    }

    public virtual void OnProcessEvent(RaisedEventInfo evntInfo)
    {
      DynamicEvent instance = evntInfo.Instance;
      DoProcessEvent(evntInfo);
    }

    public void DoProcessEvent(RaisedEventInfo evntInfo)
    {
      DynamicEvent instance = evntInfo.Instance;
      currentGraphExecEvent = evntInfo;
      if (DebugUtility.IsDebug)
      {
        if (eventProcessedStateSequence == null)
          eventProcessedStateSequence = new Dictionary<ulong, IState>(UlongComparer.Instance);
        else
          eventProcessedStateSequence.Clear();
        if (eventProcessedStateSequenceCount == null)
          eventProcessedStateSequenceCount = new Dictionary<ulong, int>(UlongComparer.Instance);
        else
          eventProcessedStateSequenceCount.Clear();
      }
      circularingAlarm = false;
      previosStateReturningMode = false;
      if (DynamicTalkingFSM.IsTalking)
      {
        bool flag = instance.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_ON_GLOBAL_TIMER, typeof (VMGameComponent));
        if (!flag && instance.OwnerFSM != null && instance.OwnerFSM.Active && instance.OwnerFSM.Entity.EngineGuid == GameTimeManager.CurrentPlayCharacterEntity.EngineGuid && CurrentState != null && CurrentState.Parent != null && ((IFiniteStateMachine) CurrentState.Parent).GraphType != EGraphType.GRAPH_TYPE_TALKING && ((IFiniteStateMachine) CurrentState.Parent).GraphType != EGraphType.GRAPH_TYPE_TRADE)
          flag = true;
        if (!flag)
          return;
      }
      if (lockingOwnerFSM != null && lockingOwnerFSM.IsActualEvent(evntInfo))
        return;
      ulong num = 0;
      List<EventMessage> messages = evntInfo.Messages;
      if (contextMessages == null)
      {
        if (messages.Count > 0)
          contextMessages = new Dictionary<string, EventMessage>();
      }
      else
        contextMessages.Clear();
      for (int index = 0; index < messages.Count; ++index)
      {
        if (messages[index].Name == "stateId")
          num = (ulong) messages[index].Value;
        contextMessages.Add(messages[index].Name, messages[index]);
      }
      VMEventLink moveLink = null;
      bool ownEvent = IsEventOwn(instance);
      DynamicFSM fsm = this.fsm;
      if (instance.OwnerFSM != null)
      {
        DynamicFSM ownerFsm = instance.OwnerFSM;
      }
      if (num != 0UL && (CurrentState == null || (long) CurrentState.BaseGuid != (long) num))
        return;
      if (CurrentState != null && CurrentState.Parent != null && ((FiniteStateMachine) CurrentState.Parent).IsSubGraph)
      {
        VMEventLink outputLinkByEvent = GetOuterOutputLinkByEvent(instance);
        if (outputLinkByEvent != null)
        {
          ProcessLink(outputLinkByEvent);
          return;
        }
      }
      if (CurrentState != null)
        moveLink = GetStateOutputLinkByEvent((VMState) CurrentState, this.fsm, instance, ownEvent);
      if (moveLink == null)
      {
        IFiniteStateMachine parentGraph = this.fsm.FSMStaticObject.StateGraph;
        if (CurrentState != null)
          parentGraph = (IFiniteStateMachine) CurrentState.Parent;
        moveLink = GetGraphEnterLinkByEvent(parentGraph, this.fsm, instance, evntInfo.Messages, ownEvent);
      }
      if (moveLink != null)
        ProcessLink(moveLink);
      else if (CurrentState != null && CurrentState.Parent != null && ((FiniteStateMachine) CurrentState.Parent).IsSubGraph)
      {
        VMEventLink enterLinkByEvent = GetOuterEnterLinkByEvent(instance, evntInfo.Messages);
        if (enterLinkByEvent != null)
          ProcessLink(enterLinkByEvent);
      }
      if (!this.fsm.Entity.Instantiated || CurrentState == null)
        return;
      if (((IFiniteStateMachine) CurrentState.Parent).GraphType == EGraphType.GRAPH_TYPE_PROCEDURE)
        Logger.AddError(string.Format(" {0} FSM  must return from procedure '{1}' after event '{2}' processing end!", this.fsm.FSMStaticObject.Name, CurrentState.Parent.Name, instance.Name));
      if (CurrentStateStack.StackType != EStateStackType.STATESTACK_TYPE_LOCAL)
        return;
      Logger.AddError(string.Format(" {0} FSM  must return from procedure '{1}' after event '{2}' processing end!", this.fsm.FSMStaticObject.Name, CurrentState.Parent.Name, instance.Name));
    }

    public void SubscribeToEvents(VMState currState)
    {
      if (currState == null)
        return;
      if (typeof (ISpeech).IsAssignableFrom(currState.GetType()))
      {
        DynamicEvent contextEvent = VirtualMachine.Instance.GameRootFsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking)));
        if (contextEvent == null)
          Logger.AddError(string.Format("Speech reply event not created in this FSM at {0}", DynamicFSM.CurrentStateInfo));
        else
          contextEvent.Subscribe(fsm);
      }
      else
      {
        for (int index = 0; index < currState.OutputLinks.Count; ++index)
        {
          VMEventLink outputLink = (VMEventLink) currState.OutputLinks[index];
          if (outputLink.Event != null && outputLink.Event.EventInstance != null)
          {
            IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(fsm, outputLink.Event.EventOwner, (IBlueprint) outputLink.Owner);
            if (ownerDynamicContext != null)
            {
              if (!ownerDynamicContext.Entity.IsDisposed)
              {
                DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(outputLink.Event.EventInstance.BaseGuid);
                if (eventByStaticGuid == null)
                {
                  Logger.AddError(string.Format("Event subscribing error: event with name {0} and static guid={1} not found in object {2} at {3} fsm", outputLink.Event.EventInstance.Name, outputLink.Event.EventInstance.BaseGuid, outputLink.Event.EventOwner.Name, ownerDynamicContext.FSMStaticObject.Name));
                  ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(outputLink.Event.EventInstance.BaseGuid);
                }
                else
                  eventByStaticGuid.Subscribe(fsm);
              }
            }
            else
              Logger.AddError(string.Format("Event subscribing error: event owner by variable {0} not found in {1} FSM at {2}", outputLink.Event.EventOwner.Name, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
          }
        }
      }
    }

    public EGraphType CurrentFSMGraphType
    {
      get
      {
        return CurrentState != null && CurrentState.Parent != null && typeof (IFiniteStateMachine).IsAssignableFrom(CurrentState.Parent.GetType()) ? ((IFiniteStateMachine) CurrentState.Parent).GraphType : EGraphType.GRAPH_TYPE_EVENTGRAPH;
      }
    }

    public EventMessage GetContextMessage(string paramName)
    {
      EventMessage eventMessage;
      return contextMessages != null && contextMessages.TryGetValue(paramName, out eventMessage) ? eventMessage : null;
    }

    public object GetLocalVariableValue(string varName)
    {
      object localVariableValue1;
      if (loopLocalVariableValuesDict != null && loopLocalVariableValuesDict.TryGetValue(varName, out localVariableValue1))
        return localVariableValue1;
      object localVariableValue2;
      if (subgraphLocalVariableValuesDict != null && subgraphLocalVariableValuesDict.TryGetValue(varName, out localVariableValue2))
        return localVariableValue2;
      Logger.AddError(string.Format("Local variable with name {0} not found in {1} FSM at {2}", varName, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
      return null;
    }

    public IState DebugCurrState => debugCurrState;

    protected void SetCurrentStateStack(EStateStackType stateStackType, bool bInit = false)
    {
      if (stateStackType == EStateStackType.STATESTACK_TYPE_MAIN)
      {
        currentStateStack = mainStateStack;
        if (CurrentState != null)
        {
          OnStateIn(CurrentState, false);
          OnChangeState(CurrentState);
        }
      }
      else
        currentStateStack = localStateStack;
      if (!bInit)
        return;
      currentStateStack.Init();
    }

    private StateStack CurrentStateStack
    {
      get
      {
        if (currentStateStack != null)
          return currentStateStack;
        Logger.AddError(string.Format("Current state stack not inited!!! at {0}", DynamicFSM.CurrentStateInfo));
        return mainStateStack;
      }
    }

    private bool IsSaveStateValid()
    {
      return CurrentState == null || ((IFiniteStateMachine) CurrentState.Parent).GraphType != EGraphType.GRAPH_TYPE_PROCEDURE && CurrentStateStack.StackType != EStateStackType.STATESTACK_TYPE_LOCAL && ((VMState) CurrentState).IsStable;
    }

    protected bool ProcessLink(VMEventLink moveLink)
    {
      if (circularingAlarm || !fsm.Entity.Instantiated || moveLink == null || !moveLink.Enabled)
        return false;
      if (moveLink.Event != null && moveLink.Event.EventInstance != null && IsProcedureLink(moveLink))
      {
        if (moveLink.DestState == null)
        {
          Logger.AddError(string.Format("After procedural event {0} in graph {1} must be procedure", moveLink.Event.EventInstance.Name, moveLink.Parent.Name));
          return false;
        }
        if (moveLink.DestState.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GRAPH)
        {
          Logger.AddError(string.Format("After procedural event {0} in graph {1} must be procedure", moveLink.Event.EventInstance.Name, moveLink.Parent.Name));
          return false;
        }
        if (((FiniteStateMachine) moveLink.DestState).GraphType != EGraphType.GRAPH_TYPE_PROCEDURE)
        {
          Logger.AddError(string.Format("After procedural event {0} in graph {1} must be procedure", moveLink.Event.EventInstance.Name, moveLink.Parent.Name));
          return false;
        }
        FiniteStateMachine destState = (FiniteStateMachine) moveLink.DestState;
        if (destState.InputParams.Count > 0)
          MakeSubgraphInputParams(destState, moveLink);
        SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_LOCAL, true);
        ProcessMoveToState(((FiniteStateMachine) moveLink.DestState).InitState, moveLink);
        return true;
      }
      if (CurrentState != null && !((VMBaseObject) moveLink.Parent).IsEqual(CurrentState.Parent))
      {
        if (typeof (VMBlueprint) == ((VMBaseObject) moveLink.Parent).Parent.GetType())
        {
          if (fsm.FSMStaticObject.IsDerivedFrom(((VMBaseObject) ((VMBaseObject) moveLink.Parent).Parent).BaseGuid, true))
          {
            lastSubGraphStateStack = new StateStack(CurrentStateStack);
            IState destState = moveLink.DestState;
            int destEntryPoint = moveLink.DestEntryPoint;
            if (lockingOwnerFSM == null || destState.IgnoreBlock)
              ProcessMoveToState(destState, moveLink, destEntryPoint);
          }
          return true;
        }
        if (!((VMBaseObject) CurrentState).IsEqual(moveLink.Parent))
        {
          if (lockingOwnerFSM != null && !CurrentState.IgnoreBlock)
            return false;
          ReturnToOuterGraph((IFiniteStateMachine) moveLink.Parent, moveLink);
          return true;
        }
      }
      IState destState1 = moveLink.DestState;
      int destEntryPoint1 = moveLink.DestEntryPoint;
      bool flag = false;
      if (CurrentState == null && destState1 != null)
        flag = destState1.Initial;
      if (destState1 != null)
      {
        if (((lockingOwnerFSM == null ? 1 : (destState1.IgnoreBlock ? 1 : 0)) | (flag ? 1 : 0)) == 0 && !IsThisTalking)
          return false;
        ProcessMoveToState(destState1, moveLink, destEntryPoint1);
      }
      else
      {
        if (lockingOwnerFSM != null && !CurrentState.IgnoreBlock)
          return false;
        if (moveLink.ExitFromSubGraph || IsThisTalking)
          ReturnToOuterGraph(moveLink);
        else
          ReturnToPreviousState();
      }
      return true;
    }

    public virtual void ProcessMoveToState(
      IState newState,
      IEventLink inputLink,
      int destEntryPoint = 0)
    {
      if (newState == null)
        Logger.AddError(string.Format("State for moving to not defined in {0} !!!", fsm.FSMStaticObject.Name));
      else if (((VMBaseObject) newState).GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
        MoveIntoSubGraph((FiniteStateMachine) newState, inputLink, destEntryPoint);
      else
        MoveIntoState(newState, destEntryPoint);
    }

    public bool Lock(DynamicFSM lockingFSM)
    {
      if (lockingOwnerFSM == null)
      {
        lockingOwnerFSM = lockingFSM;
        return true;
      }
      Logger.AddWarning(string.Format("This fsm already locked by {0} FSM of static object id={0}", lockingOwnerFSM.StaticObject.BaseGuid));
      return false;
    }

    public bool UnLock(DynamicFSM lockingFSM)
    {
      if (lockingOwnerFSM != null && (long) lockingOwnerFSM.StaticGuid == (long) lockingFSM.StaticGuid)
      {
        lockingOwnerFSM = null;
        return true;
      }
      Logger.AddWarning(string.Format("This fsm is not locked by FSM of static object id={0}", lockingFSM.StaticObject.BaseGuid));
      return false;
    }

    public DynamicFSM LockingFSM => lockingOwnerFSM;

    public static void SetCurrentDebugState(IGraphObject currentState)
    {
      debugCurrentState = currentState;
    }

    public static IGraphObject DebugCurrentState => debugCurrentState;

    public static void ClearAll() => debugCurrentState = null;

    public void ProcessState(IState state, int destEntryPoint)
    {
      try
      {
        if (!fsm.Active || OnStateIn(state))
          return;
        ExecuteState(state, destEntryPoint);
        OnStateExec(state);
        OnChangeState(state);
        if (typeof (ISpeech).IsAssignableFrom(state.GetType()))
          return;
        VMEventLink afterExitLink = ((VMState) state).GetAfterExitLink();
        if (afterExitLink == null)
          return;
        ProcessLink(afterExitLink);
      }
      catch (Exception ex)
      {
        Logger.AddError(ex + "at " + DynamicFSM.CurrentStateInfo);
        throw;
      }
    }

    public bool IsActualEvent(RaisedEventInfo evntInfo)
    {
      bool ownEvent = IsEventOwn(evntInfo.Instance);
      VMEventLink vmEventLink = null;
      if (CurrentState != null && CurrentState.Parent != null && ((FiniteStateMachine) CurrentState.Parent).IsSubGraph)
        vmEventLink = GetOuterOutputLinkByEvent(evntInfo.Instance);
      if (vmEventLink != null)
        return true;
      if (CurrentState != null)
        vmEventLink = GetStateOutputLinkByEvent((VMState) CurrentState, fsm, evntInfo.Instance, ownEvent);
      if (vmEventLink != null)
        return true;
      IFiniteStateMachine parentGraph = fsm.FSMStaticObject.StateGraph;
      if (CurrentState != null)
        parentGraph = (IFiniteStateMachine) CurrentState.Parent;
      VMEventLink enterLinkByEvent = GetGraphEnterLinkByEvent(parentGraph, fsm, evntInfo.Instance, evntInfo.Messages, ownEvent);
      if (enterLinkByEvent != null)
        return true;
      if (CurrentState != null && CurrentState.Parent != null && ((FiniteStateMachine) CurrentState.Parent).IsSubGraph)
        enterLinkByEvent = GetOuterEnterLinkByEvent(evntInfo.Instance, evntInfo.Messages);
      return enterLinkByEvent != null;
    }

    private void LoadFlipFlopBranchCurrentIndexesData(XmlElement rootNode)
    {
      if (flipFlopBranchCurrentIndexes != null)
        flipFlopBranchCurrentIndexes.Clear();
      else if (rootNode.ChildNodes.Count > 0)
        flipFlopBranchCurrentIndexes = new Dictionary<ulong, int>(UlongComparer.Instance);
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        flipFlopBranchCurrentIndexes.Add(VMSaveLoadManager.ReadUlong(firstChild), VMSaveLoadManager.ReadInt(firstChild.NextSibling));
      }
    }

    protected virtual void ReturnToOuterGraph(VMEventLink prevLink = null)
    {
      VMEventLink outerGraphLink = null;
      IFiniteStateMachine outherGraph = null;
      if (prevLink != null && prevLink.LinkExitType == ELinkExitType.LINK_EXIT_TYPE_OUTER_EVENT_EXECUTION && currentGraphExecEvent != null)
      {
        outerGraphLink = GetOuterEnterLinkByEvent(currentGraphExecEvent.Instance, currentGraphExecEvent.Messages, (IFiniteStateMachine) prevLink.Parent);
        if (outerGraphLink != null)
          outherGraph = (IFiniteStateMachine) outerGraphLink.Parent;
      }
      if (DebugUtility.IsDebug)
      {
        IState state = CurrentState ?? fsm.FSMStaticObject.StateGraph;
        if (!eventProcessedStateSequence.ContainsKey(state.BaseGuid))
        {
          eventProcessedStateSequence.Add(state.BaseGuid, state);
          eventProcessedStateSequenceCount.Add(state.BaseGuid, 1);
        }
        else
        {
          int num = eventProcessedStateSequenceCount[state.BaseGuid] + 1;
          eventProcessedStateSequenceCount[state.BaseGuid] = num;
          if (num > CIRCULARING_CHECK_MAX_ITERATIONS_COUNT)
          {
            circularingAlarm = true;
            Logger.AddError(string.Format("State {0} process is cycling!!! at {1}", state.Name, DynamicFSM.CurrentStateInfo));
            return;
          }
        }
      }
      if (outerGraphLink == null)
        outerGraphLink = GetOuterAfterExitLink();
      if (outerGraphLink != null)
      {
        ReturnToOuterGraph(outherGraph, outerGraphLink);
      }
      else
      {
        if (CurrentStateStack.Count > 1)
          PopState();
        ReturnToPreviousState();
      }
    }

    private void ReturnToOuterGraph(IFiniteStateMachine outherGraph, VMEventLink outerGraphLink = null)
    {
      if (CurrentStateStack.StackType == EStateStackType.STATESTACK_TYPE_LOCAL & IsCurrentProcedure && CurrentStateStack.Count == 1)
      {
        SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
      }
      else
      {
        lastSubGraphStateStack = outherGraph == null || outerGraphLink == null || CurrentStateStack.StackType != EStateStackType.STATESTACK_TYPE_MAIN ? null : new StateStack(CurrentStateStack);
        IState previousState = PreviousState;
        IState currentState = CurrentState;
        string str1 = "none";
        string str2 = "none";
        if (currentState != null)
        {
          str1 = currentState.Name;
          if (currentState.Parent != null)
            str2 = currentState.Parent.Name;
        }
        if (outherGraph != null)
        {
          while (CurrentState != null)
          {
            if (CurrentState.Parent == null)
              Logger.AddError(string.Format("Current state {0} id={1} has null parent", CurrentState.Name, CurrentState.BaseGuid));
            if (!CurrentState.Parent.IsEqual(outherGraph))
            {
              PopState();
              if (CurrentState == null)
              {
                Logger.AddError(string.Format("Invalid outher graph returning: return graph id={0} not found in state stack", outherGraph.BaseGuid));
                goto label_15;
              }
            }
            else
              goto label_15;
          }
          Logger.AddError(string.Format("Invalid outher graph returning: return graph id={0} not found in state stack", outherGraph.BaseGuid));
        }
        else if (CurrentStateStack.Count > 1 || outerGraphLink == null)
          PopState();
label_15:
        string str3 = "none";
        if (outherGraph != null)
          str3 = outherGraph.Name;
        if (CurrentState == null)
          Logger.AddError(string.Format("Invalid outher graph returning: CurrentState is not defined in graph {0} after returning from state {1}, graph {2}", str3, str1, str2));
        else if (CurrentState.Parent == null)
        {
          Logger.AddError(string.Format("Invalid outher graph returning: CurrentState.Parent is not defined in graph {0}", str3));
        }
        else
        {
          previosStateReturningMode = true;
          if (outerGraphLink != null && outerGraphLink.DestState != null)
            previosStateReturningMode = false;
          bool flag = false;
          if (outerGraphLink != null)
            flag = ProcessLink(outerGraphLink);
          if (flag)
            return;
          ReturnToPreviousState();
        }
      }
    }

    protected void ReturnToPreviousState(bool prevIsCurrent = false)
    {
      if (circularingAlarm)
        return;
      previosStateReturningMode = true;
      IState state = PreviousState;
      if (prevIsCurrent)
      {
        if (lastSubGraphStateStack != null)
        {
          ReturnToMemoryStack(lastSubGraphStateStack);
          if (lockingOwnerFSM != null)
            lockingOwnerFSM.SetLockedObjectNeedRestoreAction(fsm);
          lastSubGraphStateStack = null;
          return;
        }
        state = CurrentState;
      }
      if (state != null && DebugUtility.IsDebug && eventProcessedStateSequence.ContainsKey(state.BaseGuid) && PreviousState != null && (long) PreviousState.BaseGuid != (long) state.BaseGuid)
        state = PreviousState;
      if (state != null)
      {
        if (((VMBaseObject) state).GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
          MoveIntoSubGraph((FiniteStateMachine) state, null);
        else
          MoveIntoState(state, 0);
      }
      else
      {
        StateStack previousStack = PreviousStack;
        if (previousStack != null)
          ReturnToMemoryStack(previousStack);
        else if (CurrentState != null && CurrentState.Parent != null)
          ProcessMoveToState(((FiniteStateMachine) CurrentState.Parent).InitState, null);
      }
      if (lockingOwnerFSM == null)
        return;
      lockingOwnerFSM.SetLockedObjectNeedRestoreAction(fsm);
    }

    protected void MoveIntoSubGraph(
      FiniteStateMachine subGraph,
      IEventLink inputLink,
      int destEntryPoint = 0)
    {
      currentGraphExecEvent = null;
      if (OnStateIn(subGraph))
        return;
      IState prevState = CurrentState;
      if (CurrentState.IsProcedure)
        prevState = PreviousState;
      if ((!CurrentState.IsProcedure ? 0 : (subGraph.IsProcedure ? 1 : 0)) == 0)
      {
        bool flag = false;
        if (subGraph.Parent == null || CurrentState == null)
          Logger.AddError(string.Format("Invalid subgraph {0} enter in {1} fsm", subGraph.Name, fsm.FSMStaticObject.Name));
        else if ((long) subGraph.Parent.BaseGuid == (long) CurrentState.BaseGuid)
          flag = true;
        if (!flag)
          PopState();
        PushState(prevState, subGraph);
      }
      else
        SwitchState(subGraph);
      IState initState = subGraph.InitState;
      if (!IsCurrentProcedure)
        SubscribeToNullSourceGraphEvents(subGraph);
      if (subGraph.InputParams.Count > 0 && inputLink != null)
        MakeSubgraphInputParams(subGraph, (IParamListSource) inputLink);
      MoveIntoState(initState, destEntryPoint, true);
    }

    protected virtual void OnChangeState(IState state)
    {
    }

    protected virtual void MoveIntoState(IState newState, int destEntryPoint, bool bNextLevel = false)
    {
      if (newState == null)
      {
        Logger.AddError(string.Format("New state not defined in object {0}!", fsm.FSMStaticObject.Name));
      }
      else
      {
        debugCurrentState = newState;
        if (destEntryPoint < 0 || destEntryPoint >= newState.EntryPoints.Count)
          Logger.AddError(string.Format("Invalid entry point index at move to {0} state in {1} object", newState.Name, fsm.StaticGuid));
        else if (typeof (IBranch).IsAssignableFrom(newState.GetType()))
          ProcessBranch((IBranch) newState);
        else if (CurrentState != null && (long) newState.BaseGuid == (long) CurrentState.BaseGuid)
        {
          ProcessState(newState, destEntryPoint);
        }
        else
        {
          IState currentState = CurrentState;
          bool flag = false;
          if (!bNextLevel && !flag)
            PopState();
          if (!flag)
            PushState(currentState, newState);
          ProcessState(newState, destEntryPoint);
        }
      }
    }

    protected void PushState(IState prevState, IState newState)
    {
      if (newState == null)
      {
        Logger.AddError(string.Format("New state not defined in object {0}!", fsm.FSMStaticObject.Name));
      }
      else
      {
        StatePoint newStatePoint;
        if (prevState != null)
        {
          if (typeof (IFiniteStateMachine).IsAssignableFrom(prevState.GetType()))
          {
            if (lastSubGraphStateStack != null)
            {
              newStatePoint = new StatePoint(newState, lastSubGraphStateStack);
              lastSubGraphStateStack = null;
              ++currHistoryLevel;
            }
            else
              newStatePoint = new StatePoint(newState);
          }
          else
            newStatePoint = new StatePoint(newState, prevState);
        }
        else
          newStatePoint = new StatePoint(newState);
        CurrentStateStack.Push(newStatePoint);
        if (!IsCurrentProcedure)
          SubscribeToEvents((VMState) CurrentState);
        if (!((VMState) CurrentState).Initial || ((FiniteStateMachine) CurrentState.Parent).IsSubGraph)
          return;
        FiniteStateMachine parent = (FiniteStateMachine) CurrentState.Parent;
        debugCurrentState = parent;
        SubscribeToNullSourceGraphEvents(parent);
        if (parent.Owner == null)
          return;
        if (!typeof (IBlueprint).IsAssignableFrom(parent.Owner.GetType()))
          return;
        try
        {
          List<IBlueprint> baseBlueprints = ((IBlueprint) parent.Owner).BaseBlueprints;
          if (baseBlueprints == null)
            return;
          for (int index = 0; index < baseBlueprints.Count; ++index)
          {
            if (baseBlueprints[index].StateGraph != null)
              SubscribeToNullSourceGraphEvents((FiniteStateMachine) baseBlueprints[index].StateGraph);
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    protected void SwitchState(IState newState)
    {
      CurrentStateStack.Stack.Peek().currentState = newState;
      if (newState.IsProcedure)
        return;
      SubscribeToEvents((VMState) newState);
    }

    protected IState PopState()
    {
      if (CurrentStateStack.Count == 0)
        return null;
      IState currentState = CurrentStateStack.Pop().currentState;
      if (!IsCurrentProcedure)
      {
        if (typeof (FiniteStateMachine).IsAssignableFrom(currentState.GetType()))
          ResetNullSourceGraphEvents((FiniteStateMachine) currentState);
        ResetEvents((VMState) currentState);
      }
      return currentState;
    }

    protected bool OnStateIn(IState state, bool bCanBlock = true)
    {
      fsm.OnModify();
      if (DebugUtility.IsDebug)
      {
        string str1 = "";
        if (state.Parent != null)
          str1 = str1 + state.Parent.Name + ".";
        string str2 = str1 + state.Name;
        debugCurrentState = state;
        if (fsm.Entity != null)
          fsm.Entity.DebugContextInfo = str2;
        if ((long) debugCurrStepStateGuid != (long) state.BaseGuid && DebugUtility.Debugger.OnStateInput(fsm, state) && bCanBlock)
        {
          debugCurrStepStateGuid = state.BaseGuid;
          return true;
        }
        debugCurrState = state;
      }
      return false;
    }

    private void OnStateExec(IState state) => DebugUtility.OnStateExec(fsm, state);

    private void ExecuteState(IState state, int destEntryPoint)
    {
      if (DebugUtility.IsDebug)
      {
        if (!eventProcessedStateSequence.ContainsKey(((VMBaseObject) state).BaseGuid))
        {
          eventProcessedStateSequence.Add(((VMBaseObject) state).BaseGuid, state);
          eventProcessedStateSequenceCount.Add(((VMBaseObject) state).BaseGuid, 1);
        }
        else
        {
          int num = eventProcessedStateSequenceCount[((VMBaseObject) state).BaseGuid] + 1;
          eventProcessedStateSequenceCount[((VMBaseObject) state).BaseGuid] = num;
          if (num > CIRCULARING_CHECK_MAX_ITERATIONS_COUNT)
          {
            circularingAlarm = true;
            Logger.AddError(string.Format("State {0} process is cycling!!! at {1}", state.Name, DynamicFSM.CurrentStateInfo));
            return;
          }
        }
      }
      if (IsReturnToPrevios())
        return;
      ProcessActionLine(state.EntryPoints[destEntryPoint].ActionLine);
      CheckLoopStackClear();
    }

    private bool IsReturnToPrevios()
    {
      int num = previosStateReturningMode ? 1 : 0;
      previosStateReturningMode = false;
      return num != 0;
    }

    protected void ProcessActionLine(IActionLine actionLine, bool bFromSpeech = false)
    {
      if (actionLine == null)
        return;
      if (!actionLine.IsUpdated)
        actionLine.Update();
      if (actionLine.ActionLineType == EActionLineType.ACTION_LINE_TYPE_LOOP)
      {
        if (!typeof (IActionLoop).IsAssignableFrom(actionLine.GetType()))
          return;
        IActionLoop actionLoop = (IActionLoop) actionLine;
        int loopStartIndexValue = GetLoopStartIndexValue(actionLoop);
        int num = GetLoopEndIndexValue(actionLoop);
        ICommonList loopListValue = GetLoopListValue(actionLoop);
        if (loopListValue != null)
        {
          if (num > loopListValue.ObjectsCount)
            num = loopListValue.ObjectsCount;
        }
        else
          loopListValue = GetLoopListValue(actionLoop);
        PushLoopToStack(actionLine, loopListValue);
        if (actionLoop.LoopRandomIndexing)
        {
          int randomMinIndex = loopStartIndexValue;
          int randomMaxIndex = num;
          int indexesCount = num - loopStartIndexValue;
          if (loopListValue != null && randomMinIndex == 0)
            randomMaxIndex = loopListValue.ObjectsCount;
          List<int> randomIndexes = VMMath.GetRandomIndexes(randomMinIndex, randomMaxIndex, indexesCount);
          for (int index = 0; index < randomIndexes.Count; ++index)
          {
            SetCurrentLoopIndexValue(randomIndexes[index]);
            DoProcessActionLine(actionLine);
          }
        }
        else
        {
          if (num - loopStartIndexValue >= ACTION_LOOP_CHECK_MAX_ITERATIONS_COUNT)
          {
            num = loopStartIndexValue + ACTION_LOOP_CHECK_MAX_ITERATIONS_COUNT;
            Logger.AddError(string.Format("Invalid loop indexing (infinity loop) in loop action line id={0} at {1}", actionLine.BaseGuid, DynamicFSM.CurrentStateInfo));
          }
          for (int currentLoopIndex = loopStartIndexValue; currentLoopIndex < num; ++currentLoopIndex)
          {
            SetCurrentLoopIndexValue(currentLoopIndex);
            DoProcessActionLine(actionLine);
          }
        }
        PopLoopFromStack();
      }
      else
        DoProcessActionLine(actionLine, bFromSpeech);
    }

    private void DoProcessActionLine(IActionLine actionLine, bool bFromSpeech = false)
    {
      if (actionLine == null)
        return;
      foreach (IGameAction action in actionLine.Actions)
      {
        if (action != null)
        {
          if (typeof (IActionLine).IsAssignableFrom(action.GetType()))
          {
            ProcessActionLine((IActionLine) action);
          }
          else
          {
            ExpressionUtility.ProcessAction((IAbstractAction) action, fsm);
            if (!bFromSpeech && fsm.Active)
              fsm.OnProcessActionLine();
          }
          if (!fsm.Active)
            break;
        }
      }
    }

    private void ReturnToMemoryStack(StateStack prevMemoryStack)
    {
      do
      {
        PopState();
      }
      while (CurrentState != null);
      CurrentStateStack.Clear();
      Stack<StatePoint> source = new Stack<StatePoint>(prevMemoryStack.Stack);
      do
      {
        CurrentStateStack.Push(source.Pop());
        SubscribeToEvents((VMState) CurrentState);
        if (CurrentState.Parent != null)
          SubscribeToNullSourceGraphEvents((FiniteStateMachine) CurrentState.Parent);
      }
      while (source.Count() > 0);
      if (currHistoryLevel > 0)
        --currHistoryLevel;
      previosStateReturningMode = true;
      ProcessState(CurrentState, 0);
    }

    protected void ProcessBranch(IBranch branch)
    {
      if (OnStateIn(branch))
        return;
      if (branch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH || branch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH)
      {
        ProcessMinMaxBranch(branch);
      }
      else
      {
        int num1 = -1;
        if (branch.StateType == EStateType.STATE_TYPE_FLIPFLOP_BRANCH)
        {
          num1 = GetCurrentFlipFlopVariantIndex(branch);
          if (num1 >= branch.GetExitPointsCount())
          {
            ResetFlipFlopVariantIndex(branch);
            num1 = GetCurrentFlipFlopVariantIndex(branch);
          }
        }
        for (int index = 0; index < branch.GetExitPointsCount(); ++index)
        {
          bool flag;
          if (num1 >= 0)
          {
            flag = index == num1;
          }
          else
          {
            ICondition branchCondition = branch.GetBranchCondition(index);
            flag = index >= branch.GetExitPointsCount() - 1 || ExpressionUtility.CalculateConditionResult(branchCondition, fsm);
          }
          if (flag)
          {
            VMEventLink sourceExitPointIndex = ((VMState) branch).GetOutputLinkBySourceExitPointIndex(index);
            if (sourceExitPointIndex != null)
            {
              int num2 = IgnoreBranchCase(sourceExitPointIndex.DestState) ? 1 : 0;
              if (num2 == 0 || index >= branch.GetExitPointsCount() - 1)
              {
                if (sourceExitPointIndex.DestState != null)
                  ProcessLink(sourceExitPointIndex);
                else if (sourceExitPointIndex.ExitFromSubGraph || IsThisTalking)
                  ReturnToOuterGraph(sourceExitPointIndex);
                else
                  ReturnToPreviousState(true);
              }
              if (num2 == 0)
                break;
            }
            else
            {
              ReturnToPreviousState(true);
              break;
            }
          }
        }
      }
    }

    protected void ProcessMinMaxBranch(IBranch branch)
    {
      object secondValue = 0;
      int iSrcPntIndex = -1;
      if (branch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
        secondValue = float.MaxValue;
      else if (branch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH)
        secondValue = float.MinValue;
      for (int exitPntIndex = 0; exitPntIndex < branch.GetExitPointsCount(); ++exitPntIndex)
      {
        VMPartCondition branchCondition = (VMPartCondition) branch.GetBranchCondition(exitPntIndex);
        if (branchCondition != null)
        {
          VMExpression firstExpression = (VMExpression) branchCondition.FirstExpression;
          if (firstExpression != null)
          {
            object expressionResult = ExpressionUtility.CalculateExpressionResult(firstExpression, fsm);
            if (branch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
            {
              if (VMTypeMathUtility.IsValueLess(expressionResult, secondValue))
              {
                iSrcPntIndex = exitPntIndex;
                secondValue = expressionResult;
              }
            }
            else if (branch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH && VMTypeMathUtility.IsValueLarger(expressionResult, secondValue))
            {
              iSrcPntIndex = exitPntIndex;
              secondValue = expressionResult;
            }
          }
          else
            Logger.AddError(string.Format("MinMax branch {0} expression not defined at {0}", DynamicFSM.CurrentStateInfo));
        }
      }
      if (iSrcPntIndex < 0)
        return;
      VMEventLink sourceExitPointIndex = ((VMState) branch).GetOutputLinkBySourceExitPointIndex(iSrcPntIndex);
      if (sourceExitPointIndex != null)
      {
        IState destState = sourceExitPointIndex.DestState;
        if (sourceExitPointIndex.DestState != null)
          ProcessLink(sourceExitPointIndex);
        else if (sourceExitPointIndex.ExitFromSubGraph || IsThisTalking)
          ReturnToOuterGraph(sourceExitPointIndex);
        else
          ReturnToPreviousState(true);
      }
      else
        ReturnToPreviousState(true);
    }

    private void SubscribeToOnLoadEvent(VMState state)
    {
      DynamicEvent contextEvent = VirtualMachine.Instance.GameRootFsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_LOAD_GAME, typeof (VMGameComponent)));
      if (contextEvent == null)
        Logger.AddError(string.Format("Game onload event not found at {0}", DynamicFSM.CurrentStateInfo));
      else
        contextEvent.Subscribe(fsm);
    }

    private void SubscribeToNullSourceGraphEvents(FiniteStateMachine eventGraph)
    {
      for (int index = 0; index < eventGraph.EnterLinks.Count; ++index)
      {
        VMEventLink enterLink = (VMEventLink) eventGraph.EnterLinks[index];
        if (!enterLink.IsInitial() && enterLink.Event != null && enterLink.Event.EventInstance != null)
        {
          IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(fsm, enterLink.Event.EventOwner, (IBlueprint) enterLink.Owner);
          if (ownerDynamicContext != null)
          {
            if (!ownerDynamicContext.Entity.IsDisposed)
            {
              DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(enterLink.Event.EventInstance.BaseGuid);
              if (eventByStaticGuid == null)
              {
                Logger.AddError(string.Format("Event subscribing error: event with name {0} and static guid={1} not found in object {2} at {3} fsm", enterLink.Event.EventInstance.Name, enterLink.Event.EventInstance.BaseGuid, enterLink.Event.EventOwner.Name, ownerDynamicContext.FSMStaticObject.Name));
                ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(enterLink.Event.EventInstance.BaseGuid);
              }
              else
                eventByStaticGuid.Subscribe(fsm);
            }
          }
          else
            Logger.AddError(string.Format("Event subscribing error: event owner by variable {0} not found in {1} FSM at {2}", enterLink.Event.EventOwner.Name, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
        }
      }
      if (!typeof (VMBlueprint).IsAssignableFrom(eventGraph.Parent.GetType()))
        return;
      List<IBlueprint> baseBlueprints = ((VMBlueprint) eventGraph.Parent).BaseBlueprints;
      if (baseBlueprints == null)
        return;
      for (int index = 0; index < baseBlueprints.Count; ++index)
        SubscribeToNullSourceGraphEvents((FiniteStateMachine) baseBlueprints[index].StateGraph);
    }

    private void ResetNullSourceGraphEvents(FiniteStateMachine eventGraph)
    {
      for (int index = 0; index < eventGraph.EnterLinks.Count; ++index)
      {
        VMEventLink enterLink = (VMEventLink) eventGraph.EnterLinks[index];
        if (enterLink.Event != null && enterLink.Event.EventInstance != null)
        {
          IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(fsm, enterLink.Event.EventOwner, (IBlueprint) enterLink.Owner);
          if (ownerDynamicContext != null)
          {
            if (!ownerDynamicContext.Entity.IsDisposed)
            {
              DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(enterLink.Event.EventInstance.BaseGuid);
              if (eventByStaticGuid != null)
                eventByStaticGuid.DeSubscribe(fsm);
              else
                Logger.AddError(string.Format("Event desubscribing error: event with static guid={0} not found in {1} FSM at {2}", enterLink.Event.EventInstance.BaseGuid, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
            }
          }
          else
            Logger.AddWarning(string.Format("Event desubscribing error: event owner by variable {0} not found in {1} FSM at {2}", enterLink.Event.EventOwner.Name, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
        }
      }
    }

    private void ResetEvents(VMState remState)
    {
      if (typeof (ISpeech).IsAssignableFrom(remState.GetType()))
      {
        DynamicEvent contextEvent = VirtualMachine.Instance.GameRootFsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking)));
        if (contextEvent == null)
          Logger.AddError("Speech reply event not created in this FSM");
        else
          contextEvent.DeSubscribe(fsm);
      }
      for (int index = 0; index < remState.OutputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) remState.OutputLinks[index];
        if (outputLink.Event != null && outputLink.Event.EventInstance != null)
        {
          IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(fsm, outputLink.Event.EventOwner, (IBlueprint) outputLink.Owner);
          if (ownerDynamicContext != null)
          {
            if (!ownerDynamicContext.Entity.IsDisposed)
            {
              DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(outputLink.Event.EventInstance.BaseGuid);
              if (eventByStaticGuid != null)
                eventByStaticGuid.DeSubscribe(fsm);
              else
                Logger.AddError(string.Format("Event desubscribing error: event with static guid={0} not found in {1} FSM at {2}", outputLink.Event.EventInstance.BaseGuid, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
            }
          }
          else
            Logger.AddWarning(string.Format("Event desubscribing error: event owner by variable {0} not found in {1} FSM at {2}", outputLink.Event.EventOwner.Name, fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
        }
      }
    }

    private bool IsProcedureLink(IEventLink moveLink)
    {
      if (moveLink.SourceState == null && moveLink.DestState != null && typeof (IFiniteStateMachine).IsAssignableFrom(moveLink.DestState.GetType()) && moveLink.DestState.IsProcedure)
      {
        IEventLink outputLink = (IEventLink) moveLink.DestState.OutputLinks[0];
        if (outputLink != null && outputLink.DestState == null)
          return true;
      }
      return false;
    }

    private void LoadSubgraphLocalVariablesData(XmlElement rootNode)
    {
      if (rootNode.ChildNodes.Count > 0 && subgraphLocalVariableValuesDict == null)
        subgraphLocalVariableValuesDict = new Dictionary<string, object>();
      if (subgraphLocalVariableValuesDict != null)
        subgraphLocalVariableValuesDict.Clear();
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        string innerText = firstChild.InnerText;
        XmlNode nextSibling = firstChild.NextSibling;
        string str = "";
        if (nextSibling != null)
          str = nextSibling.InnerText;
        if (!subgraphLocalVariableValuesDict.ContainsKey(innerText))
          subgraphLocalVariableValuesDict.Add(innerText, str);
        else
          Logger.AddError(string.Format("Save load error: FSM {0} subgraph local variable {1} dublicated", fsm.FSMStaticObject.Name, innerText));
      }
    }

    private bool IsEventOwn(DynamicEvent dynEvent)
    {
      bool flag = false;
      if (dynEvent.Entity != null && fsm.Entity != null)
        flag = dynEvent.Entity.EngineGuid == fsm.Entity.EngineGuid;
      return flag;
    }

    private VMEventLink GetOuterOutputLinkByEvent(DynamicEvent evnt)
    {
      VMEventLink outputLinkByEvent1 = null;
      bool ownEvent = false;
      if (evnt.Entity != null && fsm.Entity != null)
        ownEvent = evnt.Entity.EngineGuid == fsm.Entity.EngineGuid;
      for (int index = 0; index < CurrentStateStack.Stack.Count; ++index)
      {
        VMEventLink outputLinkByEvent2 = GetStateOutputLinkByEvent((VMState) CurrentStateStack.Stack.ElementAt(index).currentState, fsm, evnt, ownEvent);
        if (outputLinkByEvent2 != null)
        {
          outputLinkByEvent1 = outputLinkByEvent2;
          break;
        }
      }
      return outputLinkByEvent1;
    }

    private VMEventLink GetOuterEnterLinkByEvent(
      DynamicEvent evnt,
      List<EventMessage> messages,
      IFiniteStateMachine currentGraph = null)
    {
      VMEventLink enterLinkByEvent1 = null;
      bool ownEvent = false;
      if (evnt.Entity != null && fsm.Entity != null)
        ownEvent = evnt.Entity.EngineGuid == fsm.Entity.EngineGuid;
      for (int index = 0; index < CurrentStateStack.Stack.Count; ++index)
      {
        StatePoint statePoint = CurrentStateStack.Stack.ElementAt(index);
        if (statePoint.currentState.Parent != null)
        {
          bool flag = false;
          FiniteStateMachine parent = (FiniteStateMachine) statePoint.currentState.Parent;
          if (currentGraph != null && (long) currentGraph.BaseGuid == (long) parent.BaseGuid)
            flag = true;
          if (!flag)
          {
            VMEventLink enterLinkByEvent2 = GetGraphEnterLinkByEvent(parent, fsm, evnt, messages, ownEvent);
            if (enterLinkByEvent2 != null)
            {
              enterLinkByEvent1 = enterLinkByEvent2;
              break;
            }
          }
        }
      }
      return enterLinkByEvent1;
    }

    private VMEventLink GetStateOutputLinkByEvent(
      VMState state,
      DynamicFSM fsm,
      DynamicEvent dynEvent,
      bool ownEvent)
    {
      return DynamicFsmUtility.GetStateOutputLinkByEvent(state, fsm, dynEvent, ownEvent);
    }

    private VMEventLink GetGraphEnterLinkByEvent(
      IFiniteStateMachine parentGraph,
      DynamicFSM fsm,
      DynamicEvent dynEvent,
      List<EventMessage> messages,
      bool ownEvent)
    {
      return DynamicFsmUtility.GetGraphEnterLinkByEvent(parentGraph, fsm, dynEvent, messages, ownEvent);
    }

    private VMEventLink GetOuterAfterExitLink()
    {
      StatePoint statePoint1 = CurrentStateStack.Stack.ElementAt(0);
      VMEventLink afterExitLink = ((VMState) statePoint1.currentState).GetAfterExitLink();
      if (CurrentStateStack.Stack.Count < 2)
        return afterExitLink;
      StatePoint statePoint2 = CurrentStateStack.Stack.ElementAt(1);
      if (((VMBaseObject) statePoint2.currentState).IsEqual(statePoint1.currentState.Parent))
        return ((VMState) statePoint2.currentState).GetAfterExitLink();
      return statePoint2.currentState.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH && ((FiniteStateMachine) statePoint2.currentState).SubstituteGraph != null && (long) ((FiniteStateMachine) statePoint2.currentState).SubstituteGraph.BaseGuid == (long) statePoint1.currentState.Parent.BaseGuid ? ((VMState) ((FiniteStateMachine) statePoint2.currentState).SubstituteGraph).GetAfterExitLink() : afterExitLink;
    }

    private bool IsCurrentProcedure
    {
      get
      {
        return CurrentState != null && CurrentState.Parent != null && ((VMState) CurrentState.Parent).IsProcedure;
      }
    }

    private void PushLoopToStack(IContextElement ownerContextElement, ICommonList loopList)
    {
      try
      {
        LoopInfo loopInfo = new LoopInfo(ownerContextElement, loopList);
        List<IVariable> contextVariables = ownerContextElement.LocalContextVariables;
        if (loopLocalVariableValuesDict == null)
          loopLocalVariableValuesDict = new Dictionary<string, object>();
        for (int index = 0; index < contextVariables.Count; ++index)
        {
          if (!loopLocalVariableValuesDict.ContainsKey(contextVariables[index].Name))
            loopLocalVariableValuesDict.Add(contextVariables[index].Name, null);
        }
        loopInfo.RegistrLoopLocalVarsDict(loopLocalVariableValuesDict);
        if (loopStack == null)
          loopStack = new Stack<LoopInfo>();
        loopStack.Push(loopInfo);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Push loop stack error at action {0}: {1} at {2}", ownerContextElement.BaseGuid, ex.Message, DynamicFSM.CurrentStateInfo));
      }
    }

    protected void CheckLoopStackClear()
    {
      if (loopStack != null && loopStack.Count > 0)
      {
        Logger.AddError(string.Format("Loop processing error: loop stack not clear after entry action line processing in {0} at {0}!", fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
        loopStack.Clear();
      }
      if (loopLocalVariableValuesDict == null)
        return;
      loopLocalVariableValuesDict.Clear();
    }

    protected virtual bool IsThisTalking => false;

    protected virtual bool IgnoreBranchCase(IState newState) => false;

    private void PopLoopFromStack()
    {
      if (loopStack != null)
        loopStack.Pop();
      else
        Logger.AddError(string.Format("Loop processing error: attempt to pop not inited loop stack in {0} at {0}!", fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
    }

    private void SetCurrentLoopIndexValue(int currentLoopIndex)
    {
      if (loopStack != null && loopStack.Count != 0)
        loopStack.Peek().CurrentLoopIndex = currentLoopIndex;
      else
        Logger.AddError(string.Format("Cannot get current loop index because of loop stack is empty in {0} at {1}", fsm.FSMStaticObject.Name, DynamicFSM.CurrentStateInfo));
    }

    private int GetLoopStartIndexValue(IActionLoop actionLoop)
    {
      object startIndexParam = actionLoop.StartIndexParam;
      if (startIndexParam != null)
      {
        if (typeof (int) == startIndexParam.GetType())
          return (int) startIndexParam;
        if (startIndexParam is CommonVariable)
        {
          CommonVariable variable = (CommonVariable) startIndexParam;
          VMType vmType = new VMType(typeof (int));
          if (!variable.IsBinded)
            variable.Bind(fsm.FSMStaticObject, vmType, ((VMActionLine) actionLoop).LocalContext);
          return (int) ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(variable, vmType, fsm);
        }
      }
      return 0;
    }

    private int GetLoopEndIndexValue(IActionLoop actionLoop)
    {
      object endIndexParam = actionLoop.EndIndexParam;
      if (typeof (int) == endIndexParam.GetType())
        return (int) endIndexParam;
      if (!(endIndexParam is CommonVariable))
        return 0;
      CommonVariable variable = (CommonVariable) endIndexParam;
      VMType vmType = new VMType(typeof (int));
      if (!variable.IsBinded)
        variable.Bind(fsm.FSMStaticObject, vmType, ((VMActionLine) actionLoop).LocalContext);
      return (int) ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(variable, vmType, fsm);
    }

    private ICommonList GetLoopListValue(IActionLoop actionLoop)
    {
      IVariable listParamInstance = actionLoop.LoopListParamInstance;
      if (listParamInstance != null)
      {
        try
        {
          return (ICommonList) ExpressionUtility.GetContextParamValue(fsm, actionLoop.LoopListParam, listParamInstance.Type);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Cannot get loop list value, error: {0}", ex));
        }
      }
      return null;
    }

    private int GetCurrentFlipFlopVariantIndex(IBranch branch)
    {
      if (flipFlopBranchCurrentIndexes == null)
        flipFlopBranchCurrentIndexes = new Dictionary<ulong, int>(UlongComparer.Instance);
      if (!flipFlopBranchCurrentIndexes.ContainsKey(branch.BaseGuid))
        flipFlopBranchCurrentIndexes.Add(branch.BaseGuid, 0);
      int branchCurrentIndex = flipFlopBranchCurrentIndexes[branch.BaseGuid];
      flipFlopBranchCurrentIndexes[branch.BaseGuid]++;
      return branchCurrentIndex;
    }

    private void ResetFlipFlopVariantIndex(IBranch branch)
    {
      if (flipFlopBranchCurrentIndexes == null || !flipFlopBranchCurrentIndexes.ContainsKey(branch.BaseGuid))
        return;
      flipFlopBranchCurrentIndexes[branch.BaseGuid] = 0;
    }

    private void AddSubgraphLocalVariable(string localVarName, object localVarValue)
    {
      if (localVarValue != null && typeof (IEntity).IsAssignableFrom(localVarValue.GetType()))
      {
        Logger.AddError("Add subgraph local variable error: Invalid value type !");
        IEntity entity = (IEntity) localVarValue;
        localVarValue = new VMObjRef();
        ((VMObjRef) localVarValue).Initialize(entity.Id);
      }
      if (subgraphLocalVariableValuesDict == null)
        subgraphLocalVariableValuesDict = new Dictionary<string, object>();
      subgraphLocalVariableValuesDict[localVarName] = localVarValue;
    }

    private void MakeSubgraphInputParams(
      IFiniteStateMachine subGraph,
      IParamListSource inputParamSource)
    {
      if (inputParamSource == null)
        return;
      for (int index = 0; index < subGraph.InputParams.Count; ++index)
      {
        if (index < inputParamSource.SourceParams.Count)
          AddSubgraphLocalVariable(subGraph.InputParams[index].Name, ExpressionUtility.GetContextParamValue(fsm, inputParamSource.SourceParams[index], subGraph.InputParams[index].Type, (ILocalContext) inputParamSource));
      }
    }
  }
}
