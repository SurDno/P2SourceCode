// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.LockedFSMInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Base;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class LockedFSMInfo
  {
    public LockedFSMInfo(DynamicFSM lockFSM)
    {
      this.LockedFSM = lockFSM;
      this.LastEntityMethodExecuteData = (EntityMethodExecuteData) null;
      this.NeedRestoreAction = false;
    }

    public void SetLastActionMethodExecData(EntityMethodExecuteData lastActionMethodExecData)
    {
      this.LastEntityMethodExecuteData = lastActionMethodExecData;
      this.NeedRestoreAction = false;
    }

    public bool NeedRestoreAction { get; set; }

    public DynamicFSM LockedFSM { get; private set; }

    public EntityMethodExecuteData LastEntityMethodExecuteData { get; private set; }
  }
}
