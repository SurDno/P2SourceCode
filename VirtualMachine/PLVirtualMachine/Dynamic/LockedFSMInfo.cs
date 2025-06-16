using PLVirtualMachine.Base;

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
