using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Dynamic
{
  public class RaisedEventInfo : ISerializeStateSave, IDynamicLoadSerializable
  {
    private HashSet<OwnHashInfo> hashHistory = new(OwnHashInfoEqualityComparer.Instance);
    private int historyIteration;
    private DynamicEvent eventInstance;
    private List<EventMessage> messagesList = [];
    private Guid sendingFSMGuid;
    public static int EVENTS_CIRCULATION_ITERATIONS_COUNT_MAX = 50;

    public RaisedEventInfo()
    {
    }

    public RaisedEventInfo(DynamicEvent evnt, List<EventMessage> messages, Guid sendingFsmGuid)
    {
      eventInstance = evnt;
      for (int index = 0; index < messages.Count; ++index)
      {
        EventMessage eventMessage = new EventMessage();
        eventMessage.Copy(messages[index]);
        messagesList.Add(eventMessage);
      }
      sendingFSMGuid = sendingFsmGuid;
    }

    public RaisedEventInfo(DynamicEvent evnt)
    {
      eventInstance = evnt;
      sendingFSMGuid = Guid.Empty;
    }

    public DynamicFSM OwnerFSM => eventInstance != null ? eventInstance.OwnerFSM : null;

    public DynamicEvent Instance => eventInstance;

    public List<EventMessage> Messages => messagesList;

    public Guid SendingFSMGuid => sendingFSMGuid;

    public void StateSave(IDataWriter writer)
    {
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
    }

    public bool MakeHashHistory(RaisedEventInfo parentEventInfo)
    {
      if (parentEventInfo != null)
      {
        hashHistory.Clear();
        foreach (OwnHashInfo ownHashInfo in parentEventInfo.GetHashHistory())
          hashHistory.Add(ownHashInfo);
        historyIteration = parentEventInfo.GetHistoryIteration();
      }
      if (!hashHistory.Add(GetOwnHash()) && historyIteration > EVENTS_CIRCULATION_ITERATIONS_COUNT_MAX)
      {
        Logger.AddError(string.Format("Events sequence circulation detected! Event {0} at {1}", eventInstance.Name, DynamicFSM.CurrentStateInfo));
        return false;
      }
      ++historyIteration;
      return true;
    }

    public HashSet<OwnHashInfo> GetHashHistory() => hashHistory;

    public int GetHistoryIteration() => historyIteration;

    private OwnHashInfo GetOwnHash()
    {
      if (eventInstance == null)
      {
        Logger.AddError(string.Format("Invalid event info raising at {0}!", DynamicFSM.CurrentStateInfo));
        return OwnHashInfo.Empty;
      }
      Guid dynamicGuid = eventInstance.OwnerFSM.DynamicGuid;
      ulong num = 0;
      if (eventInstance.StaticEvent != null)
        num = eventInstance.StaticEvent.BaseGuid;
      long eventId = (long) num;
      Guid sendingFsmGuid = sendingFSMGuid;
      return new OwnHashInfo(dynamicGuid, (ulong) eventId, sendingFsmGuid);
    }
  }
}
