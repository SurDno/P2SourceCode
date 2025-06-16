// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicEvent
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

#nullable disable
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
    private static List<EventMessage> raisingMessages = new List<EventMessage>(DynamicEvent.MAX_EVENT_MESSAGES_COUNT);
    public static int MAX_EVENT_MESSAGES_COUNT = 10;
    private static float COMPLEX_CONDITION_EVENTS_UPDATE_INTERVAL = 1f;
    private static int SUBSCRIBING_FSM_LIST_DEFALT_SIZE = 20;

    public DynamicEvent(VMEntity entity, IEvent staticEvent, DynamicFSM parentFSM)
      : base(entity)
    {
      try
      {
        this.ownName = "";
        this.InitStatic((IObject) staticEvent);
        this.parentFSM = parentFSM;
        this.isManual = this.IsManual;
        if (!this.isManual)
        {
          string name = this.Name;
          Type componentType = ((VMFunctionalComponent) this.StaticEvent.Parent).ComponentType;
          VMComponent objectComponentByType = EngineAPIManager.GetObjectComponentByType((VMBaseEntity) entity, componentType);
          if (objectComponentByType == null)
            return;
          this.RegisterEngineEvent(objectComponentByType);
        }
        else
          this.InitCustomEvent();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Dynamic event {0} creation error: {1}", (object) this.Name, (object) ex));
      }
    }

    public DynamicEvent(DynamicFSM parentFSM, string name)
      : base((VMEntity) null)
    {
      this.parentFSM = parentFSM;
      this.ownName = name;
      this.isManual = this.IsManual;
      if (!this.isManual)
        return;
      this.InitCustomEvent();
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
        this.ownName = apiEventInfo.EventName;
        this.parentFSM = parentFSM;
        this.isManual = this.IsManual;
        this.apiAtOnce = apiEventInfo.AtOnce;
        this.LoadApiMessages(apiEventInfo);
        this.RegisterEngineEvent(eventEngComponent);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Dynamic event {0} creation error: {1}", (object) this.Name, (object) ex));
      }
    }

    private void RegisterEngineEvent(VMComponent eventEngComponent)
    {
      DynamicEvent.RegisterInComponent(eventEngComponent, this);
      if (this.parentFSM.FSMStaticObject.GetCategory() == EObjectCategory.OBJECT_CATEGORY_GAME || !(eventEngComponent.Name != "Common"))
        return;
      this.isLazyEvent = true;
    }

    private void InitCustomEvent()
    {
      if (this.StaticEvent == null)
        return;
      if (this.StaticEvent.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_CONDITION)
        this.dynamicEventBody = new DynamicEventBody((VMPartCondition) this.StaticEvent.Condition, (INamed) this, this.GameTimeContext, new DynamicEventBody.OnEventBodyRise(this.OnCheckRise), this.Repeated, this.Entity);
      else if (this.StaticEvent.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
      {
        this.dynamicEventBody = new DynamicEventBody((VMParameter) this.StaticEvent.EventParameter, (INamed) this, this.GameTimeContext, new DynamicEventBody.OnEventBodyRise(this.OnCheckRise), this.Repeated, this.Entity);
      }
      else
      {
        if (this.StaticEvent.EventRaisingType != EEventRaisingType.EVENT_RAISING_TYPE_TIME)
          return;
        this.dynamicEventBody = new DynamicEventBody(this.StaticEvent.EventTime, (INamed) this, this.GameTimeContext, new DynamicEventBody.OnEventBodyRise(this.OnCheckRise), this.Repeated, this.Entity);
      }
    }

    public IGameMode GameTimeContext
    {
      get
      {
        return this.StaticEvent.GameTimeContext != null ? this.StaticEvent.GameTimeContext : this.parentFSM.GameTimeContext;
      }
    }

    public IEvent StaticEvent => (IEvent) this.StaticObject;

    public override void Think()
    {
      if (!this.Active || !this.isManual || this.GameTimeContext != null && GameTimeManager.CurrentGameTimeContext != null && this.GameTimeContext.Name != GameTimeManager.CurrentGameTimeContext.Name || !this.dynamicEventBody.NeedUpdate())
        return;
      this.dynamicEventBody.Think();
    }

    public void OnCheckRise(object newConditionValue, EEventRaisingMode raisingMode)
    {
      bool flag = false;
      if (this.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_CONDITION)
      {
        if ((bool) newConditionValue == this.ChangeTo)
          flag = true;
      }
      else if (this.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_PARAM_CHANGE)
        flag = (bool) newConditionValue;
      else if (this.EventRaisingType == EEventRaisingType.EVENT_RAISING_TYPE_TIME)
        flag = true;
      if (!flag)
        return;
      this.Raise(new List<EventMessage>(), raisingMode, Guid.Empty);
    }

    public void RaiseFromEngineImpl(params object[] parameters)
    {
      if (DynamicTalkingFSM.IsTalking && this.StaticEvent == null || this.isLazyEvent && !this.HasSubscribtions)
        return;
      List<EventMessage> raisingEventMessageList = new List<EventMessage>();
      for (int index = 0; index < parameters.Length; ++index)
      {
        VMType type = this.ReturnMessages[index].Type;
        string name = this.ReturnMessages[index].Name;
        object editorType = VMEngineAPIManager.ConvertEngineTypeToEditorType(parameters[index], type);
        EventMessage eventMessage = new EventMessage();
        eventMessage.Initialize(name, type, editorType);
        raisingEventMessageList.Add(eventMessage);
      }
      EEventRaisingMode raisingMode = EEventRaisingMode.ERM_ADD_TO_QUEUE;
      if (this.AtOnce && this.CheckSubcribingFSMConsistensy())
        raisingMode = EEventRaisingMode.ERM_ATONCE;
      this.Raise(raisingEventMessageList, raisingMode, Guid.Empty);
    }

    public void Raise(
      List<EventMessage> raisingEventMessageList,
      EEventRaisingMode raisingMode,
      Guid fsmGuid)
    {
      if (this.IsManual)
        this.OnModify();
      DynamicEvent.raisingMessages.Clear();
      if (this.StaticEvent != null)
      {
        if (raisingEventMessageList != null)
        {
          if (this.StaticEvent.ReturnMessages.Count != raisingEventMessageList.Count)
            Logger.AddError(string.Format("Dynamic event messages count {0} don't match static event messages count: {1}", (object) raisingEventMessageList.Count, (object) this.StaticEvent.ReturnMessages.Count));
          for (int index = 0; index < this.StaticEvent.ReturnMessages.Count; ++index)
          {
            if (index < raisingEventMessageList.Count)
            {
              if (!VMTypeUtility.IsTypesCompatible(this.StaticEvent.ReturnMessages[index].Type, raisingEventMessageList[index].Type))
                Logger.AddError(string.Format("Dynamic event message {0} type {1} is incompatible with static event message {2} type {3}", (object) raisingEventMessageList[index].Name, (object) raisingEventMessageList[index].Type, (object) this.StaticEvent.ReturnMessages[index].Name, (object) this.StaticEvent.ReturnMessages[index].Type));
              DynamicEvent.raisingMessages.Add(raisingEventMessageList[index]);
            }
          }
        }
      }
      else
      {
        for (int index = 0; index < raisingEventMessageList.Count; ++index)
          DynamicEvent.raisingMessages.Add(raisingEventMessageList[index]);
      }
      this.parentFSM.RaiseEvent(new RaisedEventInfo(this, DynamicEvent.raisingMessages, fsmGuid), raisingMode);
    }

    public bool HasSubscribtions
    {
      get => this.eventSubscriptions != null && this.eventSubscriptions.Count > 0;
    }

    public void Execute(RaisedEventInfo evntInfo)
    {
      if (!this.Active)
        return;
      if (this.currSubscribeFSMList == null)
        this.currSubscribeFSMList = new List<DynamicFSM>(DynamicEvent.SUBSCRIBING_FSM_LIST_DEFALT_SIZE);
      else
        this.currSubscribeFSMList.Clear();
      if (this.eventSubscriptions != null)
      {
        foreach (KeyValuePair<Guid, SubscribtionInfo> eventSubscription in this.eventSubscriptions)
          this.currSubscribeFSMList.Add(eventSubscription.Value.SubscribingFSM);
      }
      for (int index = 0; index < this.currSubscribeFSMList.Count; ++index)
      {
        if (evntInfo.SendingFSMGuid == Guid.Empty || this.currSubscribeFSMList[index].DynamicGuid == evntInfo.SendingFSMGuid)
        {
          try
          {
            this.currSubscribeFSMList[index].OnProcessEvent(evntInfo);
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Error during event {0} subscribtion by fsm {1} processing: {2}", (object) this.Name, (object) this.currSubscribeFSMList[index].FSMStaticObject.Name, (object) ex.ToString()));
          }
        }
      }
      DynamicFSM.SetCurrentDebugState((IGraphObject) null);
      if (this.StaticEvent == null || this.StaticEvent.Repeated)
        return;
      this.Active = false;
    }

    public void Subscribe(DynamicFSM subscribeFSM)
    {
      if (this.eventSubscriptions == null)
        this.eventSubscriptions = new Dictionary<Guid, SubscribtionInfo>((IEqualityComparer<Guid>) GuidComparer.Instance);
      if (this.eventSubscriptions.ContainsKey(subscribeFSM.EngineGuid))
        this.eventSubscriptions[subscribeFSM.EngineGuid].Add();
      else
        this.eventSubscriptions.Add(subscribeFSM.EngineGuid, new SubscribtionInfo(subscribeFSM));
    }

    public void DeSubscribe(DynamicFSM subscribeFSM)
    {
      if (this.eventSubscriptions == null || !this.eventSubscriptions.ContainsKey(subscribeFSM.EngineGuid))
        return;
      this.eventSubscriptions[subscribeFSM.EngineGuid].Remove();
      if (this.eventSubscriptions[subscribeFSM.EngineGuid].Count != 0)
        return;
      this.eventSubscriptions.Remove(subscribeFSM.EngineGuid);
    }

    public EObjectCategory GetCategory() => this.StaticEvent.GetCategory();

    public ulong BaseGuid => this.StaticEvent == null ? 0UL : this.StaticEvent.BaseGuid;

    public string Name => this.ownName.Length > 0 ? this.ownName : this.StaticEvent.Name;

    public string FunctionalName
    {
      get => this.ownName.Length > 0 ? this.ownName : this.StaticEvent.FunctionalName;
    }

    public IContainer Parent
    {
      get => this.StaticEvent == null ? (IContainer) null : this.StaticEvent.Parent;
    }

    public DynamicFSM OwnerFSM => this.parentFSM;

    public string GuidStr => this.StaticEvent != null ? this.StaticEvent.GuidStr : "";

    public bool IsEqual(IObject other)
    {
      return !(typeof (DynamicEvent) != other.GetType()) && (long) ((DynamicObject) other).StaticGuid == (long) this.StaticGuid && !(((DynamicEvent) other).OwnerFSM.EngineGuid == this.OwnerFSM.EngineGuid);
    }

    public ICondition Condition
    {
      get => this.StaticEvent == null ? (ICondition) null : this.StaticEvent.Condition;
    }

    public IParam EventParameter
    {
      get => this.StaticEvent == null ? (IParam) null : this.StaticEvent.EventParameter;
    }

    public GameTime EventTime
    {
      get => this.StaticEvent == null ? new GameTime() : this.StaticEvent.EventTime;
    }

    public bool ChangeTo => this.StaticEvent != null && this.StaticEvent.ChangeTo;

    public bool Repeated => this.StaticEvent != null && this.StaticEvent.Repeated;

    public bool AtOnce
    {
      get => this.StaticEvent != null ? ((VMEvent) this.StaticEvent).AtOnce : this.apiAtOnce;
    }

    public IContainer Owner
    {
      get => this.StaticEvent != null ? this.StaticEvent.Owner : (IContainer) null;
    }

    public bool IsInitial(IObject obj)
    {
      return this.StaticEvent != null && this.StaticEvent.IsInitial(obj);
    }

    public bool IsManual => this.StaticEvent != null && this.StaticEvent.IsManual;

    public EEventRaisingType EventRaisingType => this.StaticEvent.EventRaisingType;

    public List<BaseMessage> ReturnMessages
    {
      get
      {
        return this.StaticObject != null ? ((IEvent) this.StaticObject).ReturnMessages : this.apiEventMessages;
      }
    }

    public void Update()
    {
    }

    public bool IsUpdated => true;

    public void ClearSubscribtions()
    {
      if (this.eventSubscriptions != null)
        this.eventSubscriptions.Clear();
      if (this.currSubscribeFSMList != null)
        this.currSubscribeFSMList.Clear();
      if (this.dynamicEventBody == null)
        return;
      this.dynamicEventBody.ClearSubscribtions();
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticId", this.StaticGuid);
      SaveManagerUtility.Save(writer, "Active", this.Active);
      SaveManagerUtility.SaveDynamicSerializable(writer, "EventBody", (ISerializeStateSave) this.dynamicEventBody);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      XmlElement xmlNode1 = (XmlElement) null;
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "Active")
          this.Active = VMSaveLoadManager.ReadBool(xmlNode.ChildNodes[i]);
        else if (xmlNode.ChildNodes[i].Name == "EventBody")
          xmlNode1 = (XmlElement) xmlNode.ChildNodes[i];
      }
      if (this.dynamicEventBody == null || xmlNode1 == null)
        return;
      this.dynamicEventBody.LoadFromXML(xmlNode1);
    }

    public void AfterSaveLoading()
    {
      if (this.dynamicEventBody == null)
        return;
      this.dynamicEventBody.AfterSaveLoading();
    }

    public bool NeedUpdate()
    {
      if (this.dynamicEventBody == null)
        return false;
      long timestamp = Stopwatch.GetTimestamp();
      if ((double) (timestamp - this.prevUpdateTicks) / (double) Stopwatch.Frequency <= (double) DynamicEvent.COMPLEX_CONDITION_EVENTS_UPDATE_INTERVAL)
        return false;
      this.prevUpdateTicks = timestamp;
      return this.dynamicEventBody.NeedUpdate();
    }

    public void Clear()
    {
    }

    private void LoadApiMessages(APIEventInfo apiEventInfo)
    {
      if (this.apiEventMessages != null)
        this.apiEventMessages.Clear();
      if (apiEventInfo == null)
      {
        Logger.AddError(string.Format("Cannot load messages for event {0}: api event info not defined", (object) this.Name));
      }
      else
      {
        if (apiEventInfo.MessageParams.Count <= 0)
          return;
        if (this.apiEventMessages == null)
          this.apiEventMessages = new List<BaseMessage>();
        for (int index = 0; index < apiEventInfo.MessageParams.Count; ++index)
        {
          VMType type = apiEventInfo.MessageParams[index].Type;
          string name = this.Name + "_message_" + apiEventInfo.MessageParams[index].Name;
          BaseMessage baseMessage = new BaseMessage();
          baseMessage.Initialize(name, type);
          this.apiEventMessages.Add(baseMessage);
        }
      }
    }

    private bool CheckSubcribingFSMConsistensy()
    {
      if (this.currSubscribeFSMList != null)
      {
        for (int index = 0; index < this.currSubscribeFSMList.Count; ++index)
        {
          if (this.currSubscribeFSMList[index].CurrentFSMGraphType == EGraphType.GRAPH_TYPE_PROCEDURE)
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

    public static void StaticClear() => DynamicEvent.raisingMessages.Clear();
  }
}
