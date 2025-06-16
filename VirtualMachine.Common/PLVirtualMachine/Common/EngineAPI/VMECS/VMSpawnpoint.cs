using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Milestone", typeof (ISpawnpointComponent))]
  [Depended("Position")]
  public class VMSpawnpoint : VMEngineComponent<ISpawnpointComponent>
  {
    public const string ComponentName = "Milestone";
  }
}
