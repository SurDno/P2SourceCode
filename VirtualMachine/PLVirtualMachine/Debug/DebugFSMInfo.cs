using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Dynamic;
using System.Collections.Generic;

namespace PLVirtualMachine.Debug
{
  public class DebugFSMInfo
  {
    public DynamicFSM FSM;
    public Dictionary<ulong, IState> Breakpoints = new Dictionary<ulong, IState>((IEqualityComparer<ulong>) UlongComparer.Instance);
    public EDebugFSMStatus CurrentStatus;
    public IState CurrentPosition;
    public EDebugFSMPositionType CurrentPositionType;

    public DebugFSMInfo(DynamicFSM fsm)
    {
      this.FSM = fsm;
      this.CurrentPosition = (IState) null;
      this.CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_NONE;
      this.CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY;
    }
  }
}
