using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Services;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.FSM;
using PLVirtualMachine.Objects;
using PLVirtualMachine.Time;

namespace PLVirtualMachine.Dynamic
{
  public class DynamicFSM : 
    DynamicObject,
    IDynamicGameObjectContext,
    ISerializeStateSave,
    IDynamicLoadSerializable,
    INeedSave
  {
    private bool initialized;
    private bool passive;
    private List<DynamicParameter> refParams;
    private List<KeyValuePair<ulong, Guid>> refParamsData;
    private DynamicEvent outerStartingEvent;
    protected FSMGraphManager graphManager;
    protected FSMEventManager eventManager;
    protected FSMParamsManager paramsManager;
    protected FSMFunctionManager functionManager;
    protected static DynamicFSM debugThinkingFSM;
    public static int TotalCreatedFSMCount = 0;
    public static int ActiveCreatedFSMCount = 0;
    public static double FsmCreationTimeMax = 0.0;
    public static double FsmGraphManagerCreationTimeMax = 0.0;
    public static double FsmEventManagerCreationTimeMax = 0.0;
    public static double FsmParamsManagerCreationTimeMax = 0.0;
    public static double FsmFunctionManagerCreationTimeMax = 0.0;
    public static double FsmCreationTimeMaxRT = 0.0;
    public static double FsmGraphManagerCreationTimeMaxRT = 0.0;
    public static double FsmEventManagerCreationTimeMaxRT = 0.0;
    public static double FsmParamsManagerCreationTimeMaxRT = 0.0;
    public static double FsmFunctionManagerCreationTimeMaxRT = 0.0;

    public DynamicFSM(VMEntity entity, VMLogicObject templateObj)
      : base(entity, false)
    {
      entity.InitFSM(this);
      initialized = false;
      InitStatic(templateObj);
      if (Guid.Empty == DynamicGuid)
        return;
      passive = templateObj.StateGraph == null;
      if (!passive || GetType() == typeof (DynamicTalkingFSM))
      {
        graphManager = CreateGraphManager();
        eventManager = CreateEventManager();
        functionManager = CreateFunctionManager();
      }
      if (!passive)
      {
        SubscribeToEvents(null);
        if (templateObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_QUEST)
        {
          if (((VMQuest) templateObj).StartEvent == null)
          {
            Logger.AddError(string.Format("Starting event for quest {0} not dfined. Quest won't be started", templateObj.Name));
          }
          else
          {
            Active = false;
            DynamicEvent eventByStaticGuid = VirtualMachine.Instance.GameRootFsm.GetEventByStaticGuid(((VMQuest) templateObj).StartEvent.BaseGuid);
            if (eventByStaticGuid == null)
              Logger.AddError(string.Format("Starting event {0} for quest {1} not found. Quest won't be started", ((VMQuest) templateObj).StartEvent.Name, templateObj.Name));
            OuterStartingEvent = eventByStaticGuid;
          }
        }
        else if (templateObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME)
          Active = true;
        if (templateObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME)
          eventManager.LoadFSMEvent(new DynamicEvent(this, EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking))), false);
        VirtualMachine.Instance.RegisterActiveFSM(this);
      }
      DebugUtility.OnAddObject(this);
    }

    public virtual FSMGraphManager CreateGraphManager() => new FSMGraphManager(this);

    public FSMEventManager CreateEventManager() => new FSMEventManager(this);

    public FSMFunctionManager CreateFunctionManager() => new FSMFunctionManager(this);

    public FSMParamsManager CreateParamsManager() => new FSMParamsManager(this);

    public virtual void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "Active", Active);
      if (!Entity.Instantiated)
        return;
      if (paramsManager != null)
        paramsManager.StateSave(writer);
      if (graphManager != null)
        graphManager.StateSave(writer);
      if (refParams != null)
        SaveSubgraphRefParamsData(writer, "RefParams");
      if (eventManager == null)
        return;
      eventManager.StateSave(writer);
    }

    private void SaveSubgraphRefParamsData(IDataWriter writer, string name)
    {
      writer.Begin(name, null, true);
      for (int index = 0; index < refParams.Count; ++index)
      {
        DynamicParameter refParam = refParams[index];
        if (refParam.StaticGuid != 0UL && !refParam.Entity.IsDisposed)
        {
          writer.Begin("Item", null, true);
          SaveManagerUtility.Save(writer, "ParamStaticGuid", refParam.StaticGuid);
          SaveManagerUtility.Save(writer, "OwnerGuid", refParam.Entity.EngineGuid);
          writer.End("Item", true);
        }
      }
      writer.End(name, true);
    }

    private void LoadSubgraphRefParamsData(XmlElement rootNode)
    {
      if (refParamsData == null && rootNode.ChildNodes.Count > 0)
        refParamsData = new List<KeyValuePair<ulong, Guid>>();
      if (refParamsData != null)
        refParamsData.Clear();
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        refParamsData.Add(new KeyValuePair<ulong, Guid>(VMSaveLoadManager.ReadUlong(firstChild), VMSaveLoadManager.ReadGuid(firstChild.NextSibling)));
      }
    }

    public virtual void LoadFromXML(XmlElement xmlNode)
    {
      if (xmlNode == null)
      {
        Logger.AddError(string.Format("SaveLoad error: null node received for fsm loading in entity", Entity.Name));
      }
      else
      {
        modified = true;
        for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
        {
          XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
          if (childNode.Name == "Active")
            active = VMSaveLoadManager.ReadBool(childNode);
          else if (childNode.Name == "Parameters")
          {
            if (childNode.InnerText != string.Empty)
              LoadParamsManager(childNode);
          }
          else if (childNode.Name == "IsStateValid")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "MainStateStack")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "LocalStateStack")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "LastSubgraphStateStack")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "CurrentStateStackName")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "LockingFSM")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "FlipFlopBranchCurrentIndexesData")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "SubgraphLocalVariablesData")
          {
            if (childNode.InnerText != string.Empty)
              LoadGraphManager(childNode);
          }
          else if (childNode.Name == "RefParams")
            LoadSubgraphRefParamsData(childNode);
          else if (childNode.Name == "ExecutedEvents" && childNode.InnerText != string.Empty)
            LoadEventManager(childNode);
        }
        if (graphManager == null)
          return;
        graphManager.OnSaveLoad();
      }
    }

    private void LoadGraphManager(XmlElement childNode)
    {
      if (graphManager == null)
        graphManager = CreateGraphManager();
      graphManager.LoadFromXML(childNode);
    }

    private void LoadEventManager(XmlElement childNode)
    {
      if (eventManager == null)
        eventManager = CreateEventManager();
      eventManager.LoadFromXML(childNode);
    }

    private void LoadParamsManager(XmlElement childNode)
    {
      if (paramsManager == null)
        paramsManager = CreateParamsManager();
      paramsManager.LoadFromXML(childNode);
    }

    public void Clear()
    {
      if (paramsManager != null)
        paramsManager.Clear();
      if (eventManager != null)
        eventManager.Clear();
      if (functionManager != null)
        functionManager.Clear();
      if (graphManager != null)
        graphManager.Clear();
      if (refParams == null)
        return;
      refParams.Clear();
    }

    public virtual void PreLoading()
    {
      if (eventManager == null)
        return;
      eventManager.PreLoading();
    }

    public virtual void AfterSaveLoading()
    {
      if (paramsManager != null)
        paramsManager.AfterSaveLoading();
      if (eventManager != null)
        eventManager.AfterSaveLoading();
      if (refParamsData != null)
      {
        for (int index = 0; index < refParamsData.Count; ++index)
        {
          KeyValuePair<ulong, Guid> keyValuePair = refParamsData[index];
          ulong key = keyValuePair.Key;
          keyValuePair = refParamsData[index];
          Guid engGuid = keyValuePair.Value;
          VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(engGuid);
          if (entityByEngineGuid != null)
          {
            IParam contextParam = entityByEngineGuid.GetFSM().GetContextParam(key.ToString());
            if (contextParam != null)
            {
              if (typeof (DynamicParameter) == contextParam.GetType())
              {
                if (refParams == null)
                  refParams = new List<DynamicParameter>();
                refParams.Add((DynamicParameter) contextParam);
              }
              else
                Logger.AddError(string.Format("SaveLoad error: strange ref param with id={0} in entity {1}: ref param type is {2}", key, entityByEngineGuid.Name, contextParam.GetType()));
            }
            else
              Logger.AddError(string.Format("SaveLoad error: ref param with id={0} in entity {1} not found", key, entityByEngineGuid.Name));
          }
          else
            Logger.AddError(string.Format("SaveLoad error: ref params owner entity with id={0} not found", engGuid));
        }
      }
      if (graphManager == null)
        return;
      graphManager.AfterSaveLoading();
    }

    public Guid EngineGuid => Entity.EngineGuid;

    public DynamicEvent OuterStartingEvent
    {
      get => outerStartingEvent;
      set
      {
        outerStartingEvent = value;
        if (outerStartingEvent == null)
          return;
        outerStartingEvent.Subscribe(this);
      }
    }

    public EGraphType CurrentFSMGraphType => graphManager.CurrentFSMGraphType;

    public override bool Active
    {
      get => base.Active;
      set
      {
        if (!base.Active & value)
        {
          if (Entity.Instance != null && Entity.Instance.IsDisposed)
            Logger.AddError(string.Format("Object fsm start error: object {0} is dead !!!", FSMStaticObject.Name));
          OnStart();
          if (FSMStaticObject.StateGraph != null)
          {
            string startEventFuncName = FSMStaticObject.GetStartEventFuncName();
            if (startEventFuncName == null)
            {
              Logger.AddError(string.Format("Null start event name at {0} received !!!", FSMStaticObject.Name));
              return;
            }
            if ("" == startEventFuncName)
            {
              Logger.AddError(string.Format("Empty start event name at {0} received !", FSMStaticObject.Name));
              return;
            }
            RaiseEventByName(startEventFuncName);
          }
        }
        else if (base.Active && !value)
          OnStop();
        base.Active = value;
      }
    }

    public IGameMode GameTimeContext => FSMStaticObject.GameTimeContext;

    public void AddRefParam(DynamicParameter param)
    {
      if (refParams == null)
        refParams = new List<DynamicParameter>();
      refParams.Add(param);
    }

    public void RemoveRefParam(DynamicParameter param)
    {
      if (refParams == null || !refParams.Contains(param))
        return;
      refParams.Remove(param);
    }

    public bool PropertyInitialized
    {
      get => initialized;
      set => initialized = value;
    }

    public EEventRaisingMode FsmEventProcessingMode { get; protected set; } = EEventRaisingMode.ERM_ADD_TO_QUEUE;

    public static EEventRaisingMode EventProcessingMode { get; protected set; } = EEventRaisingMode.ERM_ADD_TO_QUEUE;

    public override void Think()
    {
      if (!Entity.Instantiated || !HasActiveEvents || !Active)
        return;
      SetCurrentDebugFSM(this);
      IEvent startEvent = FSMStaticObject.GetStartEvent();
      try
      {
        if (eventManager != null)
        {
          IState currentState = graphManager.CurrentState;
          foreach (DynamicEvent fsmEvent in eventManager.FSMEvents)
          {
            if ((currentState != null || (long) fsmEvent.BaseGuid == (long) startEvent.BaseGuid) && fsmEvent.NeedUpdate())
              fsmEvent.Think();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(ex + "at " + CurrentStateInfo);
        throw;
      }
      SetCurrentDebugFSM(null);
    }

    public void ProcessEvent(RaisedEventInfo eventInfo) => ExecuteEvent(eventInfo);

    public VMLogicObject FSMStaticObject => (VMLogicObject) StaticObject;

    public IParam GetContextParam(ulong stGuid)
    {
      if (paramsManager == null)
        paramsManager = CreateParamsManager();
      return paramsManager.GetContextParam(stGuid);
    }

    public IParam GetContextParam(string paramName)
    {
      if (Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} param accessing at  {1}", FSMStaticObject.Name, CurrentStateInfo));
        return null;
      }
      if (graphManager != null)
      {
        EventMessage contextMessage = graphManager.GetContextMessage(paramName);
        if (contextMessage != null)
          return contextMessage;
      }
      if (paramsManager == null)
        paramsManager = CreateParamsManager();
      return paramsManager.GetContextParam(paramName);
    }

    public DynamicParameter GetDynamicObjectParameter(ulong paramId)
    {
      if (paramsManager == null)
        paramsManager = CreateParamsManager();
      return paramsManager.GetDynamicObjectParameter(paramId);
    }

    public EventMessage GetContextMessage(string messageName)
    {
      return graphManager != null ? graphManager.GetContextMessage(messageName) : null;
    }

    public BaseFunction GetContextFunction(string functionName)
    {
      if (functionManager == null)
        functionManager = CreateFunctionManager();
      return functionManager.GetContextFunction(functionName);
    }

    public DynamicEvent GetContextEvent(string eventName)
    {
      if (Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} event rising at  {1}", FSMStaticObject.Name, CurrentStateInfo));
        return null;
      }
      if (eventManager == null)
        eventManager = CreateEventManager();
      return eventManager.GetContextEvent(eventName);
    }

    public DynamicEvent GetContextEvent(ulong eventId)
    {
      if (Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} event rising at  {1}", FSMStaticObject.Name, CurrentStateInfo));
        return null;
      }
      if (eventManager == null)
        eventManager = CreateEventManager();
      return eventManager.GetContextEvent(eventId);
    }

    public object GetLocalVariableValue(string varName)
    {
      return graphManager != null ? graphManager.GetLocalVariableValue(varName) : null;
    }

    public DynamicEvent GetEventByStaticGuid(ulong stEventGuid)
    {
      try
      {
        DynamicEvent eventByStaticGuid = FindEventByStaticGuid(stEventGuid);
        if (eventByStaticGuid == null && FSMStaticObject.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GAME && typeof (IBlueprint).IsAssignableFrom(FSMStaticObject.GetType()))
        {
          stEventGuid = ((VMBlueprint) FSMStaticObject).GetInheritanceMappedEventGuid(stEventGuid);
          eventByStaticGuid = FindEventByStaticGuid(stEventGuid);
        }
        return eventByStaticGuid;
      }
      catch (Exception ex)
      {
        Logger.AddError(ex + "at " + CurrentStateInfo);
      }
      return null;
    }

    private DynamicEvent FindEventByStaticGuid(ulong stEventGuid)
    {
      if (eventManager == null)
        eventManager = CreateEventManager();
      return eventManager.FindEventByStaticGuid(stEventGuid);
    }

    public bool IsStaticDerived(IBlueprint blueprint)
    {
      return FSMStaticObject.IsDerivedFrom(blueprint.BaseGuid, true);
    }

    public virtual void OnProcessEvent(RaisedEventInfo evntInfo)
    {
      if (evntInfo == null)
        return;
      DynamicEvent instance = evntInfo.Instance;
      if (instance == null)
        return;
      if ("OnTimer" == evntInfo.Instance.Name)
      {
        ulong num = 0;
        if (evntInfo.Messages.Count > 0)
        {
          EventMessage message = evntInfo.Messages[0];
          if (message != null)
            num = (ulong) message.Value;
        }
        Logger.AddWarning(string.Format("Test timer event received, timer id={0}", num));
      }
      if (outerStartingEvent != null && (long) outerStartingEvent.BaseGuid == (long) instance.BaseGuid)
      {
        Active = true;
        outerStartingEvent = null;
      }
      else
      {
        if (!Active || graphManager == null)
          return;
        graphManager.OnProcessEvent(evntInfo);
      }
    }

    public bool IsActualEvent(RaisedEventInfo evntInfo)
    {
      return graphManager != null && graphManager.IsActualEvent(evntInfo);
    }

    public IEnumerable<DynamicParameter> FSMDynamicParams
    {
      get
      {
        if (paramsManager == null)
          paramsManager = CreateParamsManager();
        return paramsManager.FSMDynamicParams;
      }
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnStop()
    {
      if (FSMStaticObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME || refParams == null)
        return;
      for (int index = 0; index < refParams.Count; ++index)
        refParams[index].OnUpdateParam();
    }

    public IState DebugCurrState
    {
      get => graphManager != null ? graphManager.DebugCurrState : null;
    }

    private void SubscribeToEvents(VMState currState)
    {
      if (currState != null)
      {
        graphManager.SubscribeToEvents(currState);
      }
      else
      {
        string startEventFuncName = FSMStaticObject.GetStartEventFuncName();
        if (startEventFuncName == null)
        {
          Logger.AddError(string.Format("Start event not defined in FSM {0}", FSMStaticObject.Name));
        }
        else
        {
          if (eventManager == null)
            eventManager = CreateEventManager();
          DynamicEvent contextEvent = eventManager.GetContextEvent(startEventFuncName);
          if (contextEvent != null)
            contextEvent.Subscribe(this);
          else
            Logger.AddError(string.Format("Start event not found in FSM ", FSMStaticObject.Name));
        }
      }
    }

    public void RaiseEventByName(string eventName, EEventRaisingMode raisingMode = EEventRaisingMode.ERM_ADD_TO_QUEUE)
    {
      if (eventName == null)
      {
        Logger.AddError(string.Format("Event by name raising error at {0}: event name is null !!!", StaticObject.BaseGuid));
      }
      else
      {
        try
        {
          if (eventManager == null)
            eventManager = CreateEventManager();
          DynamicEvent contextEvent = eventManager.GetContextEvent(eventName);
          if (contextEvent != null)
            RaiseEvent(new RaisedEventInfo(contextEvent), raisingMode);
          else
            Logger.AddError(string.Format("Event with name {0} not found in object {1}", eventName, StaticObject.BaseGuid));
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Fsm {0} events accessing error: {1}", FSMStaticObject.Name, ex));
        }
      }
    }

    public void RaiseEvent(RaisedEventInfo evntInfo, EEventRaisingMode raisingMode)
    {
      bool flag = raisingMode == EEventRaisingMode.ERM_ATONCE;
      if (flag && DynamicTalkingFSM.IsTalking && evntInfo.Instance.Name != EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking)))
        flag = false;
      if (flag)
        ExecuteEvent(evntInfo);
      else
        AddEventToFsmQueue(evntInfo);
    }

    private void AddEventToFsmQueue(RaisedEventInfo evntInfo)
    {
      VirtualMachine.Instance.AddFSMToProcessingEvents(evntInfo);
    }

    public void RaiseTimerEvent(GameTimer gameTimer, int iCount = 1)
    {
      string eventName = "";
      if (gameTimer.GTType == EGameTimerType.GAME_TIMER_TYPE_RELATIVE_GLOBAL)
        eventName = EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_ON_GLOBAL_TIMER, typeof (VMGameComponent), true);
      else if (gameTimer.GTType == EGameTimerType.GAME_TIMER_TYPE_RELATIVE_LOCAL)
        eventName = EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_ON_LOCAL_TIMER, typeof (VMGameComponent), true);
      if (eventManager == null)
        eventManager = CreateEventManager();
      DynamicEvent contextEvent = eventManager.GetContextEvent(eventName);
      if (contextEvent != null)
      {
        List<EventMessage> raisingEventMessageList = new List<EventMessage>();
        List<BaseMessage> returnMessages = contextEvent.StaticEvent.ReturnMessages;
        if (returnMessages.Count > 0)
        {
          EventMessage eventMessage = new EventMessage();
          eventMessage.Initialize(returnMessages[0], gameTimer.ResultMessageValue);
          raisingEventMessageList.Add(eventMessage);
        }
        for (int index = 0; index < iCount; ++index)
          contextEvent.Raise(raisingEventMessageList, EEventRaisingMode.ERM_ADD_TO_QUEUE, gameTimer.FSMGuid);
      }
      else
        Logger.AddError(string.Format("Timer event with guid={0} not found in object at {1}", StaticObject.BaseGuid, CurrentStateInfo));
    }

    public bool Lock(DynamicFSM lockingFSM)
    {
      return graphManager != null && graphManager.Lock(lockingFSM);
    }

    public bool UnLock(DynamicFSM lockingFSM)
    {
      return graphManager != null && graphManager.UnLock(lockingFSM);
    }

    public DynamicFSM LockingFSM
    {
      get => graphManager != null ? graphManager.LockingFSM : null;
    }

    public bool NeedSave => !Entity.IsHierarchy || Modified;

    public virtual void OnAddChildDynamicObject(DynamicFSM childDynFSM)
    {
    }

    public virtual void OnRemoveChildDynamicObject(DynamicFSM childDynFSM)
    {
    }

    public virtual void SetLockedObjectNeedRestoreAction(DynamicFSM lockedFSM)
    {
    }

    public void OnProcessActionLine()
    {
      EntityMethodExecuteData methodExecuteData = ExpressionUtility.GetLastActionMethodExecuteData();
      if (methodExecuteData == null)
        return;
      RememberMetodExecData(methodExecuteData);
    }

    protected virtual void RememberMetodExecData(EntityMethodExecuteData lastMethodExecData)
    {
    }

    public static void SetCurrentDebugState(IGraphObject currentState)
    {
      FSMGraphManager.SetCurrentDebugState(currentState);
    }

    public static void SetCurrentDebugFSM(DynamicFSM currentFSM)
    {
      debugThinkingFSM = currentFSM;
    }

    public static string CurrentStateInfo
    {
      get
      {
        if (FSMGraphManager.DebugCurrentState != null)
        {
          string str = "";
          if (FSMGraphManager.DebugCurrentState.Owner != null)
            str += FSMGraphManager.DebugCurrentState.Owner.Name;
          if (FSMGraphManager.DebugCurrentState.Parent != null)
            str = str + "." + FSMGraphManager.DebugCurrentState.Parent.Name;
          return str + "." + FSMGraphManager.DebugCurrentState.Name;
        }
        return debugThinkingFSM != null ? debugThinkingFSM.FSMStaticObject.Name + " think" : "";
      }
    }

    public static DynamicFSM CreateEntityFSM(VMEntity newObjEntity)
    {
      ServiceCache.OptimizationService.FrameHasSpike = true;
      IBlueprint editorTemplate = newObjEntity.EditorTemplate;
      return editorTemplate.GetCategory() != EObjectCategory.OBJECT_CATEGORY_QUEST ? (newObjEntity.GetComponentByName("Speaking") == null ? new DynamicFSM(newObjEntity, (VMLogicObject) editorTemplate) : new DynamicTalkingFSM(newObjEntity, (VMLogicObject) editorTemplate)) : new QuestFSM(newObjEntity, (VMLogicObject) editorTemplate);
    }

    public static void ClearAll()
    {
      FSMGraphManager.ClearAll();
      debugThinkingFSM = null;
    }

    public IState CurrentState
    {
      get => graphManager != null ? graphManager.CurrentState : null;
    }

    private void ExecuteEvent(RaisedEventInfo evntInfo) => evntInfo.Instance.Execute(evntInfo);

    public bool HasActiveEvents => eventManager != null && eventManager.HasActiveEvents;
  }
}
