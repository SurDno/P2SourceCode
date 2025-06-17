using System.Collections.Generic;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine.Debug
{
  public class DebugFSMInfo(DynamicFSM fsm) {
    public DynamicFSM FSM = fsm;
    public Dictionary<ulong, IState> Breakpoints = new(UlongComparer.Instance);
    public EDebugFSMStatus CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY;
    public IState CurrentPosition = null;
    public EDebugFSMPositionType CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_NONE;
  }
}
