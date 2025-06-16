using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Model", typeof (IDynamicModelComponent))]
  public class VMDynamicModel : VMEngineComponent<IDynamicModelComponent>
  {
    public const string ComponentName = "Model";

    [Property("Model name", "", false)]
    public IModel Object
    {
      get => Component.Model;
      set => Component.Model = value;
    }
  }
}
