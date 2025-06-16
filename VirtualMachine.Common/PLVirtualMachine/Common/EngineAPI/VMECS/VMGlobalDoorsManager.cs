using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

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
