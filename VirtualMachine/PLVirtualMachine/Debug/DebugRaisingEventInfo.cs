namespace PLVirtualMachine.Debug
{
  public class DebugRaisingEventInfo
  {
    private string eventOwnerUniName;
    private ulong eventGuid;

    public DebugRaisingEventInfo(string eventOwnerUniName, ulong eventGuid)
    {
      this.eventOwnerUniName = eventOwnerUniName;
      this.eventGuid = eventGuid;
    }

    public string EventOwnerUniName => this.eventOwnerUniName;

    public ulong EventGuid => this.eventGuid;
  }
}
