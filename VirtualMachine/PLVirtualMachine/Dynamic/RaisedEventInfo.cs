// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.RaisedEventInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class RaisedEventInfo : ISerializeStateSave, IDynamicLoadSerializable
  {
    private HashSet<OwnHashInfo> hashHistory = new HashSet<OwnHashInfo>((IEqualityComparer<OwnHashInfo>) OwnHashInfoEqualityComparer.Instance);
    private int historyIteration;
    private DynamicEvent eventInstance;
    private List<EventMessage> messagesList = new List<EventMessage>();
    private Guid sendingFSMGuid;
    public static int EVENTS_CIRCULATION_ITERATIONS_COUNT_MAX = 50;

    public RaisedEventInfo()
    {
    }

    public RaisedEventInfo(DynamicEvent evnt, List<EventMessage> messages, Guid sendingFsmGuid)
    {
      this.eventInstance = evnt;
      for (int index = 0; index < messages.Count; ++index)
      {
        EventMessage eventMessage = new EventMessage();
        eventMessage.Copy(messages[index]);
        this.messagesList.Add(eventMessage);
      }
      this.sendingFSMGuid = sendingFsmGuid;
    }

    public RaisedEventInfo(DynamicEvent evnt)
    {
      this.eventInstance = evnt;
      this.sendingFSMGuid = Guid.Empty;
    }

    public DynamicFSM OwnerFSM
    {
      get => this.eventInstance != null ? this.eventInstance.OwnerFSM : (DynamicFSM) null;
    }

    public DynamicEvent Instance => this.eventInstance;

    public List<EventMessage> Messages => this.messagesList;

    public Guid SendingFSMGuid => this.sendingFSMGuid;

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
        this.hashHistory.Clear();
        foreach (OwnHashInfo ownHashInfo in parentEventInfo.GetHashHistory())
          this.hashHistory.Add(ownHashInfo);
        this.historyIteration = parentEventInfo.GetHistoryIteration();
      }
      if (!this.hashHistory.Add(this.GetOwnHash()) && this.historyIteration > RaisedEventInfo.EVENTS_CIRCULATION_ITERATIONS_COUNT_MAX)
      {
        Logger.AddError(string.Format("Events sequence circulation detected! Event {0} at {1}", (object) this.eventInstance.Name, (object) DynamicFSM.CurrentStateInfo));
        return false;
      }
      ++this.historyIteration;
      return true;
    }

    public HashSet<OwnHashInfo> GetHashHistory() => this.hashHistory;

    public int GetHistoryIteration() => this.historyIteration;

    private OwnHashInfo GetOwnHash()
    {
      if (this.eventInstance == null)
      {
        Logger.AddError(string.Format("Invalid event info raising at {0}!", (object) DynamicFSM.CurrentStateInfo));
        return OwnHashInfo.Empty;
      }
      Guid dynamicGuid = this.eventInstance.OwnerFSM.DynamicGuid;
      ulong num = 0;
      if (this.eventInstance.StaticEvent != null)
        num = this.eventInstance.StaticEvent.BaseGuid;
      long eventId = (long) num;
      Guid sendingFsmGuid = this.sendingFSMGuid;
      return new OwnHashInfo(dynamicGuid, (ulong) eventId, sendingFsmGuid);
    }
  }
}
