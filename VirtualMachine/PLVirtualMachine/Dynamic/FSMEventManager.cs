using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;

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
          LoadEventsFromEngineDirect();
        else
          LoadEvents();
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public void Clear()
    {
      if (fsmEvents != null)
      {
        foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in fsmEvents)
          fsmEvent.Value.ClearSubscribtions();
      }
      if (fsmEvents != null)
        fsmEvents.Clear();
      if (fsmEventsByName != null)
        fsmEventsByName.Clear();
      if (savedExecutedEventsInfo == null)
        return;
      savedExecutedEventsInfo.Clear();
    }

    public void PreLoading()
    {
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in fsmEvents)
        fsmEvent.Value.ClearSubscribtions();
    }

    public IEnumerable<DynamicEvent> FSMEvents
    {
      get
      {
        foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in fsmEvents)
          yield return fsmEvent.Value;
      }
    }

    public DynamicEvent GetContextEvent(string eventName)
    {
      return fsmEventsByName != null && fsmEventsByName.TryGetValue(eventName, out DynamicEvent dynamicEvent) ? dynamicEvent : null;
    }

    public DynamicEvent GetContextEvent(ulong eventId)
    {
      return fsmEvents.TryGetValue(eventId, out DynamicEvent dynamicEvent) ? dynamicEvent : null;
    }

    public DynamicEvent FindEventByStaticGuid(ulong stEventGuid)
    {
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in fsmEvents)
      {
        if ((long) fsmEvent.Value.StaticGuid == (long) stEventGuid)
          return fsmEvent.Value;
      }
      return null;
    }

    public void AfterSaveLoading()
    {
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in fsmEvents)
      {
        DynamicEvent dynamicEvent = fsmEvent.Value;
        bool flag = false;
        if (savedExecutedEventsInfo != null && savedExecutedEventsInfo.Contains(fsmEvent.Key))
          flag = true;
        if (flag)
          dynamicEvent.Active = false;
        else
          dynamicEvent.AfterSaveLoading();
      }
    }

    public void LoadFSMEvent(DynamicEvent dynamicEvent, bool byFuncName)
    {
      fsmEvents[dynamicEvent.BaseGuid] = dynamicEvent;
      if (dynamicEvent.IsManual)
        HasActiveEvents = true;
      else
        fsmEventsByName[byFuncName ? dynamicEvent.FunctionalName : dynamicEvent.Name] = dynamicEvent;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveExecutedEventsInfo(writer, "ExecutedEvents");
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      if (!(xmlNode.Name == "ExecutedEvents"))
        return;
      LoadExecutedEventsInfo(xmlNode);
    }

    public bool HasActiveEvents { get; set; }

    private void LoadEvents()
    {
      int count = fsm.FSMStaticObject.Events.Count;
      int num = 0;
      if (fsm.FSMStaticObject.CustomEvents != null)
        num = fsm.FSMStaticObject.CustomEvents.Count;
      fsmEvents = new Dictionary<ulong, DynamicEvent>(count);
      fsmEventsByName = new Dictionary<string, DynamicEvent>(count - num);
      if (fsm.FSMStaticObject.Events == null)
        return;
      foreach (IEvent staticEvent in fsm.FSMStaticObject.Events)
        LoadFSMEvent(new DynamicEvent(fsm.Entity, staticEvent, fsm), true);
    }

    private void LoadEventsFromEngineDirect()
    {
      if (fsmEvents == null)
        fsmEvents = new Dictionary<ulong, DynamicEvent>();
      else
        fsmEvents.Clear();
      if (fsmEvents == null)
        fsmEventsByName = new Dictionary<string, DynamicEvent>();
      else
        fsmEventsByName.Clear();
      foreach (VMComponent component in fsm.Entity.Components)
      {
        ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
        if (functionalComponentByName == null)
        {
          Logger.AddError(string.Format("Component with name {0} not found in virtual machine api", component.Name));
        }
        else
        {
          for (int index = 0; index < functionalComponentByName.Events.Count; ++index)
          {
            APIEventInfo apiEventInfo = functionalComponentByName.Events[index];
            LoadFSMEvent(new DynamicEvent(fsm.Entity, component, apiEventInfo, fsm), true);
          }
        }
      }
    }

    private void SaveExecutedEventsInfo(IDataWriter writer, string name)
    {
      writer.Begin(name, null, true);
      foreach (KeyValuePair<ulong, DynamicEvent> fsmEvent in fsmEvents)
      {
        DynamicEvent dynamicEvent = fsmEvent.Value;
        if (dynamicEvent.IsManual && !dynamicEvent.Active)
        {
          writer.Begin("Item", null, true);
          SaveManagerUtility.Save(writer, "EventGuid", fsmEvent.Key);
          SaveManagerUtility.Save(writer, "EventName", dynamicEvent.Name);
          writer.End("Item", true);
        }
      }
      writer.End(name, true);
    }

    private void LoadExecutedEventsInfo(XmlElement rootNode)
    {
      if (savedExecutedEventsInfo == null && rootNode.ChildNodes.Count > 0)
        savedExecutedEventsInfo = [];
      if (savedExecutedEventsInfo != null)
        savedExecutedEventsInfo.Clear();
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlNode firstChild = rootNode.ChildNodes[i].FirstChild;
        if (firstChild != null)
          savedExecutedEventsInfo.Add(StringUtility.ToUInt64(firstChild.InnerText));
      }
    }
  }
}
