// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Debug.DebugRaisingEventInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

#nullable disable
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
