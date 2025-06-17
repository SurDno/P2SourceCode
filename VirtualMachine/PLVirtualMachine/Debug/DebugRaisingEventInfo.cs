namespace PLVirtualMachine.Debug
{
  public class DebugRaisingEventInfo(string eventOwnerUniName, ulong eventGuid) {
    public string EventOwnerUniName => eventOwnerUniName;

    public ulong EventGuid => eventGuid;
  }
}
