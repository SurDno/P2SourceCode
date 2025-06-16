using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Scene")]
  public class VMScene : VMComponent
  {
    public const string ComponentName = "Scene";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);
  }
}
