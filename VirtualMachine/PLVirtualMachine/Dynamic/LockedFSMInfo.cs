using PLVirtualMachine.Base;

namespace PLVirtualMachine.Dynamic
{
  public class LockedFSMInfo(DynamicFSM lockFsm) {
    public void SetLastActionMethodExecData(EntityMethodExecuteData lastActionMethodExecData)
    {
      LastEntityMethodExecuteData = lastActionMethodExecData;
      NeedRestoreAction = false;
    }

    public bool NeedRestoreAction { get; set; } = false;

    public DynamicFSM LockedFSM { get; private set; } = lockFsm;

    public EntityMethodExecuteData LastEntityMethodExecuteData { get; private set; } = null;
  }
}
