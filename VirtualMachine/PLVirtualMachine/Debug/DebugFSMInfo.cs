using System.Collections.Generic;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine.Debug
{
  public class DebugFSMInfo
  {
    public DynamicFSM FSM;
    public Dictionary<ulong, IState> Breakpoints = new Dictionary<ulong, IState>(UlongComparer.Instance);
    public EDebugFSMStatus CurrentStatus;
    public IState CurrentPosition;
    public EDebugFSMPositionType CurrentPositionType;

    public DebugFSMInfo(DynamicFSM fsm)
    {
      FSM = fsm;
      CurrentPosition = null;
      CurrentPositionType = EDebugFSMPositionType.DEBUG_FSM_POSITION_NONE;
      CurrentStatus = EDebugFSMStatus.DEBUG_FSM_STATUS_PLAY;
    }
  }
}
