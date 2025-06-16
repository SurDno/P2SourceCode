using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Comparers;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Time;

namespace PLVirtualMachine.Dynamic
{
  public class DynamicEvent : 
    DynamicObject,
    IEvent,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ISerializeStateSave,
    IDynamicLoadSerializable
  {
    private DynamicFSM parentFSM;
    private string ownName;
    private bool apiAtOnce;
    private List<BaseMessage> apiEventMessages;
    private bool isManual;
    private Dictionary<Guid, SubscribtionInfo> eventSubscriptions;
    private List<DynamicFSM> currSubscribeFSMList;
    private DynamicEventBody dynamicEventBody;
    private bool isLazyEvent;
    private long prevUpdateTicks;
    private static List<EventMessage> raisingMessages = new List<EventMessage>(MAX_EVENT_MESSAGES_COUNT);
    public static int MAX_EVENT_MESSAGES_COUNT = 10;
    private static float COMPLEX_CONDITION_EVENTS_UPDATE_INTERVAL = 1f;
    private static int SUBSCRIBING_FSM_LIST_DEFALT_SIZE = 20;

    public DynamicEvent(VMEntity entity, IEvent staticEvent, DynamicFSM parentFSM)
      : base(entity)
    {
      try
      {
        ownName = "";
        InitStatic(staticEvent);
        this.parentFSM = parentFSM;
        isManual = IsManual;
        if (!isManual)
        {
          string name = Name;
          Type componentType = ((VMFunctionalComponent) StaticEvent.Parent).ComponentType;
          VMComponent objectComponentByType = EngineAPIManager.GetObjectComponentByType(entity, componentType);
          if (objectComponentByType == null)
            return;
          RegisterEngineEvent(objectComponentByType);
        }
        else
          InitCustomEvent();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Dynamic event {0} creation error: {1}", Name, ex));
      }
    }

    public DynamicEvent(DynamicFSM parentFSM, string name)
      : base(null)
    {
      this.parentFSM = parentFSM;
      ownName = name;
      isManual = IsManual;
      if (!isManual)
        return;
      InitCustomEvent();
    }

    public DynamicEvent(
      VMEntity entity,
      VMComponent eventEngComponent,
      APIEventInfo apiEventInfo,
      DynamicFSM parentFSM)
      : base(entity)
    {
      try
      {
        ownName = apiEventInfo.EventName;
        this.parentFSM = parentFSM;
        isManual = IsManual;
        apiAtOnce = apiEventInfo.AtOnce;
        LoadApiMessages(apiEventInfo);
        RegisterEngineEvent(eventEngComponent);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Dynamic event {0} creation error: {1}", Name, ex));
      }
    }

    private void RegisterEngineEvent(VMComponent eventEngComponent)
    {
      RegisterInComponent(eventEngComponent, this);
      if (parentFSM.FSMStaticObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME || !(eventEngComponent.Name != "Common"))
        return;
      isLazyEvent = true;
    }

    private void InitCustomEvent()
    {
      if (StaticEvent == null)
        return;
      if (StaticEvent.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_CONDITION)
        dynamicEventBody = new DynamicEventBody((VMPartCondition) StaticEvent.Condition, this, GameTimeContext, OnCheckRise, Repeated, Entity);
      else if (StaticEvent.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
      {
        dynamicEventBody = new DynamicEventBody((VMParameter) StaticEvent.EventParameter, this, GameTimeContext, OnCheckRise, Repeated, Entity);
      }
      else
      {
        if (StaticEvent.EventRaisingType != EEventRaisingType.EVENT_RAISING_TYPE_TIME)
          return;
        dynamicEventBody = new DynamicEventBody(StaticEvent.EventTime, this, GameTimeContext, OnCheckRise, Repeated, Entity);
      }
    }

    public IGameMode GameTimeContext
    {
      get
      {
        return StaticEvent.GameTimeContext != null ? StaticEvent.GameTimeContext : parentFSM.GameTimeContext;
      }
    }

    public IEvent StaticEvent => (IEvent) StaticObject;

    public override void Think()
    {
      if (!Active || !isManual || GameTimeContext != null && GameTimeManager.CurrentGameTimeContext != null && GameTimeContext.Name != GameTimeManager.CurrentGameTimeContext.Name || !dynamicEventBody.NeedUpdate())
        return;
      dynamicEventBody.Think();
    }

    public void OnCheckRise(object newConditionValue, EEventRaisingMode raisingMode)
    {
      bool flag = false;
      if (EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_CONDITION)
      {
        if ((bool) newConditionValue == ChangeTo)
          flag = true;
      }
      else if (EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
        flag = (bool) newConditionValue;
      else if (EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_TIME)
        flag = true;
      if (!flag)
        return;
      Raise(new List<EventMessage>(), raisingMode, Guid.Empty);
    }

    public void RaiseFromEngineImpl(params object[] parameters)
    {
      if (DynamicTalkingFSM.IsTalking && StaticEvent == null || isLazyEvent && !HasSubscribtions)
        return;
      List<EventMessage> raisingEventMessageList = new List<EventMessage>();
      for (int index = 0; index < parameters.Length; ++index)
      {
        VMType type = ReturnMessages[index].Type;
        string name = ReturnMessages[index].Name;
        object editorType = VMEngineAPIManager.ConvertEngineTypeToEditorType(parameters[index], type);
        EventMessage eventMessage = new EventMessage();
        eventMessage.Initialize(name, type, editorType);
        raisingEventMessageList.Add(eventMessage);
      }
      EEventRaisingMode raisingMode = EEventRaisingMode.ERM_ADD_TO_QUEUE;
      if (AtOnce && CheckSubcribingFSMConsistensy())
        raisingMode = EEventRaisingMode.ERM_ATONCE;
      Raise(raisingEventMessageList, raisingMode, Guid.Empty);
    }

    public void Raise(
      List<EventMessage> raisingEventMessageList,
      EEventRaisingMode raisingMode,
      Guid fsmGuid)
    {
      if (IsManual)
        OnModify();
      raisingMessages.Clear();
      if (StaticEvent != null)
      {
        if (raisingEventMessageList != null)
        {
          if (StaticEvent.ReturnMessages.Count != raisingEventMessageList.Count)
            Logger.AddError(string.Format("Dynamic event messages count {0} don't match static event messages count: {1}", raisingEventMessageList.Count, StaticEvent.ReturnMessages.Count));
          for (int index = 0; index < StaticEvent.ReturnMessages.Count; ++index)
          {
            if (index < raisingEventMessageList.Count)
            {
              if (!VMTypeUtility.IsTypesCompatible(StaticEvent.ReturnMessages[index].Type, raisingEventMessageList[index].Type))
                Logger.AddError(string.Format("Dynamic event message {0} type {1} is incompatible with static event message {2} type {3}", raisingEventMessageList[index].Name, raisingEventMessageList[index].Type, StaticEvent.ReturnMessages[index].Name, StaticEvent.ReturnMessages[index].Type));
              raisingMessages.Add(raisingEventMessageList[index]);
            }
          }
        }
      }
      else
      {
        for (int index = 0; index < raisingEventMessageList.Count; ++index)
          raisingMessages.Add(raisingEventMessageList[index]);
      }
      parentFSM.RaiseEvent(new RaisedEventInfo(this, raisingMessages, fsmGuid), raisingMode);
    }

    public bool HasSubscribtions
    {
      get => eventSubscriptions != null && eventSubscriptions.Count > 0;
    }

    public void Execute(RaisedEventInfo evntInfo)
    {
      if (!Active)
        return;
      if (currSubscribeFSMList == null)
        currSubscribeFSMList = new List<DynamicFSM>(SUBSCRIBING_FSM_LIST_DEFALT_SIZE);
      else
        currSubscribeFSMList.Clear();
      if (eventSubscriptions != null)
      {
        foreach (KeyValuePair<Guid, SubscribtionInfo> eventSubscription in eventSubscriptions)
          currSubscribeFSMList.Add(eventSubscription.Value.SubscribingFSM);
      }
      for (int index = 0; index < currSubscribeFSMList.Count; ++index)
      {
        if (evntInfo.SendingFSMGuid == Guid.Empty || currSubscribeFSMList[index].DynamicGuid == evntInfo.SendingFSMGuid)
        {
          try
          {
            currSubscribeFSMList[index].OnProcessEvent(evntInfo);
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Error during event {0} subscribtion by fsm {1} processing: {2}", Name, currSubscribeFSMList[index].FSMStaticObject.Name, ex));
          }
        }
      }
      DynamicFSM.SetCurrentDebugState(null);
      if (StaticEvent == null || StaticEvent.Repeated)
        return;
      Active = false;
    }

    public void Subscribe(DynamicFSM subscribeFSM)
    {
      if (eventSubscriptions == null)
        eventSubscriptions = new Dictionary<Guid, SubscribtionInfo>(GuidComparer.Instance);
      if (eventSubscriptions.ContainsKey(subscribeFSM.EngineGuid))
        eventSubscriptions[subscribeFSM.EngineGuid].Add();
      else
        eventSubscriptions.Add(subscribeFSM.EngineGuid, new SubscribtionInfo(subscribeFSM));
    }

    public void DeSubscribe(DynamicFSM subscribeFSM)
    {
      if (eventSubscriptions == null || !eventSubscriptions.ContainsKey(subscribeFSM.EngineGuid))
        return;
      eventSubscriptions[subscribeFSM.EngineGuid].Remove();
      if (eventSubscriptions[subscribeFSM.EngineGuid].Count != 0)
        return;
      eventSubscriptions.Remove(subscribeFSM.EngineGuid);
    }

    public EObjectCategory GetCategory() => StaticEvent.GetCategory();

    public ulong BaseGuid => StaticEvent == null ? 0UL : StaticEvent.BaseGuid;

    public string Name => ownName.Length > 0 ? ownName : StaticEvent.Name;

    public string FunctionalName
    {
      get => ownName.Length > 0 ? ownName : StaticEvent.FunctionalName;
    }

    public IContainer Parent
    {
      get => StaticEvent == null ? null : StaticEvent.Parent;
    }

    public DynamicFSM OwnerFSM => parentFSM;

    public string GuidStr => StaticEvent != null ? StaticEvent.GuidStr : "";

    public bool IsEqual(IObject other)
    {
      return !(typeof (DynamicEvent) != other.GetType()) && (long) ((DynamicObject) other).StaticGuid == (long) StaticGuid && !(((DynamicEvent) other).OwnerFSM.EngineGuid == OwnerFSM.EngineGuid);
    }

    public ICondition Condition
    {
      get => StaticEvent == null ? null : StaticEvent.Condition;
    }

    public IParam EventParameter
    {
      get => StaticEvent == null ? null : StaticEvent.EventParameter;
    }

    public GameTime EventTime
    {
      get => StaticEvent == null ? new GameTime() : StaticEvent.EventTime;
    }

    public bool ChangeTo => StaticEvent != null && StaticEvent.ChangeTo;

    public bool Repeated => StaticEvent != null && StaticEvent.Repeated;

    public bool AtOnce
    {
      get => StaticEvent != null ? ((VMEvent) StaticEvent).AtOnce : apiAtOnce;
    }

    public IContainer Owner
    {
      get => StaticEvent != null ? StaticEvent.Owner : null;
    }

    public bool IsInitial(IObject obj)
    {
      return StaticEvent != null && StaticEvent.IsInitial(obj);
    }

    public bool IsManual => StaticEvent != null && StaticEvent.IsManual;

    public EEventRaisingType EventRaisingType => StaticEvent.EventRaisingType;

    public List<BaseMessage> ReturnMessages
    {
      get
      {
        return StaticObject != null ? ((IEvent) StaticObject).ReturnMessages : apiEventMessages;
      }
    }

    public void Update()
    {
    }

    public bool IsUpdated => true;

    public void ClearSubscribtions()
    {
      if (eventSubscriptions != null)
        eventSubscriptions.Clear();
      if (currSubscribeFSMList != null)
        currSubscribeFSMList.Clear();
      if (dynamicEventBody == null)
        return;
      dynamicEventBody.ClearSubscribtions();
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticId", StaticGuid);
      SaveManagerUtility.Save(writer, "Active", Active);
      SaveManagerUtility.SaveDynamicSerializable(writer, "EventBody", dynamicEventBody);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      XmlElement xmlNode1 = null;
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "Active")
          Active = VMSaveLoadManager.ReadBool(xmlNode.ChildNodes[i]);
        else if (xmlNode.ChildNodes[i].Name == "EventBody")
          xmlNode1 = (XmlElement) xmlNode.ChildNodes[i];
      }
      if (dynamicEventBody == null || xmlNode1 == null)
        return;
      dynamicEventBody.LoadFromXML(xmlNode1);
    }

    public void AfterSaveLoading()
    {
      if (dynamicEventBody == null)
        return;
      dynamicEventBody.AfterSaveLoading();
    }

    public bool NeedUpdate()
    {
      if (dynamicEventBody == null)
        return false;
      long timestamp = Stopwatch.GetTimestamp();
      if ((timestamp - prevUpdateTicks) / (double) Stopwatch.Frequency <= COMPLEX_CONDITION_EVENTS_UPDATE_INTERVAL)
        return false;
      prevUpdateTicks = timestamp;
      return dynamicEventBody.NeedUpdate();
    }

    public void Clear()
    {
    }

    private void LoadApiMessages(APIEventInfo apiEventInfo)
    {
      if (apiEventMessages != null)
        apiEventMessages.Clear();
      if (apiEventInfo == null)
      {
        Logger.AddError(string.Format("Cannot load messages for event {0}: api event info not defined", Name));
      }
      else
      {
        if (apiEventInfo.MessageParams.Count <= 0)
          return;
        if (apiEventMessages == null)
          apiEventMessages = new List<BaseMessage>();
        for (int index = 0; index < apiEventInfo.MessageParams.Count; ++index)
        {
          VMType type = apiEventInfo.MessageParams[index].Type;
          string name = Name + "_message_" + apiEventInfo.MessageParams[index].Name;
          BaseMessage baseMessage = new BaseMessage();
          baseMessage.Initialize(name, type);
          apiEventMessages.Add(baseMessage);
        }
      }
    }

    private bool CheckSubcribingFSMConsistensy()
    {
      if (currSubscribeFSMList != null)
      {
        for (int index = 0; index < currSubscribeFSMList.Count; ++index)
        {
          if (currSubscribeFSMList[index].CurrentFSMGraphType == EGraphType.GRAPH_TYPE_PROCEDURE)
            return false;
        }
      }
      return true;
    }

    private static void RegisterInComponent(VMComponent component, DynamicEvent target)
    {
      if (component is IInitialiseEvents initialiseEvents)
        initialiseEvents.InitialiseEvent(target);
      else
        Logger.AddError("Event not found, need generate code, component : " + component.Name + " , event name : " + target.Name);
    }

    public static void StaticClear() => raisingMessages.Clear();
  }
}
