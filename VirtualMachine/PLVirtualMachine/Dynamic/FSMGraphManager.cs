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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
    protected static IGraphObject debugCurrentState = (IGraphObject) null;
    private static int CIRCULARING_CHECK_MAX_ITERATIONS_COUNT = 100;
    private static int ACTION_LOOP_CHECK_MAX_ITERATIONS_COUNT = 100000;

    public FSMGraphManager(DynamicFSM fsm)
    {
      this.fsm = fsm;
      this.previosStateReturningMode = false;
      this.SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
    }

    public virtual void StateSave(IDataWriter writer)
    {
      bool flag = this.IsSaveStateValid();
      if (!flag)
        Logger.AddError(string.Format("SaveLoad error: saving entity {0} is in invalid state {1}", (object) this.fsm.Entity.Name, (object) this.CurrentState.Name));
      SaveManagerUtility.Save(writer, "IsStateValid", flag);
      SaveManagerUtility.SaveDynamicSerializable(writer, "MainStateStack", (ISerializeStateSave) this.mainStateStack);
      SaveManagerUtility.SaveDynamicSerializable(writer, "LocalStateStack", (ISerializeStateSave) this.localStateStack);
      SaveManagerUtility.SaveDynamicSerializable(writer, "LastSubgraphStateStack", (ISerializeStateSave) this.lastSubGraphStateStack);
      if (this.currentStateStack == this.mainStateStack)
        SaveManagerUtility.Save(writer, "CurrentStateStackName", "Main");
      else if (this.currentStateStack == this.localStateStack)
      {
        SaveManagerUtility.Save(writer, "CurrentStateStackName", "Local");
      }
      else
      {
        Logger.AddError(string.Format("Invalid current state stack at {0} fsm", (object) this.fsm.FSMStaticObject.Name));
        SaveManagerUtility.Save(writer, "CurrentStateStackName", "Main");
      }
      if (this.flipFlopBranchCurrentIndexes != null)
        SaveManagerUtility.SaveDictionary(writer, "FlipFlopBranchCurrentIndexesData", this.flipFlopBranchCurrentIndexes);
      SaveManagerUtility.Save(writer, "LockingFSM", this.lockingOwnerFSM != null ? this.lockingOwnerFSM.EngineGuid : Guid.Empty);
      if (this.subgraphLocalVariableValuesDict == null)
        return;
      SaveManagerUtility.SaveDictionaryCommon<object>(writer, "SubgraphLocalVariablesData", this.subgraphLocalVariableValuesDict);
    }

    public virtual void LoadFromXML(XmlElement xmlNode)
    {
      if (xmlNode.Name == "IsStateValid")
        this.isStateValid = VMSaveLoadManager.ReadBool((XmlNode) xmlNode);
      else if (xmlNode.Name == "MainStateStack")
      {
        if (this.mainStateStack == null)
          this.mainStateStack = new StateStack();
        this.mainStateStack.LoadFromXML(xmlNode);
      }
      else if (xmlNode.Name == "LocalStateStack")
      {
        if (this.localStateStack == null)
          this.localStateStack = new StateStack(EStateStackType.STATESTACK_TYPE_LOCAL);
        this.localStateStack.LoadFromXML(xmlNode);
      }
      else if (xmlNode.Name == "LastSubgraphStateStack")
      {
        if (this.lastSubGraphStateStack == null)
          this.lastSubGraphStateStack = new StateStack();
        this.lastSubGraphStateStack.LoadFromXML(xmlNode);
      }
      else if (xmlNode.Name == "CurrentStateStackName")
      {
        if (xmlNode.InnerText == "Local")
          this.currentStateStack = this.localStateStack;
        else
          this.currentStateStack = this.mainStateStack;
      }
      else if (xmlNode.Name == "LockingFSM")
        this.lockingOwnerGuid = VMSaveLoadManager.ReadGuid((XmlNode) xmlNode);
      else if (xmlNode.Name == "FlipFlopBranchCurrentIndexesData")
      {
        this.LoadFlipFlopBranchCurrentIndexesData(xmlNode);
      }
      else
      {
        if (!(xmlNode.Name == "SubgraphLocalVariablesData"))
          return;
        this.LoadSubgraphLocalVariablesData(xmlNode);
      }
    }

    public void OnSaveLoad()
    {
      if (this.CurrentState != null)
      {
        try
        {
          if (this.subgraphLocalVariableValuesDict != null)
          {
            List<string> list = this.subgraphLocalVariableValuesDict.Keys.ToList<string>();
            for (int index = 0; index < list.Count; ++index)
            {
              string str = list[index];
              IVariable localContextVariable = this.CurrentState.GetLocalContextVariable(str);
              if (localContextVariable != null)
              {
                object obj = PLVirtualMachine.Common.Data.StringSerializer.ReadValue((string) this.subgraphLocalVariableValuesDict[str], localContextVariable.Type.BaseType);
                this.subgraphLocalVariableValuesDict[str] = obj;
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("SaveLoad error: cannot load local variables dict for fsm in entity {0}, error: {1}", (object) this.fsm.Entity.Name, (object) ex.ToString()));
        }
      }
      if (this.isStateValid)
        return;
      string str1 = "not defined";
      if (this.CurrentState != null)
        str1 = this.CurrentState.Name;
      Logger.AddError(string.Format("SaveLoad error: entity {0} has saved in invalid state {1}", (object) this.fsm.Entity.Name, (object) str1));
    }

    public virtual void AfterSaveLoading()
    {
      if (this.fsm.Entity.SaveLoadInstantiated)
      {
        try
        {
          if (this.CurrentState != null)
          {
            this.OnStateIn(this.CurrentState);
            this.OnChangeState(this.CurrentState);
            List<StatePoint> list = this.CurrentStateStack.Stack.ToList<StatePoint>();
            for (int index = list.Count - 1; index >= 0; --index)
            {
              IState currentState = list[index].currentState;
              if (currentState != null)
              {
                if (currentState.Parent != null && typeof (IFiniteStateMachine).IsAssignableFrom(currentState.Parent.GetType()))
                  this.SubscribeToNullSourceGraphEvents((FiniteStateMachine) currentState.Parent);
                if (typeof (VMState).IsAssignableFrom(currentState.GetType()))
                {
                  if (currentState.IsStable || typeof (IFiniteStateMachine).IsAssignableFrom(currentState.GetType()))
                    this.SubscribeToEvents((VMState) currentState);
                  else
                    this.SubscribeToOnLoadEvent((VMState) currentState);
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("SaveLoad error at {0} fsm afterloading, error: {1}", (object) this.fsm.FSMStaticObject.Name, (object) ex.ToString()));
        }
      }
      if (!(Guid.Empty != this.lockingOwnerGuid))
        return;
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(this.lockingOwnerGuid);
      if (entityByEngineGuid == null)
        return;
      this.lockingOwnerFSM = entityByEngineGuid.GetFSM();
    }

    public void Clear()
    {
      this.mainStateStack = (StateStack) null;
      this.localStateStack = (StateStack) null;
      if (this.contextMessages != null)
        this.contextMessages.Clear();
      if (this.flipFlopBranchCurrentIndexes != null)
        this.flipFlopBranchCurrentIndexes.Clear();
      if (this.eventProcessedStateSequence != null)
        this.eventProcessedStateSequence.Clear();
      if (this.loopStack != null)
        this.loopStack.Clear();
      if (this.loopLocalVariableValuesDict != null)
        this.loopLocalVariableValuesDict.Clear();
      if (this.subgraphLocalVariableValuesDict == null)
        return;
      this.subgraphLocalVariableValuesDict.Clear();
    }

    public IState CurrentState => this.CurrentStateStack.Peek()?.currentState;

    public IState PreviousState
    {
      get
      {
        if (this.CurrentStateStack.Count == 0)
          return (IState) null;
        return this.CurrentStateStack.Peek().prevState == null ? (IState) null : this.CurrentStateStack.Peek().prevState.State;
      }
    }

    public StateStack PreviousStack
    {
      get
      {
        if (this.CurrentStateStack.Count == 0)
          return (StateStack) null;
        return this.CurrentStateStack.Peek().prevState == null ? (StateStack) null : this.CurrentStateStack.Peek().prevState.Stack;
      }
    }

    public virtual void OnProcessEvent(RaisedEventInfo evntInfo)
    {
      DynamicEvent instance = evntInfo.Instance;
      this.DoProcessEvent(evntInfo);
    }

    public void DoProcessEvent(RaisedEventInfo evntInfo)
    {
      DynamicEvent instance = evntInfo.Instance;
      this.currentGraphExecEvent = evntInfo;
      if (DebugUtility.IsDebug)
      {
        if (this.eventProcessedStateSequence == null)
          this.eventProcessedStateSequence = new Dictionary<ulong, IState>((IEqualityComparer<ulong>) UlongComparer.Instance);
        else
          this.eventProcessedStateSequence.Clear();
        if (this.eventProcessedStateSequenceCount == null)
          this.eventProcessedStateSequenceCount = new Dictionary<ulong, int>((IEqualityComparer<ulong>) UlongComparer.Instance);
        else
          this.eventProcessedStateSequenceCount.Clear();
      }
      this.circularingAlarm = false;
      this.previosStateReturningMode = false;
      if (DynamicTalkingFSM.IsTalking)
      {
        bool flag = instance.Name == EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_ON_GLOBAL_TIMER, typeof (VMGameComponent));
        if (!flag && instance.OwnerFSM != null && instance.OwnerFSM.Active && instance.OwnerFSM.Entity.EngineGuid == GameTimeManager.CurrentPlayCharacterEntity.EngineGuid && this.CurrentState != null && this.CurrentState.Parent != null && ((IFiniteStateMachine) this.CurrentState.Parent).GraphType != EGraphType.GRAPH_TYPE_TALKING && ((IFiniteStateMachine) this.CurrentState.Parent).GraphType != EGraphType.GRAPH_TYPE_TRADE)
          flag = true;
        if (!flag)
          return;
      }
      if (this.lockingOwnerFSM != null && this.lockingOwnerFSM.IsActualEvent(evntInfo))
        return;
      ulong num = 0;
      List<EventMessage> messages = evntInfo.Messages;
      if (this.contextMessages == null)
      {
        if (messages.Count > 0)
          this.contextMessages = new Dictionary<string, EventMessage>();
      }
      else
        this.contextMessages.Clear();
      for (int index = 0; index < messages.Count; ++index)
      {
        if (messages[index].Name == "stateId")
          num = (ulong) messages[index].Value;
        this.contextMessages.Add(messages[index].Name, messages[index]);
      }
      VMEventLink moveLink = (VMEventLink) null;
      bool ownEvent = this.IsEventOwn(instance);
      DynamicFSM fsm = this.fsm;
      if (instance.OwnerFSM != null)
      {
        DynamicFSM ownerFsm = instance.OwnerFSM;
      }
      if (num != 0UL && (this.CurrentState == null || (long) this.CurrentState.BaseGuid != (long) num))
        return;
      if (this.CurrentState != null && this.CurrentState.Parent != null && ((FiniteStateMachine) this.CurrentState.Parent).IsSubGraph)
      {
        VMEventLink outputLinkByEvent = this.GetOuterOutputLinkByEvent(instance);
        if (outputLinkByEvent != null)
        {
          this.ProcessLink(outputLinkByEvent);
          return;
        }
      }
      if (this.CurrentState != null)
        moveLink = this.GetStateOutputLinkByEvent((VMState) this.CurrentState, this.fsm, instance, ownEvent);
      if (moveLink == null)
      {
        IFiniteStateMachine parentGraph = this.fsm.FSMStaticObject.StateGraph;
        if (this.CurrentState != null)
          parentGraph = (IFiniteStateMachine) this.CurrentState.Parent;
        moveLink = this.GetGraphEnterLinkByEvent(parentGraph, this.fsm, instance, evntInfo.Messages, ownEvent);
      }
      if (moveLink != null)
        this.ProcessLink(moveLink);
      else if (this.CurrentState != null && this.CurrentState.Parent != null && ((FiniteStateMachine) this.CurrentState.Parent).IsSubGraph)
      {
        VMEventLink enterLinkByEvent = this.GetOuterEnterLinkByEvent(instance, evntInfo.Messages);
        if (enterLinkByEvent != null)
          this.ProcessLink(enterLinkByEvent);
      }
      if (!this.fsm.Entity.Instantiated || this.CurrentState == null)
        return;
      if (((IFiniteStateMachine) this.CurrentState.Parent).GraphType == EGraphType.GRAPH_TYPE_PROCEDURE)
        Logger.AddError(string.Format(" {0} FSM  must return from procedure '{1}' after event '{2}' processing end!", (object) this.fsm.FSMStaticObject.Name, (object) this.CurrentState.Parent.Name, (object) instance.Name));
      if (this.CurrentStateStack.StackType != EStateStackType.STATESTACK_TYPE_LOCAL)
        return;
      Logger.AddError(string.Format(" {0} FSM  must return from procedure '{1}' after event '{2}' processing end!", (object) this.fsm.FSMStaticObject.Name, (object) this.CurrentState.Parent.Name, (object) instance.Name));
    }

    public void SubscribeToEvents(VMState currState)
    {
      if (currState == null)
        return;
      if (typeof (ISpeech).IsAssignableFrom(currState.GetType()))
      {
        DynamicEvent contextEvent = VirtualMachine.Instance.GameRootFsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking)));
        if (contextEvent == null)
          Logger.AddError(string.Format("Speech reply event not created in this FSM at {0}", (object) DynamicFSM.CurrentStateInfo));
        else
          contextEvent.Subscribe(this.fsm);
      }
      else
      {
        for (int index = 0; index < currState.OutputLinks.Count; ++index)
        {
          VMEventLink outputLink = (VMEventLink) currState.OutputLinks[index];
          if (outputLink.Event != null && outputLink.Event.EventInstance != null)
          {
            IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(this.fsm, outputLink.Event.EventOwner, (IBlueprint) outputLink.Owner);
            if (ownerDynamicContext != null)
            {
              if (!ownerDynamicContext.Entity.IsDisposed)
              {
                DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(outputLink.Event.EventInstance.BaseGuid);
                if (eventByStaticGuid == null)
                {
                  Logger.AddError(string.Format("Event subscribing error: event with name {0} and static guid={1} not found in object {2} at {3} fsm", (object) outputLink.Event.EventInstance.Name, (object) outputLink.Event.EventInstance.BaseGuid, (object) outputLink.Event.EventOwner.Name, (object) ownerDynamicContext.FSMStaticObject.Name));
                  ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(outputLink.Event.EventInstance.BaseGuid);
                }
                else
                  eventByStaticGuid.Subscribe(this.fsm);
              }
            }
            else
              Logger.AddError(string.Format("Event subscribing error: event owner by variable {0} not found in {1} FSM at {2}", (object) outputLink.Event.EventOwner.Name, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
          }
        }
      }
    }

    public EGraphType CurrentFSMGraphType
    {
      get
      {
        return this.CurrentState != null && this.CurrentState.Parent != null && typeof (IFiniteStateMachine).IsAssignableFrom(this.CurrentState.Parent.GetType()) ? ((IFiniteStateMachine) this.CurrentState.Parent).GraphType : EGraphType.GRAPH_TYPE_EVENTGRAPH;
      }
    }

    public EventMessage GetContextMessage(string paramName)
    {
      EventMessage eventMessage;
      return this.contextMessages != null && this.contextMessages.TryGetValue(paramName, out eventMessage) ? eventMessage : (EventMessage) null;
    }

    public object GetLocalVariableValue(string varName)
    {
      object localVariableValue1;
      if (this.loopLocalVariableValuesDict != null && this.loopLocalVariableValuesDict.TryGetValue(varName, out localVariableValue1))
        return localVariableValue1;
      object localVariableValue2;
      if (this.subgraphLocalVariableValuesDict != null && this.subgraphLocalVariableValuesDict.TryGetValue(varName, out localVariableValue2))
        return localVariableValue2;
      Logger.AddError(string.Format("Local variable with name {0} not found in {1} FSM at {2}", (object) varName, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
      return (object) null;
    }

    public IState DebugCurrState => this.debugCurrState;

    protected void SetCurrentStateStack(EStateStackType stateStackType, bool bInit = false)
    {
      if (stateStackType == EStateStackType.STATESTACK_TYPE_MAIN)
      {
        this.currentStateStack = this.mainStateStack;
        if (this.CurrentState != null)
        {
          this.OnStateIn(this.CurrentState, false);
          this.OnChangeState(this.CurrentState);
        }
      }
      else
        this.currentStateStack = this.localStateStack;
      if (!bInit)
        return;
      this.currentStateStack.Init();
    }

    private StateStack CurrentStateStack
    {
      get
      {
        if (this.currentStateStack != null)
          return this.currentStateStack;
        Logger.AddError(string.Format("Current state stack not inited!!! at {0}", (object) DynamicFSM.CurrentStateInfo));
        return this.mainStateStack;
      }
    }

    private bool IsSaveStateValid()
    {
      return this.CurrentState == null || ((IFiniteStateMachine) this.CurrentState.Parent).GraphType != EGraphType.GRAPH_TYPE_PROCEDURE && this.CurrentStateStack.StackType != EStateStackType.STATESTACK_TYPE_LOCAL && ((VMState) this.CurrentState).IsStable;
    }

    protected bool ProcessLink(VMEventLink moveLink)
    {
      if (this.circularingAlarm || !this.fsm.Entity.Instantiated || moveLink == null || !moveLink.Enabled)
        return false;
      if (moveLink.Event != null && moveLink.Event.EventInstance != null && this.IsProcedureLink((IEventLink) moveLink))
      {
        if (moveLink.DestState == null)
        {
          Logger.AddError(string.Format("After procedural event {0} in graph {1} must be procedure", (object) moveLink.Event.EventInstance.Name, (object) moveLink.Parent.Name));
          return false;
        }
        if (moveLink.DestState.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GRAPH)
        {
          Logger.AddError(string.Format("After procedural event {0} in graph {1} must be procedure", (object) moveLink.Event.EventInstance.Name, (object) moveLink.Parent.Name));
          return false;
        }
        if (((FiniteStateMachine) moveLink.DestState).GraphType != EGraphType.GRAPH_TYPE_PROCEDURE)
        {
          Logger.AddError(string.Format("After procedural event {0} in graph {1} must be procedure", (object) moveLink.Event.EventInstance.Name, (object) moveLink.Parent.Name));
          return false;
        }
        FiniteStateMachine destState = (FiniteStateMachine) moveLink.DestState;
        if (destState.InputParams.Count > 0)
          this.MakeSubgraphInputParams((IFiniteStateMachine) destState, (IParamListSource) moveLink);
        this.SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_LOCAL, true);
        this.ProcessMoveToState(((FiniteStateMachine) moveLink.DestState).InitState, (IEventLink) moveLink);
        return true;
      }
      if (this.CurrentState != null && !((VMBaseObject) moveLink.Parent).IsEqual((PLVirtualMachine.Common.IObject) this.CurrentState.Parent))
      {
        if (typeof (VMBlueprint) == ((VMBaseObject) moveLink.Parent).Parent.GetType())
        {
          if (this.fsm.FSMStaticObject.IsDerivedFrom(((VMBaseObject) ((VMBaseObject) moveLink.Parent).Parent).BaseGuid, true))
          {
            this.lastSubGraphStateStack = new StateStack(this.CurrentStateStack);
            IState destState = moveLink.DestState;
            int destEntryPoint = moveLink.DestEntryPoint;
            if (this.lockingOwnerFSM == null || destState.IgnoreBlock)
              this.ProcessMoveToState(destState, (IEventLink) moveLink, destEntryPoint);
          }
          return true;
        }
        if (!((VMBaseObject) this.CurrentState).IsEqual((PLVirtualMachine.Common.IObject) moveLink.Parent))
        {
          if (this.lockingOwnerFSM != null && !this.CurrentState.IgnoreBlock)
            return false;
          this.ReturnToOuterGraph((IFiniteStateMachine) moveLink.Parent, moveLink);
          return true;
        }
      }
      IState destState1 = moveLink.DestState;
      int destEntryPoint1 = moveLink.DestEntryPoint;
      bool flag = false;
      if (this.CurrentState == null && destState1 != null)
        flag = destState1.Initial;
      if (destState1 != null)
      {
        if (((this.lockingOwnerFSM == null ? 1 : (destState1.IgnoreBlock ? 1 : 0)) | (flag ? 1 : 0)) == 0 && !this.IsThisTalking)
          return false;
        this.ProcessMoveToState(destState1, (IEventLink) moveLink, destEntryPoint1);
      }
      else
      {
        if (this.lockingOwnerFSM != null && !this.CurrentState.IgnoreBlock)
          return false;
        if (moveLink.ExitFromSubGraph || this.IsThisTalking)
          this.ReturnToOuterGraph(moveLink);
        else
          this.ReturnToPreviousState();
      }
      return true;
    }

    public virtual void ProcessMoveToState(
      IState newState,
      IEventLink inputLink,
      int destEntryPoint = 0)
    {
      if (newState == null)
        Logger.AddError(string.Format("State for moving to not defined in {0} !!!", (object) this.fsm.FSMStaticObject.Name));
      else if (((VMBaseObject) newState).GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
        this.MoveIntoSubGraph((FiniteStateMachine) newState, inputLink, destEntryPoint);
      else
        this.MoveIntoState(newState, destEntryPoint);
    }

    public bool Lock(DynamicFSM lockingFSM)
    {
      if (this.lockingOwnerFSM == null)
      {
        this.lockingOwnerFSM = lockingFSM;
        return true;
      }
      Logger.AddWarning(string.Format("This fsm already locked by {0} FSM of static object id={0}", (object) this.lockingOwnerFSM.StaticObject.BaseGuid));
      return false;
    }

    public bool UnLock(DynamicFSM lockingFSM)
    {
      if (this.lockingOwnerFSM != null && (long) this.lockingOwnerFSM.StaticGuid == (long) lockingFSM.StaticGuid)
      {
        this.lockingOwnerFSM = (DynamicFSM) null;
        return true;
      }
      Logger.AddWarning(string.Format("This fsm is not locked by FSM of static object id={0}", (object) lockingFSM.StaticObject.BaseGuid));
      return false;
    }

    public DynamicFSM LockingFSM => this.lockingOwnerFSM;

    public static void SetCurrentDebugState(IGraphObject currentState)
    {
      FSMGraphManager.debugCurrentState = currentState;
    }

    public static IGraphObject DebugCurrentState => FSMGraphManager.debugCurrentState;

    public static void ClearAll() => FSMGraphManager.debugCurrentState = (IGraphObject) null;

    public void ProcessState(IState state, int destEntryPoint)
    {
      try
      {
        if (!this.fsm.Active || this.OnStateIn(state))
          return;
        this.ExecuteState(state, destEntryPoint);
        this.OnStateExec(state);
        this.OnChangeState(state);
        if (typeof (ISpeech).IsAssignableFrom(state.GetType()))
          return;
        VMEventLink afterExitLink = ((VMState) state).GetAfterExitLink();
        if (afterExitLink == null)
          return;
        this.ProcessLink(afterExitLink);
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString() + "at " + DynamicFSM.CurrentStateInfo);
        throw;
      }
    }

    public bool IsActualEvent(RaisedEventInfo evntInfo)
    {
      bool ownEvent = this.IsEventOwn(evntInfo.Instance);
      VMEventLink vmEventLink = (VMEventLink) null;
      if (this.CurrentState != null && this.CurrentState.Parent != null && ((FiniteStateMachine) this.CurrentState.Parent).IsSubGraph)
        vmEventLink = this.GetOuterOutputLinkByEvent(evntInfo.Instance);
      if (vmEventLink != null)
        return true;
      if (this.CurrentState != null)
        vmEventLink = this.GetStateOutputLinkByEvent((VMState) this.CurrentState, this.fsm, evntInfo.Instance, ownEvent);
      if (vmEventLink != null)
        return true;
      IFiniteStateMachine parentGraph = this.fsm.FSMStaticObject.StateGraph;
      if (this.CurrentState != null)
        parentGraph = (IFiniteStateMachine) this.CurrentState.Parent;
      VMEventLink enterLinkByEvent = this.GetGraphEnterLinkByEvent(parentGraph, this.fsm, evntInfo.Instance, evntInfo.Messages, ownEvent);
      if (enterLinkByEvent != null)
        return true;
      if (this.CurrentState != null && this.CurrentState.Parent != null && ((FiniteStateMachine) this.CurrentState.Parent).IsSubGraph)
        enterLinkByEvent = this.GetOuterEnterLinkByEvent(evntInfo.Instance, evntInfo.Messages);
      return enterLinkByEvent != null;
    }

    private void LoadFlipFlopBranchCurrentIndexesData(XmlElement rootNode)
    {
      if (this.flipFlopBranchCurrentIndexes != null)
        this.flipFlopBranchCurrentIndexes.Clear();
      else if (rootNode.ChildNodes.Count > 0)
        this.flipFlopBranchCurrentIndexes = new Dictionary<ulong, int>((IEqualityComparer<ulong>) UlongComparer.Instance);
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        this.flipFlopBranchCurrentIndexes.Add(VMSaveLoadManager.ReadUlong(firstChild), VMSaveLoadManager.ReadInt(firstChild.NextSibling));
      }
    }

    protected virtual void ReturnToOuterGraph(VMEventLink prevLink = null)
    {
      VMEventLink outerGraphLink = (VMEventLink) null;
      IFiniteStateMachine outherGraph = (IFiniteStateMachine) null;
      if (prevLink != null && prevLink.LinkExitType == ELinkExitType.LINK_EXIT_TYPE_OUTER_EVENT_EXECUTION && this.currentGraphExecEvent != null)
      {
        outerGraphLink = this.GetOuterEnterLinkByEvent(this.currentGraphExecEvent.Instance, this.currentGraphExecEvent.Messages, (IFiniteStateMachine) prevLink.Parent);
        if (outerGraphLink != null)
          outherGraph = (IFiniteStateMachine) outerGraphLink.Parent;
      }
      if (DebugUtility.IsDebug)
      {
        IState state = this.CurrentState ?? (IState) this.fsm.FSMStaticObject.StateGraph;
        if (!this.eventProcessedStateSequence.ContainsKey(state.BaseGuid))
        {
          this.eventProcessedStateSequence.Add(state.BaseGuid, state);
          this.eventProcessedStateSequenceCount.Add(state.BaseGuid, 1);
        }
        else
        {
          int num = this.eventProcessedStateSequenceCount[state.BaseGuid] + 1;
          this.eventProcessedStateSequenceCount[state.BaseGuid] = num;
          if (num > FSMGraphManager.CIRCULARING_CHECK_MAX_ITERATIONS_COUNT)
          {
            this.circularingAlarm = true;
            Logger.AddError(string.Format("State {0} process is cycling!!! at {1}", (object) state.Name, (object) DynamicFSM.CurrentStateInfo));
            return;
          }
        }
      }
      if (outerGraphLink == null)
        outerGraphLink = this.GetOuterAfterExitLink();
      if (outerGraphLink != null)
      {
        this.ReturnToOuterGraph(outherGraph, outerGraphLink);
      }
      else
      {
        if (this.CurrentStateStack.Count > 1)
          this.PopState();
        this.ReturnToPreviousState();
      }
    }

    private void ReturnToOuterGraph(IFiniteStateMachine outherGraph, VMEventLink outerGraphLink = null)
    {
      if (this.CurrentStateStack.StackType == EStateStackType.STATESTACK_TYPE_LOCAL & this.IsCurrentProcedure && this.CurrentStateStack.Count == 1)
      {
        this.SetCurrentStateStack(EStateStackType.STATESTACK_TYPE_MAIN);
      }
      else
      {
        this.lastSubGraphStateStack = outherGraph == null || outerGraphLink == null || this.CurrentStateStack.StackType != EStateStackType.STATESTACK_TYPE_MAIN ? (StateStack) null : new StateStack(this.CurrentStateStack);
        IState previousState = this.PreviousState;
        IState currentState = this.CurrentState;
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
          while (this.CurrentState != null)
          {
            if (this.CurrentState.Parent == null)
              Logger.AddError(string.Format("Current state {0} id={1} has null parent", (object) this.CurrentState.Name, (object) this.CurrentState.BaseGuid));
            if (!this.CurrentState.Parent.IsEqual((PLVirtualMachine.Common.IObject) outherGraph))
            {
              this.PopState();
              if (this.CurrentState == null)
              {
                Logger.AddError(string.Format("Invalid outher graph returning: return graph id={0} not found in state stack", (object) outherGraph.BaseGuid));
                goto label_15;
              }
            }
            else
              goto label_15;
          }
          Logger.AddError(string.Format("Invalid outher graph returning: return graph id={0} not found in state stack", (object) outherGraph.BaseGuid));
        }
        else if (this.CurrentStateStack.Count > 1 || outerGraphLink == null)
          this.PopState();
label_15:
        string str3 = "none";
        if (outherGraph != null)
          str3 = outherGraph.Name;
        if (this.CurrentState == null)
          Logger.AddError(string.Format("Invalid outher graph returning: CurrentState is not defined in graph {0} after returning from state {1}, graph {2}", (object) str3, (object) str1, (object) str2));
        else if (this.CurrentState.Parent == null)
        {
          Logger.AddError(string.Format("Invalid outher graph returning: CurrentState.Parent is not defined in graph {0}", (object) str3));
        }
        else
        {
          this.previosStateReturningMode = true;
          if (outerGraphLink != null && outerGraphLink.DestState != null)
            this.previosStateReturningMode = false;
          bool flag = false;
          if (outerGraphLink != null)
            flag = this.ProcessLink(outerGraphLink);
          if (flag)
            return;
          this.ReturnToPreviousState();
        }
      }
    }

    protected void ReturnToPreviousState(bool prevIsCurrent = false)
    {
      if (this.circularingAlarm)
        return;
      this.previosStateReturningMode = true;
      IState state = this.PreviousState;
      if (prevIsCurrent)
      {
        if (this.lastSubGraphStateStack != null)
        {
          this.ReturnToMemoryStack(this.lastSubGraphStateStack);
          if (this.lockingOwnerFSM != null)
            this.lockingOwnerFSM.SetLockedObjectNeedRestoreAction(this.fsm);
          this.lastSubGraphStateStack = (StateStack) null;
          return;
        }
        state = this.CurrentState;
      }
      if (state != null && DebugUtility.IsDebug && this.eventProcessedStateSequence.ContainsKey(state.BaseGuid) && this.PreviousState != null && (long) this.PreviousState.BaseGuid != (long) state.BaseGuid)
        state = this.PreviousState;
      if (state != null)
      {
        if (((VMBaseObject) state).GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH)
          this.MoveIntoSubGraph((FiniteStateMachine) state, (IEventLink) null);
        else
          this.MoveIntoState(state, 0);
      }
      else
      {
        StateStack previousStack = this.PreviousStack;
        if (previousStack != null)
          this.ReturnToMemoryStack(previousStack);
        else if (this.CurrentState != null && this.CurrentState.Parent != null)
          this.ProcessMoveToState(((FiniteStateMachine) this.CurrentState.Parent).InitState, (IEventLink) null);
      }
      if (this.lockingOwnerFSM == null)
        return;
      this.lockingOwnerFSM.SetLockedObjectNeedRestoreAction(this.fsm);
    }

    protected void MoveIntoSubGraph(
      FiniteStateMachine subGraph,
      IEventLink inputLink,
      int destEntryPoint = 0)
    {
      this.currentGraphExecEvent = (RaisedEventInfo) null;
      if (this.OnStateIn((IState) subGraph))
        return;
      IState prevState = this.CurrentState;
      if (this.CurrentState.IsProcedure)
        prevState = this.PreviousState;
      if ((!this.CurrentState.IsProcedure ? 0 : (subGraph.IsProcedure ? 1 : 0)) == 0)
      {
        bool flag = false;
        if (subGraph.Parent == null || this.CurrentState == null)
          Logger.AddError(string.Format("Invalid subgraph {0} enter in {1} fsm", (object) subGraph.Name, (object) this.fsm.FSMStaticObject.Name));
        else if ((long) subGraph.Parent.BaseGuid == (long) this.CurrentState.BaseGuid)
          flag = true;
        if (!flag)
          this.PopState();
        this.PushState(prevState, (IState) subGraph);
      }
      else
        this.SwitchState((IState) subGraph);
      IState initState = subGraph.InitState;
      if (!this.IsCurrentProcedure)
        this.SubscribeToNullSourceGraphEvents(subGraph);
      if (subGraph.InputParams.Count > 0 && inputLink != null)
        this.MakeSubgraphInputParams((IFiniteStateMachine) subGraph, (IParamListSource) inputLink);
      this.MoveIntoState(initState, destEntryPoint, true);
    }

    protected virtual void OnChangeState(IState state)
    {
    }

    protected virtual void MoveIntoState(IState newState, int destEntryPoint, bool bNextLevel = false)
    {
      if (newState == null)
      {
        Logger.AddError(string.Format("New state not defined in object {0}!", (object) this.fsm.FSMStaticObject.Name));
      }
      else
      {
        FSMGraphManager.debugCurrentState = (IGraphObject) newState;
        if (destEntryPoint < 0 || destEntryPoint >= newState.EntryPoints.Count)
          Logger.AddError(string.Format("Invalid entry point index at move to {0} state in {1} object", (object) newState.Name, (object) this.fsm.StaticGuid));
        else if (typeof (IBranch).IsAssignableFrom(newState.GetType()))
          this.ProcessBranch((IBranch) newState);
        else if (this.CurrentState != null && (long) newState.BaseGuid == (long) this.CurrentState.BaseGuid)
        {
          this.ProcessState(newState, destEntryPoint);
        }
        else
        {
          IState currentState = this.CurrentState;
          bool flag = false;
          if (!bNextLevel && !flag)
            this.PopState();
          if (!flag)
            this.PushState(currentState, newState);
          this.ProcessState(newState, destEntryPoint);
        }
      }
    }

    protected void PushState(IState prevState, IState newState)
    {
      if (newState == null)
      {
        Logger.AddError(string.Format("New state not defined in object {0}!", (object) this.fsm.FSMStaticObject.Name));
      }
      else
      {
        StatePoint newStatePoint;
        if (prevState != null)
        {
          if (typeof (IFiniteStateMachine).IsAssignableFrom(prevState.GetType()))
          {
            if (this.lastSubGraphStateStack != null)
            {
              newStatePoint = new StatePoint(newState, this.lastSubGraphStateStack);
              this.lastSubGraphStateStack = (StateStack) null;
              ++this.currHistoryLevel;
            }
            else
              newStatePoint = new StatePoint(newState);
          }
          else
            newStatePoint = new StatePoint(newState, prevState);
        }
        else
          newStatePoint = new StatePoint(newState);
        this.CurrentStateStack.Push(newStatePoint);
        if (!this.IsCurrentProcedure)
          this.SubscribeToEvents((VMState) this.CurrentState);
        if (!((VMState) this.CurrentState).Initial || ((FiniteStateMachine) this.CurrentState.Parent).IsSubGraph)
          return;
        FiniteStateMachine parent = (FiniteStateMachine) this.CurrentState.Parent;
        FSMGraphManager.debugCurrentState = (IGraphObject) parent;
        this.SubscribeToNullSourceGraphEvents(parent);
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
              this.SubscribeToNullSourceGraphEvents((FiniteStateMachine) baseBlueprints[index].StateGraph);
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    protected void SwitchState(IState newState)
    {
      this.CurrentStateStack.Stack.Peek().currentState = newState;
      if (newState.IsProcedure)
        return;
      this.SubscribeToEvents((VMState) newState);
    }

    protected IState PopState()
    {
      if (this.CurrentStateStack.Count == 0)
        return (IState) null;
      IState currentState = this.CurrentStateStack.Pop().currentState;
      if (!this.IsCurrentProcedure)
      {
        if (typeof (FiniteStateMachine).IsAssignableFrom(currentState.GetType()))
          this.ResetNullSourceGraphEvents((FiniteStateMachine) currentState);
        this.ResetEvents((VMState) currentState);
      }
      return currentState;
    }

    protected bool OnStateIn(IState state, bool bCanBlock = true)
    {
      this.fsm.OnModify();
      if (DebugUtility.IsDebug)
      {
        string str1 = "";
        if (state.Parent != null)
          str1 = str1 + state.Parent.Name + ".";
        string str2 = str1 + state.Name;
        FSMGraphManager.debugCurrentState = (IGraphObject) state;
        if (this.fsm.Entity != null)
          this.fsm.Entity.DebugContextInfo = str2;
        if ((long) this.debugCurrStepStateGuid != (long) state.BaseGuid && DebugUtility.Debugger.OnStateInput(this.fsm, state) && bCanBlock)
        {
          this.debugCurrStepStateGuid = state.BaseGuid;
          return true;
        }
        this.debugCurrState = state;
      }
      return false;
    }

    private void OnStateExec(IState state) => DebugUtility.OnStateExec(this.fsm, state);

    private void ExecuteState(IState state, int destEntryPoint)
    {
      if (DebugUtility.IsDebug)
      {
        if (!this.eventProcessedStateSequence.ContainsKey(((VMBaseObject) state).BaseGuid))
        {
          this.eventProcessedStateSequence.Add(((VMBaseObject) state).BaseGuid, state);
          this.eventProcessedStateSequenceCount.Add(((VMBaseObject) state).BaseGuid, 1);
        }
        else
        {
          int num = this.eventProcessedStateSequenceCount[((VMBaseObject) state).BaseGuid] + 1;
          this.eventProcessedStateSequenceCount[((VMBaseObject) state).BaseGuid] = num;
          if (num > FSMGraphManager.CIRCULARING_CHECK_MAX_ITERATIONS_COUNT)
          {
            this.circularingAlarm = true;
            Logger.AddError(string.Format("State {0} process is cycling!!! at {1}", (object) state.Name, (object) DynamicFSM.CurrentStateInfo));
            return;
          }
        }
      }
      if (this.IsReturnToPrevios())
        return;
      this.ProcessActionLine(state.EntryPoints[destEntryPoint].ActionLine);
      this.CheckLoopStackClear();
    }

    private bool IsReturnToPrevios()
    {
      int num = this.previosStateReturningMode ? 1 : 0;
      this.previosStateReturningMode = false;
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
        int loopStartIndexValue = this.GetLoopStartIndexValue(actionLoop);
        int num = this.GetLoopEndIndexValue(actionLoop);
        ICommonList loopListValue = this.GetLoopListValue(actionLoop);
        if (loopListValue != null)
        {
          if (num > loopListValue.ObjectsCount)
            num = loopListValue.ObjectsCount;
        }
        else
          loopListValue = this.GetLoopListValue(actionLoop);
        this.PushLoopToStack((IContextElement) actionLine, loopListValue);
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
            this.SetCurrentLoopIndexValue(randomIndexes[index]);
            this.DoProcessActionLine(actionLine);
          }
        }
        else
        {
          if (num - loopStartIndexValue >= FSMGraphManager.ACTION_LOOP_CHECK_MAX_ITERATIONS_COUNT)
          {
            num = loopStartIndexValue + FSMGraphManager.ACTION_LOOP_CHECK_MAX_ITERATIONS_COUNT;
            Logger.AddError(string.Format("Invalid loop indexing (infinity loop) in loop action line id={0} at {1}", (object) actionLine.BaseGuid, (object) DynamicFSM.CurrentStateInfo));
          }
          for (int currentLoopIndex = loopStartIndexValue; currentLoopIndex < num; ++currentLoopIndex)
          {
            this.SetCurrentLoopIndexValue(currentLoopIndex);
            this.DoProcessActionLine(actionLine);
          }
        }
        this.PopLoopFromStack();
      }
      else
        this.DoProcessActionLine(actionLine, bFromSpeech);
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
            this.ProcessActionLine((IActionLine) action);
          }
          else
          {
            ExpressionUtility.ProcessAction((IAbstractAction) action, (IDynamicGameObjectContext) this.fsm);
            if (!bFromSpeech && this.fsm.Active)
              this.fsm.OnProcessActionLine();
          }
          if (!this.fsm.Active)
            break;
        }
      }
    }

    private void ReturnToMemoryStack(StateStack prevMemoryStack)
    {
      do
      {
        this.PopState();
      }
      while (this.CurrentState != null);
      this.CurrentStateStack.Clear();
      Stack<StatePoint> source = new Stack<StatePoint>((IEnumerable<StatePoint>) prevMemoryStack.Stack);
      do
      {
        this.CurrentStateStack.Push(source.Pop());
        this.SubscribeToEvents((VMState) this.CurrentState);
        if (this.CurrentState.Parent != null)
          this.SubscribeToNullSourceGraphEvents((FiniteStateMachine) this.CurrentState.Parent);
      }
      while (source.Count<StatePoint>() > 0);
      if (this.currHistoryLevel > 0)
        --this.currHistoryLevel;
      this.previosStateReturningMode = true;
      this.ProcessState(this.CurrentState, 0);
    }

    protected void ProcessBranch(IBranch branch)
    {
      if (this.OnStateIn((IState) branch))
        return;
      if (branch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH || branch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH)
      {
        this.ProcessMinMaxBranch(branch);
      }
      else
      {
        int num1 = -1;
        if (branch.StateType == EStateType.STATE_TYPE_FLIPFLOP_BRANCH)
        {
          num1 = this.GetCurrentFlipFlopVariantIndex(branch);
          if (num1 >= branch.GetExitPointsCount())
          {
            this.ResetFlipFlopVariantIndex(branch);
            num1 = this.GetCurrentFlipFlopVariantIndex(branch);
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
            flag = index >= branch.GetExitPointsCount() - 1 || ExpressionUtility.CalculateConditionResult(branchCondition, (IDynamicGameObjectContext) this.fsm);
          }
          if (flag)
          {
            VMEventLink sourceExitPointIndex = ((VMState) branch).GetOutputLinkBySourceExitPointIndex(index);
            if (sourceExitPointIndex != null)
            {
              int num2 = this.IgnoreBranchCase(sourceExitPointIndex.DestState) ? 1 : 0;
              if (num2 == 0 || index >= branch.GetExitPointsCount() - 1)
              {
                if (sourceExitPointIndex.DestState != null)
                  this.ProcessLink(sourceExitPointIndex);
                else if (sourceExitPointIndex.ExitFromSubGraph || this.IsThisTalking)
                  this.ReturnToOuterGraph(sourceExitPointIndex);
                else
                  this.ReturnToPreviousState(true);
              }
              if (num2 == 0)
                break;
            }
            else
            {
              this.ReturnToPreviousState(true);
              break;
            }
          }
        }
      }
    }

    protected void ProcessMinMaxBranch(IBranch branch)
    {
      object secondValue = (object) 0;
      int iSrcPntIndex = -1;
      if (branch.StateType == EStateType.STATE_TYPE_MINVALUE_BRANCH)
        secondValue = (object) float.MaxValue;
      else if (branch.StateType == EStateType.STATE_TYPE_MAXVALUE_BRANCH)
        secondValue = (object) float.MinValue;
      for (int exitPntIndex = 0; exitPntIndex < branch.GetExitPointsCount(); ++exitPntIndex)
      {
        VMPartCondition branchCondition = (VMPartCondition) branch.GetBranchCondition(exitPntIndex);
        if (branchCondition != null)
        {
          VMExpression firstExpression = (VMExpression) branchCondition.FirstExpression;
          if (firstExpression != null)
          {
            object expressionResult = ExpressionUtility.CalculateExpressionResult((IExpression) firstExpression, (IDynamicGameObjectContext) this.fsm);
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
            Logger.AddError(string.Format("MinMax branch {0} expression not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        }
      }
      if (iSrcPntIndex < 0)
        return;
      VMEventLink sourceExitPointIndex = ((VMState) branch).GetOutputLinkBySourceExitPointIndex(iSrcPntIndex);
      if (sourceExitPointIndex != null)
      {
        IState destState = sourceExitPointIndex.DestState;
        if (sourceExitPointIndex.DestState != null)
          this.ProcessLink(sourceExitPointIndex);
        else if (sourceExitPointIndex.ExitFromSubGraph || this.IsThisTalking)
          this.ReturnToOuterGraph(sourceExitPointIndex);
        else
          this.ReturnToPreviousState(true);
      }
      else
        this.ReturnToPreviousState(true);
    }

    private void SubscribeToOnLoadEvent(VMState state)
    {
      DynamicEvent contextEvent = VirtualMachine.Instance.GameRootFsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_LOAD_GAME, typeof (VMGameComponent)));
      if (contextEvent == null)
        Logger.AddError(string.Format("Game onload event not found at {0}", (object) DynamicFSM.CurrentStateInfo));
      else
        contextEvent.Subscribe(this.fsm);
    }

    private void SubscribeToNullSourceGraphEvents(FiniteStateMachine eventGraph)
    {
      for (int index = 0; index < eventGraph.EnterLinks.Count; ++index)
      {
        VMEventLink enterLink = (VMEventLink) eventGraph.EnterLinks[index];
        if (!enterLink.IsInitial() && enterLink.Event != null && enterLink.Event.EventInstance != null)
        {
          IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(this.fsm, enterLink.Event.EventOwner, (IBlueprint) enterLink.Owner);
          if (ownerDynamicContext != null)
          {
            if (!ownerDynamicContext.Entity.IsDisposed)
            {
              DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(enterLink.Event.EventInstance.BaseGuid);
              if (eventByStaticGuid == null)
              {
                Logger.AddError(string.Format("Event subscribing error: event with name {0} and static guid={1} not found in object {2} at {3} fsm", (object) enterLink.Event.EventInstance.Name, (object) enterLink.Event.EventInstance.BaseGuid, (object) enterLink.Event.EventOwner.Name, (object) ownerDynamicContext.FSMStaticObject.Name));
                ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(enterLink.Event.EventInstance.BaseGuid);
              }
              else
                eventByStaticGuid.Subscribe(this.fsm);
            }
          }
          else
            Logger.AddError(string.Format("Event subscribing error: event owner by variable {0} not found in {1} FSM at {2}", (object) enterLink.Event.EventOwner.Name, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        }
      }
      if (!typeof (VMBlueprint).IsAssignableFrom(eventGraph.Parent.GetType()))
        return;
      List<IBlueprint> baseBlueprints = ((VMBlueprint) eventGraph.Parent).BaseBlueprints;
      if (baseBlueprints == null)
        return;
      for (int index = 0; index < baseBlueprints.Count; ++index)
        this.SubscribeToNullSourceGraphEvents((FiniteStateMachine) baseBlueprints[index].StateGraph);
    }

    private void ResetNullSourceGraphEvents(FiniteStateMachine eventGraph)
    {
      for (int index = 0; index < eventGraph.EnterLinks.Count; ++index)
      {
        VMEventLink enterLink = (VMEventLink) eventGraph.EnterLinks[index];
        if (enterLink.Event != null && enterLink.Event.EventInstance != null)
        {
          IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(this.fsm, enterLink.Event.EventOwner, (IBlueprint) enterLink.Owner);
          if (ownerDynamicContext != null)
          {
            if (!ownerDynamicContext.Entity.IsDisposed)
            {
              DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(enterLink.Event.EventInstance.BaseGuid);
              if (eventByStaticGuid != null)
                eventByStaticGuid.DeSubscribe(this.fsm);
              else
                Logger.AddError(string.Format("Event desubscribing error: event with static guid={0} not found in {1} FSM at {2}", (object) enterLink.Event.EventInstance.BaseGuid, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
            }
          }
          else
            Logger.AddWarning(string.Format("Event desubscribing error: event owner by variable {0} not found in {1} FSM at {2}", (object) enterLink.Event.EventOwner.Name, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        }
      }
    }

    private void ResetEvents(VMState remState)
    {
      if (typeof (ISpeech).IsAssignableFrom(remState.GetType()))
      {
        DynamicEvent contextEvent = VirtualMachine.Instance.GameRootFsm.GetContextEvent(EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking)));
        if (contextEvent == null)
          Logger.AddError(string.Format("Speech reply event not created in this FSM"));
        else
          contextEvent.DeSubscribe(this.fsm);
      }
      for (int index = 0; index < remState.OutputLinks.Count; ++index)
      {
        VMEventLink outputLink = (VMEventLink) remState.OutputLinks[index];
        if (outputLink.Event != null && outputLink.Event.EventInstance != null)
        {
          IDynamicGameObjectContext ownerDynamicContext = DynamicFsmUtility.GetEventOwnerDynamicContext(this.fsm, outputLink.Event.EventOwner, (IBlueprint) outputLink.Owner);
          if (ownerDynamicContext != null)
          {
            if (!ownerDynamicContext.Entity.IsDisposed)
            {
              DynamicEvent eventByStaticGuid = ((DynamicFSM) ownerDynamicContext).GetEventByStaticGuid(outputLink.Event.EventInstance.BaseGuid);
              if (eventByStaticGuid != null)
                eventByStaticGuid.DeSubscribe(this.fsm);
              else
                Logger.AddError(string.Format("Event desubscribing error: event with static guid={0} not found in {1} FSM at {2}", (object) outputLink.Event.EventInstance.BaseGuid, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
            }
          }
          else
            Logger.AddWarning(string.Format("Event desubscribing error: event owner by variable {0} not found in {1} FSM at {2}", (object) outputLink.Event.EventOwner.Name, (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
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
      if (rootNode.ChildNodes.Count > 0 && this.subgraphLocalVariableValuesDict == null)
        this.subgraphLocalVariableValuesDict = new Dictionary<string, object>();
      if (this.subgraphLocalVariableValuesDict != null)
        this.subgraphLocalVariableValuesDict.Clear();
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        string innerText = firstChild.InnerText;
        XmlNode nextSibling = firstChild.NextSibling;
        string str = "";
        if (nextSibling != null)
          str = nextSibling.InnerText;
        if (!this.subgraphLocalVariableValuesDict.ContainsKey(innerText))
          this.subgraphLocalVariableValuesDict.Add(innerText, (object) str);
        else
          Logger.AddError(string.Format("Save load error: FSM {0} subgraph local variable {1} dublicated", (object) this.fsm.FSMStaticObject.Name, (object) innerText));
      }
    }

    private bool IsEventOwn(DynamicEvent dynEvent)
    {
      bool flag = false;
      if (dynEvent.Entity != null && this.fsm.Entity != null)
        flag = dynEvent.Entity.EngineGuid == this.fsm.Entity.EngineGuid;
      return flag;
    }

    private VMEventLink GetOuterOutputLinkByEvent(DynamicEvent evnt)
    {
      VMEventLink outputLinkByEvent1 = (VMEventLink) null;
      bool ownEvent = false;
      if (evnt.Entity != null && this.fsm.Entity != null)
        ownEvent = evnt.Entity.EngineGuid == this.fsm.Entity.EngineGuid;
      for (int index = 0; index < this.CurrentStateStack.Stack.Count; ++index)
      {
        VMEventLink outputLinkByEvent2 = this.GetStateOutputLinkByEvent((VMState) this.CurrentStateStack.Stack.ElementAt<StatePoint>(index).currentState, this.fsm, evnt, ownEvent);
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
      VMEventLink enterLinkByEvent1 = (VMEventLink) null;
      bool ownEvent = false;
      if (evnt.Entity != null && this.fsm.Entity != null)
        ownEvent = evnt.Entity.EngineGuid == this.fsm.Entity.EngineGuid;
      for (int index = 0; index < this.CurrentStateStack.Stack.Count; ++index)
      {
        StatePoint statePoint = this.CurrentStateStack.Stack.ElementAt<StatePoint>(index);
        if (statePoint.currentState.Parent != null)
        {
          bool flag = false;
          FiniteStateMachine parent = (FiniteStateMachine) statePoint.currentState.Parent;
          if (currentGraph != null && (long) currentGraph.BaseGuid == (long) parent.BaseGuid)
            flag = true;
          if (!flag)
          {
            VMEventLink enterLinkByEvent2 = this.GetGraphEnterLinkByEvent((IFiniteStateMachine) parent, this.fsm, evnt, messages, ownEvent);
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
      StatePoint statePoint1 = this.CurrentStateStack.Stack.ElementAt<StatePoint>(0);
      VMEventLink afterExitLink = ((VMState) statePoint1.currentState).GetAfterExitLink();
      if (this.CurrentStateStack.Stack.Count < 2)
        return afterExitLink;
      StatePoint statePoint2 = this.CurrentStateStack.Stack.ElementAt<StatePoint>(1);
      if (((VMBaseObject) statePoint2.currentState).IsEqual((PLVirtualMachine.Common.IObject) statePoint1.currentState.Parent))
        return ((VMState) statePoint2.currentState).GetAfterExitLink();
      return statePoint2.currentState.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GRAPH && ((FiniteStateMachine) statePoint2.currentState).SubstituteGraph != null && (long) ((FiniteStateMachine) statePoint2.currentState).SubstituteGraph.BaseGuid == (long) statePoint1.currentState.Parent.BaseGuid ? ((VMState) ((FiniteStateMachine) statePoint2.currentState).SubstituteGraph).GetAfterExitLink() : afterExitLink;
    }

    private bool IsCurrentProcedure
    {
      get
      {
        return this.CurrentState != null && this.CurrentState.Parent != null && ((VMState) this.CurrentState.Parent).IsProcedure;
      }
    }

    private void PushLoopToStack(IContextElement ownerContextElement, ICommonList loopList)
    {
      try
      {
        LoopInfo loopInfo = new LoopInfo(ownerContextElement, loopList);
        List<IVariable> contextVariables = ownerContextElement.LocalContextVariables;
        if (this.loopLocalVariableValuesDict == null)
          this.loopLocalVariableValuesDict = new Dictionary<string, object>();
        for (int index = 0; index < contextVariables.Count; ++index)
        {
          if (!this.loopLocalVariableValuesDict.ContainsKey(contextVariables[index].Name))
            this.loopLocalVariableValuesDict.Add(contextVariables[index].Name, (object) null);
        }
        loopInfo.RegistrLoopLocalVarsDict(this.loopLocalVariableValuesDict);
        if (this.loopStack == null)
          this.loopStack = new Stack<LoopInfo>();
        this.loopStack.Push(loopInfo);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Push loop stack error at action {0}: {1} at {2}", (object) ownerContextElement.BaseGuid, (object) ex.Message, (object) DynamicFSM.CurrentStateInfo));
      }
    }

    protected void CheckLoopStackClear()
    {
      if (this.loopStack != null && this.loopStack.Count > 0)
      {
        Logger.AddError(string.Format("Loop processing error: loop stack not clear after entry action line processing in {0} at {0}!", (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        this.loopStack.Clear();
      }
      if (this.loopLocalVariableValuesDict == null)
        return;
      this.loopLocalVariableValuesDict.Clear();
    }

    protected virtual bool IsThisTalking => false;

    protected virtual bool IgnoreBranchCase(IState newState) => false;

    private void PopLoopFromStack()
    {
      if (this.loopStack != null)
        this.loopStack.Pop();
      else
        Logger.AddError(string.Format("Loop processing error: attempt to pop not inited loop stack in {0} at {0}!", (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
    }

    private void SetCurrentLoopIndexValue(int currentLoopIndex)
    {
      if (this.loopStack != null && this.loopStack.Count != 0)
        this.loopStack.Peek().CurrentLoopIndex = currentLoopIndex;
      else
        Logger.AddError(string.Format("Cannot get current loop index because of loop stack is empty in {0} at {1}", (object) this.fsm.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
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
            variable.Bind((IContext) this.fsm.FSMStaticObject, vmType, ((VMActionLine) actionLoop).LocalContext);
          return (int) ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(variable, vmType, (IDynamicGameObjectContext) this.fsm);
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
        variable.Bind((IContext) this.fsm.FSMStaticObject, vmType, ((VMActionLine) actionLoop).LocalContext);
      return (int) ((VMVariableService) IVariableService.Instance).GetDynamicVariableValue(variable, vmType, (IDynamicGameObjectContext) this.fsm);
    }

    private ICommonList GetLoopListValue(IActionLoop actionLoop)
    {
      IVariable listParamInstance = actionLoop.LoopListParamInstance;
      if (listParamInstance != null)
      {
        try
        {
          return (ICommonList) ExpressionUtility.GetContextParamValue((IDynamicGameObjectContext) this.fsm, actionLoop.LoopListParam, listParamInstance.Type);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Cannot get loop list value, error: {0}", (object) ex));
        }
      }
      return (ICommonList) null;
    }

    private int GetCurrentFlipFlopVariantIndex(IBranch branch)
    {
      if (this.flipFlopBranchCurrentIndexes == null)
        this.flipFlopBranchCurrentIndexes = new Dictionary<ulong, int>((IEqualityComparer<ulong>) UlongComparer.Instance);
      if (!this.flipFlopBranchCurrentIndexes.ContainsKey(branch.BaseGuid))
        this.flipFlopBranchCurrentIndexes.Add(branch.BaseGuid, 0);
      int branchCurrentIndex = this.flipFlopBranchCurrentIndexes[branch.BaseGuid];
      this.flipFlopBranchCurrentIndexes[branch.BaseGuid]++;
      return branchCurrentIndex;
    }

    private void ResetFlipFlopVariantIndex(IBranch branch)
    {
      if (this.flipFlopBranchCurrentIndexes == null || !this.flipFlopBranchCurrentIndexes.ContainsKey(branch.BaseGuid))
        return;
      this.flipFlopBranchCurrentIndexes[branch.BaseGuid] = 0;
    }

    private void AddSubgraphLocalVariable(string localVarName, object localVarValue)
    {
      if (localVarValue != null && typeof (IEntity).IsAssignableFrom(localVarValue.GetType()))
      {
        Logger.AddError(string.Format("Add subgraph local variable error: Invalid value type !"));
        IEntity entity = (IEntity) localVarValue;
        localVarValue = (object) new VMObjRef();
        ((VMObjRef) localVarValue).Initialize(entity.Id);
      }
      if (this.subgraphLocalVariableValuesDict == null)
        this.subgraphLocalVariableValuesDict = new Dictionary<string, object>();
      this.subgraphLocalVariableValuesDict[localVarName] = localVarValue;
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
          this.AddSubgraphLocalVariable(subGraph.InputParams[index].Name, ExpressionUtility.GetContextParamValue((IDynamicGameObjectContext) this.fsm, inputParamSource.SourceParams[index], subGraph.InputParams[index].Type, (ILocalContext) inputParamSource));
      }
    }
  }
}
