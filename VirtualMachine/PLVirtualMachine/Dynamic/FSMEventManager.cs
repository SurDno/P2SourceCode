using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using System;
using System.Collections.Generic;
using System.Xml;

namespace PLVirtualMachine.Dynamic
{
  public class FSMEventManager
  {
    private DynamicFSM fsm;
    private Dictionary<ulong, DynamicEvent> fsmEvents;
    private Dictionary<string, DynamicEvent> fsmEventsByName;
    private HashSet<ulong> savedExecutedEventsInfo;

    public FSMEventManager(DynamicFSM fsm)
    {
      this.fsm = fsm;
      try
      {
        if (fsm.FSMStaticObject.DirectEngineCreated)
          this.LoadEventsFromEngineDirect();
        else
          this.LoadEvents();
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public void Clear()
    {
      if (this.fsmEvents != null)
      {
        foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in this.fsmEvents)
          fsmEvent.Value.ClearSubscribtions();
      }
      if (this.fsmEvents != null)
        this.fsmEvents.Clear();
      if (this.fsmEventsByName != null)
        this.fsmEventsByName.Clear();
      if (this.savedExecutedEventsInfo == null)
        return;
      this.savedExecutedEventsInfo.Clear();
    }

    public void PreLoading()
    {
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in this.fsmEvents)
        fsmEvent.Value.ClearSubscribtions();
    }

    public IEnumerable<DynamicEvent> FSMEvents
    {
      get
      {
        foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in this.fsmEvents)
          yield return fsmEvent.Value;
      }
    }

    public DynamicEvent GetContextEvent(string eventName)
    {
      DynamicEvent dynamicEvent;
      return this.fsmEventsByName != null && this.fsmEventsByName.TryGetValue(eventName, out dynamicEvent) ? dynamicEvent : (DynamicEvent) null;
    }

    public DynamicEvent GetContextEvent(ulong eventId)
    {
      DynamicEvent dynamicEvent;
      return this.fsmEvents.TryGetValue(eventId, out dynamicEvent) ? dynamicEvent : (DynamicEvent) null;
    }

    public DynamicEvent FindEventByStaticGuid(ulong stEventGuid)
    {
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in this.fsmEvents)
      {
        if ((long) fsmEvent.Value.StaticGuid == (long) stEventGuid)
          return fsmEvent.Value;
      }
      return (DynamicEvent) null;
    }

    public void AfterSaveLoading()
    {
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in this.fsmEvents)
      {
        DynamicEvent dynamicEvent = fsmEvent.Value;
        bool flag = false;
        if (this.savedExecutedEventsInfo != null && this.savedExecutedEventsInfo.Contains(fsmEvent.Key))
          flag = true;
        if (flag)
          dynamicEvent.Active = false;
        else
          dynamicEvent.AfterSaveLoading();
      }
    }

    public void LoadFSMEvent(DynamicEvent dynamicEvent, bool byFuncName)
    {
      this.fsmEvents[dynamicEvent.BaseGuid] = dynamicEvent;
      if (dynamicEvent.IsManual)
        this.HasActiveEvents = true;
      else
        this.fsmEventsByName[byFuncName ? dynamicEvent.FunctionalName : dynamicEvent.Name] = dynamicEvent;
    }

    public void StateSave(IDataWriter writer)
    {
      this.SaveExecutedEventsInfo(writer, "ExecutedEvents");
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      if (!(xmlNode.Name == "ExecutedEvents"))
        return;
      this.LoadExecutedEventsInfo(xmlNode);
    }

    public bool HasActiveEvents { get; set; }

    private void LoadEvents()
    {
      int count = this.fsm.FSMStaticObject.Events.Count;
      int num = 0;
      if (this.fsm.FSMStaticObject.CustomEvents != null)
        num = this.fsm.FSMStaticObject.CustomEvents.Count;
      this.fsmEvents = new Dictionary<ulong, DynamicEvent>(count);
      this.fsmEventsByName = new Dictionary<string, DynamicEvent>(count - num);
      if (this.fsm.FSMStaticObject.Events == null)
        return;
      foreach (IEvent staticEvent in this.fsm.FSMStaticObject.Events)
        this.LoadFSMEvent(new DynamicEvent(this.fsm.Entity, staticEvent, this.fsm), true);
    }

    private void LoadEventsFromEngineDirect()
    {
      if (this.fsmEvents == null)
        this.fsmEvents = new Dictionary<ulong, DynamicEvent>();
      else
        this.fsmEvents.Clear();
      if (this.fsmEvents == null)
        this.fsmEventsByName = new Dictionary<string, DynamicEvent>();
      else
        this.fsmEventsByName.Clear();
      foreach (VMComponent component in this.fsm.Entity.Components)
      {
        ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
        if (functionalComponentByName == null)
        {
          Logger.AddError(string.Format("Component with name {0} not found in virtual machine api", (object) component.Name));
        }
        else
        {
          for (int index = 0; index < functionalComponentByName.Events.Count; ++index)
          {
            APIEventInfo apiEventInfo = functionalComponentByName.Events[index];
            this.LoadFSMEvent(new DynamicEvent(this.fsm.Entity, component, apiEventInfo, this.fsm), true);
          }
        }
      }
    }

    private void SaveExecutedEventsInfo(IDataWriter writer, string name)
    {
      writer.Begin(name, (Type) null, true);
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in this.fsmEvents)
      {
        DynamicEvent dynamicEvent = fsmEvent.Value;
        if (dynamicEvent.IsManual && !dynamicEvent.Active)
        {
          writer.Begin("Item", (Type) null, true);
          SaveManagerUtility.Save(writer, "EventGuid", fsmEvent.Key);
          SaveManagerUtility.Save(writer, "EventName", dynamicEvent.Name);
          writer.End("Item", true);
        }
      }
      writer.End(name, true);
    }

    private void LoadExecutedEventsInfo(XmlElement rootNode)
    {
      if (this.savedExecutedEventsInfo == null && rootNode.ChildNodes.Count > 0)
        this.savedExecutedEventsInfo = new HashSet<ulong>();
      if (this.savedExecutedEventsInfo != null)
        this.savedExecutedEventsInfo.Clear();
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        if (firstChild != null)
          this.savedExecutedEventsInfo.Add(StringUtility.ToUInt64(firstChild.InnerText));
      }
    }
  }
}
