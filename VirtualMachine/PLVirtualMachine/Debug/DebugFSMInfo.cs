// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Debug.DebugFSMInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.VMDebug;
using PLVirtualMachine.Dynamic;
using System.Collections.Generic;

#nullable disable
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
