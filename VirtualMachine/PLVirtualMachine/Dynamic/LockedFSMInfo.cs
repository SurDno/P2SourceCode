using PLVirtualMachine.Base;

namespace PLVirtualMachine.Dynamic
{
  public class LockedFSMInfo
  {
    public LockedFSMInfo(DynamicFSM lockFSM)
    {
      LockedFSM = lockFSM;
      LastEntityMethodExecuteData = null;
      NeedRestoreAction = false;
    }

    public void SetLastActionMethodExecData(EntityMethodExecuteData lastActionMethodExecData)
    {
      LastEntityMethodExecuteData = lastActionMethodExecData;
      NeedRestoreAction = false;
    }

    public bool NeedRestoreAction { get; set; }

    public DynamicFSM LockedFSM { get; private set; }

    public EntityMethodExecuteData LastEntityMethodExecuteData { get; private set; }
  }
}
