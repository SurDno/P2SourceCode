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
using System;
using System.Collections.Generic;
using System.Xml;

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
    protected static DynamicFSM debugThinkingFSM = (DynamicFSM) null;
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
      this.initialized = false;
      this.InitStatic((IObject) templateObj);
      if (Guid.Empty == this.DynamicGuid)
        return;
      this.passive = templateObj.StateGraph == null;
      if (!this.passive || this.GetType() == typeof (DynamicTalkingFSM))
      {
        this.graphManager = this.CreateGraphManager();
        this.eventManager = this.CreateEventManager();
        this.functionManager = this.CreateFunctionManager();
      }
      if (!this.passive)
      {
        this.SubscribeToEvents((VMState) null);
        if (templateObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_QUEST)
        {
          if (((VMQuest) templateObj).StartEvent == null)
          {
            Logger.AddError(string.Format("Starting event for quest {0} not dfined. Quest won't be started", (object) templateObj.Name));
          }
          else
          {
            this.Active = false;
            DynamicEvent eventByStaticGuid = VirtualMachine.Instance.GameRootFsm.GetEventByStaticGuid(((VMQuest) templateObj).StartEvent.BaseGuid);
            if (eventByStaticGuid == null)
              Logger.AddError(string.Format("Starting event {0} for quest {1} not found. Quest won't be started", (object) ((VMQuest) templateObj).StartEvent.Name, (object) templateObj.Name));
            this.OuterStartingEvent = eventByStaticGuid;
          }
        }
        else if (templateObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME)
          this.Active = true;
        if (templateObj.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME)
          this.eventManager.LoadFSMEvent(new DynamicEvent(this, EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking))), false);
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
      SaveManagerUtility.Save(writer, "Active", this.Active);
      if (!this.Entity.Instantiated)
        return;
      if (this.paramsManager != null)
        this.paramsManager.StateSave(writer);
      if (this.graphManager != null)
        this.graphManager.StateSave(writer);
      if (this.refParams != null)
        this.SaveSubgraphRefParamsData(writer, "RefParams");
      if (this.eventManager == null)
        return;
      this.eventManager.StateSave(writer);
    }

    private void SaveSubgraphRefParamsData(IDataWriter writer, string name)
    {
      writer.Begin(name, (Type) null, true);
      for (int index = 0; index < this.refParams.Count; ++index)
      {
        DynamicParameter refParam = this.refParams[index];
        if (refParam.StaticGuid != 0UL && !refParam.Entity.IsDisposed)
        {
          writer.Begin("Item", (Type) null, true);
          SaveManagerUtility.Save(writer, "ParamStaticGuid", refParam.StaticGuid);
          SaveManagerUtility.Save(writer, "OwnerGuid", refParam.Entity.EngineGuid);
          writer.End("Item", true);
        }
      }
      writer.End(name, true);
    }

    private void LoadSubgraphRefParamsData(XmlElement rootNode)
    {
      if (this.refParamsData == null && rootNode.ChildNodes.Count > 0)
        this.refParamsData = new List<KeyValuePair<ulong, Guid>>();
      if (this.refParamsData != null)
        this.refParamsData.Clear();
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        this.refParamsData.Add(new KeyValuePair<ulong, Guid>(VMSaveLoadManager.ReadUlong(firstChild), VMSaveLoadManager.ReadGuid(firstChild.NextSibling)));
      }
    }

    public virtual void LoadFromXML(XmlElement xmlNode)
    {
      if (xmlNode == null)
      {
        Logger.AddError(string.Format("SaveLoad error: null node received for fsm loading in entity", (object) this.Entity.Name));
      }
      else
      {
        this.modified = true;
        for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
        {
          XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
          if (childNode.Name == "Active")
            this.active = VMSaveLoadManager.ReadBool((XmlNode) childNode);
          else if (childNode.Name == "Parameters")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadParamsManager(childNode);
          }
          else if (childNode.Name == "IsStateValid")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "MainStateStack")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "LocalStateStack")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "LastSubgraphStateStack")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "CurrentStateStackName")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "LockingFSM")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "FlipFlopBranchCurrentIndexesData")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "SubgraphLocalVariablesData")
          {
            if (childNode.InnerText != string.Empty)
              this.LoadGraphManager(childNode);
          }
          else if (childNode.Name == "RefParams")
            this.LoadSubgraphRefParamsData(childNode);
          else if (childNode.Name == "ExecutedEvents" && childNode.InnerText != string.Empty)
            this.LoadEventManager(childNode);
        }
        if (this.graphManager == null)
          return;
        this.graphManager.OnSaveLoad();
      }
    }

    private void LoadGraphManager(XmlElement childNode)
    {
      if (this.graphManager == null)
        this.graphManager = this.CreateGraphManager();
      this.graphManager.LoadFromXML(childNode);
    }

    private void LoadEventManager(XmlElement childNode)
    {
      if (this.eventManager == null)
        this.eventManager = this.CreateEventManager();
      this.eventManager.LoadFromXML(childNode);
    }

    private void LoadParamsManager(XmlElement childNode)
    {
      if (this.paramsManager == null)
        this.paramsManager = this.CreateParamsManager();
      this.paramsManager.LoadFromXML(childNode);
    }

    public void Clear()
    {
      if (this.paramsManager != null)
        this.paramsManager.Clear();
      if (this.eventManager != null)
        this.eventManager.Clear();
      if (this.functionManager != null)
        this.functionManager.Clear();
      if (this.graphManager != null)
        this.graphManager.Clear();
      if (this.refParams == null)
        return;
      this.refParams.Clear();
    }

    public virtual void PreLoading()
    {
      if (this.eventManager == null)
        return;
      this.eventManager.PreLoading();
    }

    public virtual void AfterSaveLoading()
    {
      if (this.paramsManager != null)
        this.paramsManager.AfterSaveLoading();
      if (this.eventManager != null)
        this.eventManager.AfterSaveLoading();
      if (this.refParamsData != null)
      {
        for (int index = 0; index < this.refParamsData.Count; ++index)
        {
          KeyValuePair<ulong, Guid> keyValuePair = this.refParamsData[index];
          ulong key = keyValuePair.Key;
          keyValuePair = this.refParamsData[index];
          Guid engGuid = keyValuePair.Value;
          VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(engGuid);
          if (entityByEngineGuid != null)
          {
            IParam contextParam = entityByEngineGuid.GetFSM().GetContextParam(key.ToString());
            if (contextParam != null)
            {
              if (typeof (DynamicParameter) == contextParam.GetType())
              {
                if (this.refParams == null)
                  this.refParams = new List<DynamicParameter>();
                this.refParams.Add((DynamicParameter) contextParam);
              }
              else
                Logger.AddError(string.Format("SaveLoad error: strange ref param with id={0} in entity {1}: ref param type is {2}", (object) key, (object) entityByEngineGuid.Name, (object) contextParam.GetType()));
            }
            else
              Logger.AddError(string.Format("SaveLoad error: ref param with id={0} in entity {1} not found", (object) key, (object) entityByEngineGuid.Name));
          }
          else
            Logger.AddError(string.Format("SaveLoad error: ref params owner entity with id={0} not found", (object) engGuid));
        }
      }
      if (this.graphManager == null)
        return;
      this.graphManager.AfterSaveLoading();
    }

    public Guid EngineGuid => this.Entity.EngineGuid;

    public DynamicEvent OuterStartingEvent
    {
      get => this.outerStartingEvent;
      set
      {
        this.outerStartingEvent = value;
        if (this.outerStartingEvent == null)
          return;
        this.outerStartingEvent.Subscribe(this);
      }
    }

    public EGraphType CurrentFSMGraphType => this.graphManager.CurrentFSMGraphType;

    public override bool Active
    {
      get => base.Active;
      set
      {
        if (!base.Active & value)
        {
          if (this.Entity.Instance != null && this.Entity.Instance.IsDisposed)
            Logger.AddError(string.Format("Object fsm start error: object {0} is dead !!!", (object) this.FSMStaticObject.Name));
          this.OnStart();
          if (this.FSMStaticObject.StateGraph != null)
          {
            string startEventFuncName = this.FSMStaticObject.GetStartEventFuncName();
            if (startEventFuncName == null)
            {
              Logger.AddError(string.Format("Null start event name at {0} received !!!", (object) this.FSMStaticObject.Name));
              return;
            }
            if ("" == startEventFuncName)
            {
              Logger.AddError(string.Format("Empty start event name at {0} received !", (object) this.FSMStaticObject.Name));
              return;
            }
            this.RaiseEventByName(startEventFuncName);
          }
        }
        else if (base.Active && !value)
          this.OnStop();
        base.Active = value;
      }
    }

    public IGameMode GameTimeContext => this.FSMStaticObject.GameTimeContext;

    public void AddRefParam(DynamicParameter param)
    {
      if (this.refParams == null)
        this.refParams = new List<DynamicParameter>();
      this.refParams.Add(param);
    }

    public void RemoveRefParam(DynamicParameter param)
    {
      if (this.refParams == null || !this.refParams.Contains(param))
        return;
      this.refParams.Remove(param);
    }

    public bool PropertyInitialized
    {
      get => this.initialized;
      set => this.initialized = value;
    }

    public EEventRaisingMode FsmEventProcessingMode { get; protected set; } = EEventRaisingMode.ERM_ADD_TO_QUEUE;

    public static EEventRaisingMode EventProcessingMode { get; protected set; } = EEventRaisingMode.ERM_ADD_TO_QUEUE;

    public override void Think()
    {
      if (!this.Entity.Instantiated || !this.HasActiveEvents || !this.Active)
        return;
      DynamicFSM.SetCurrentDebugFSM(this);
      IEvent startEvent = this.FSMStaticObject.GetStartEvent();
      try
      {
        if (this.eventManager != null)
        {
          IState currentState = this.graphManager.CurrentState;
          foreach (DynamicEvent fsmEvent in this.eventManager.FSMEvents)
          {
            if ((currentState != null || (long) fsmEvent.BaseGuid == (long) startEvent.BaseGuid) && fsmEvent.NeedUpdate())
              fsmEvent.Think();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString() + "at " + DynamicFSM.CurrentStateInfo);
        throw;
      }
      DynamicFSM.SetCurrentDebugFSM((DynamicFSM) null);
    }

    public void ProcessEvent(RaisedEventInfo eventInfo) => this.ExecuteEvent(eventInfo);

    public VMLogicObject FSMStaticObject => (VMLogicObject) this.StaticObject;

    public IParam GetContextParam(ulong stGuid)
    {
      if (this.paramsManager == null)
        this.paramsManager = this.CreateParamsManager();
      return this.paramsManager.GetContextParam(stGuid);
    }

    public IParam GetContextParam(string paramName)
    {
      if (this.Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} param accessing at  {1}", (object) this.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        return (IParam) null;
      }
      if (this.graphManager != null)
      {
        EventMessage contextMessage = this.graphManager.GetContextMessage(paramName);
        if (contextMessage != null)
          return (IParam) contextMessage;
      }
      if (this.paramsManager == null)
        this.paramsManager = this.CreateParamsManager();
      return this.paramsManager.GetContextParam(paramName);
    }

    public DynamicParameter GetDynamicObjectParameter(ulong paramId)
    {
      if (this.paramsManager == null)
        this.paramsManager = this.CreateParamsManager();
      return this.paramsManager.GetDynamicObjectParameter(paramId);
    }

    public EventMessage GetContextMessage(string messageName)
    {
      return this.graphManager != null ? this.graphManager.GetContextMessage(messageName) : (EventMessage) null;
    }

    public BaseFunction GetContextFunction(string functionName)
    {
      if (this.functionManager == null)
        this.functionManager = this.CreateFunctionManager();
      return this.functionManager.GetContextFunction(functionName);
    }

    public DynamicEvent GetContextEvent(string eventName)
    {
      if (this.Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} event rising at  {1}", (object) this.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        return (DynamicEvent) null;
      }
      if (this.eventManager == null)
        this.eventManager = this.CreateEventManager();
      return this.eventManager.GetContextEvent(eventName);
    }

    public DynamicEvent GetContextEvent(ulong eventId)
    {
      if (this.Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} event rising at  {1}", (object) this.FSMStaticObject.Name, (object) DynamicFSM.CurrentStateInfo));
        return (DynamicEvent) null;
      }
      if (this.eventManager == null)
        this.eventManager = this.CreateEventManager();
      return this.eventManager.GetContextEvent(eventId);
    }

    public object GetLocalVariableValue(string varName)
    {
      return this.graphManager != null ? this.graphManager.GetLocalVariableValue(varName) : (object) null;
    }

    public DynamicEvent GetEventByStaticGuid(ulong stEventGuid)
    {
      try
      {
        DynamicEvent eventByStaticGuid = this.FindEventByStaticGuid(stEventGuid);
        if (eventByStaticGuid == null && this.FSMStaticObject.GetCategory() != EObjectCategory.OBJECT_CATEGORY_GAME && typeof (IBlueprint).IsAssignableFrom(this.FSMStaticObject.GetType()))
        {
          stEventGuid = ((VMBlueprint) this.FSMStaticObject).GetInheritanceMappedEventGuid(stEventGuid);
          eventByStaticGuid = this.FindEventByStaticGuid(stEventGuid);
        }
        return eventByStaticGuid;
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString() + "at " + DynamicFSM.CurrentStateInfo);
      }
      return (DynamicEvent) null;
    }

    private DynamicEvent FindEventByStaticGuid(ulong stEventGuid)
    {
      if (this.eventManager == null)
        this.eventManager = this.CreateEventManager();
      return this.eventManager.FindEventByStaticGuid(stEventGuid);
    }

    public bool IsStaticDerived(IBlueprint blueprint)
    {
      return this.FSMStaticObject.IsDerivedFrom(blueprint.BaseGuid, true);
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
        Logger.AddWarning(string.Format("Test timer event received, timer id={0}", (object) num));
      }
      if (this.outerStartingEvent != null && (long) this.outerStartingEvent.BaseGuid == (long) instance.BaseGuid)
      {
        this.Active = true;
        this.outerStartingEvent = (DynamicEvent) null;
      }
      else
      {
        if (!this.Active || this.graphManager == null)
          return;
        this.graphManager.OnProcessEvent(evntInfo);
      }
    }

    public bool IsActualEvent(RaisedEventInfo evntInfo)
    {
      return this.graphManager != null && this.graphManager.IsActualEvent(evntInfo);
    }

    public IEnumerable<DynamicParameter> FSMDynamicParams
    {
      get
      {
        if (this.paramsManager == null)
          this.paramsManager = this.CreateParamsManager();
        return this.paramsManager.FSMDynamicParams;
      }
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnStop()
    {
      if (this.FSMStaticObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME || this.refParams == null)
        return;
      for (int index = 0; index < this.refParams.Count; ++index)
        this.refParams[index].OnUpdateParam();
    }

    public IState DebugCurrState
    {
      get => this.graphManager != null ? this.graphManager.DebugCurrState : (IState) null;
    }

    private void SubscribeToEvents(VMState currState)
    {
      if (currState != null)
      {
        this.graphManager.SubscribeToEvents(currState);
      }
      else
      {
        string startEventFuncName = this.FSMStaticObject.GetStartEventFuncName();
        if (startEventFuncName == null)
        {
          Logger.AddError(string.Format("Start event not defined in FSM {0}", (object) this.FSMStaticObject.Name));
        }
        else
        {
          if (this.eventManager == null)
            this.eventManager = this.CreateEventManager();
          DynamicEvent contextEvent = this.eventManager.GetContextEvent(startEventFuncName);
          if (contextEvent != null)
            contextEvent.Subscribe(this);
          else
            Logger.AddError(string.Format("Start event not found in FSM ", (object) this.FSMStaticObject.Name));
        }
      }
    }

    public void RaiseEventByName(string eventName, EEventRaisingMode raisingMode = EEventRaisingMode.ERM_ADD_TO_QUEUE)
    {
      if (eventName == null)
      {
        Logger.AddError(string.Format("Event by name raising error at {0}: event name is null !!!", (object) this.StaticObject.BaseGuid));
      }
      else
      {
        try
        {
          if (this.eventManager == null)
            this.eventManager = this.CreateEventManager();
          DynamicEvent contextEvent = this.eventManager.GetContextEvent(eventName);
          if (contextEvent != null)
            this.RaiseEvent(new RaisedEventInfo(contextEvent), raisingMode);
          else
            Logger.AddError(string.Format("Event with name {0} not found in object {1}", (object) eventName, (object) this.StaticObject.BaseGuid));
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Fsm {0} events accessing error: {1}", (object) this.FSMStaticObject.Name, (object) ex));
        }
      }
    }

    public void RaiseEvent(RaisedEventInfo evntInfo, EEventRaisingMode raisingMode)
    {
      bool flag = raisingMode == EEventRaisingMode.ERM_ATONCE;
      if (flag && DynamicTalkingFSM.IsTalking && evntInfo.Instance.Name != EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_SPEECH_REPLY, typeof (VMSpeaking)))
        flag = false;
      if (flag)
        this.ExecuteEvent(evntInfo);
      else
        this.AddEventToFsmQueue(evntInfo);
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
      if (this.eventManager == null)
        this.eventManager = this.CreateEventManager();
      DynamicEvent contextEvent = this.eventManager.GetContextEvent(eventName);
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
        Logger.AddError(string.Format("Timer event with guid={0} not found in object at {1}", (object) this.StaticObject.BaseGuid, (object) DynamicFSM.CurrentStateInfo));
    }

    public bool Lock(DynamicFSM lockingFSM)
    {
      return this.graphManager != null && this.graphManager.Lock(lockingFSM);
    }

    public bool UnLock(DynamicFSM lockingFSM)
    {
      return this.graphManager != null && this.graphManager.UnLock(lockingFSM);
    }

    public DynamicFSM LockingFSM
    {
      get => this.graphManager != null ? this.graphManager.LockingFSM : (DynamicFSM) null;
    }

    public bool NeedSave => !this.Entity.IsHierarchy || this.Modified;

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
      this.RememberMetodExecData(methodExecuteData);
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
      DynamicFSM.debugThinkingFSM = currentFSM;
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
        return DynamicFSM.debugThinkingFSM != null ? DynamicFSM.debugThinkingFSM.FSMStaticObject.Name + " think" : "";
      }
    }

    public static DynamicFSM CreateEntityFSM(VMEntity newObjEntity)
    {
      ServiceCache.OptimizationService.FrameHasSpike = true;
      IBlueprint editorTemplate = newObjEntity.EditorTemplate;
      return editorTemplate.GetCategory() != EObjectCategory.OBJECT_CATEGORY_QUEST ? (newObjEntity.GetComponentByName("Speaking") == null ? new DynamicFSM(newObjEntity, (VMLogicObject) editorTemplate) : (DynamicFSM) new DynamicTalkingFSM(newObjEntity, (VMLogicObject) editorTemplate)) : (DynamicFSM) new QuestFSM(newObjEntity, (VMLogicObject) editorTemplate);
    }

    public static void ClearAll()
    {
      FSMGraphManager.ClearAll();
      DynamicFSM.debugThinkingFSM = (DynamicFSM) null;
    }

    public IState CurrentState
    {
      get => this.graphManager != null ? this.graphManager.CurrentState : (IState) null;
    }

    private void ExecuteEvent(RaisedEventInfo evntInfo) => evntInfo.Instance.Execute(evntInfo);

    public bool HasActiveEvents => this.eventManager != null && this.eventManager.HasActiveEvents;
  }
}
