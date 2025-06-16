// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMGlobalDoorsManager
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("GlobalDoorsManager", null)]
  public class VMGlobalDoorsManager : VMComponent
  {
    public const string ComponentName = "GlobalDoorsManager";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Method("Open door", "Door", "")]
    public virtual void OpenDoor(IObjRef gateObj)
    {
    }

    [Method("Unlock door", "Door", "")]
    public virtual void UnlockDoor(IObjRef gateObj)
    {
    }

    [Method("Is door open", "Door", "")]
    public virtual bool IsDoorOpen(IObjRef gateObj) => false;

    [Method("Is door unlocked", "Door", "")]
    public virtual bool IsDoorUnlocked(IObjRef gateObj) => false;

    [Method("", ",,,", "")]
    public virtual void SetGatesOpeningState(
      string gatesRootInfo,
      GateState gateState,
      string operationVolume,
      PriorityParameterEnum priority)
    {
    }

    [Method("", ",,,", "")]
    public virtual void SetGatesLockState(
      string gatesRootInfo,
      LockState gateLockState,
      string operationVolume,
      PriorityParameterEnum priority)
    {
    }

    [Method("", ",,,,,", "")]
    public virtual void SetGatesAllStates(
      string gatesRootInfo,
      GateState gateState,
      LockState gateLockState,
      string gateStatuses,
      string operationVolume,
      PriorityParameterEnum priority)
    {
    }

    public static VMGlobalDoorsManager Instance { get; protected set; }
  }
}
