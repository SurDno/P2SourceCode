using Engine.Common.Components;
using Engine.Common.Components.Storable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Inventory", typeof (IInventoryComponent))]
  public class VMInventory : VMEngineComponent<IInventoryComponent>
  {
    public const string ComponentName = "Inventory";

    [Property("Available", "", false, true, false)]
    public bool Available
    {
      get => this.Component.Available.Value;
      set => this.Component.Available.Value = value;
    }

    [Property("Enabled", "", false, true, false)]
    public bool Enabled
    {
      get => this.Component.Enabled.Value;
      set => this.Component.Enabled.Value = value;
    }

    [Property("Open state", "", false, ContainerOpenStateEnum.Open, false)]
    public ContainerOpenStateEnum OpenState
    {
      get => this.Component.OpenState.Value;
      set => this.Component.OpenState.Value = value;
    }

    [Property("Disease", "", false, 0.0f, false)]
    public float Disease
    {
      get => this.Component.Disease.Value;
      set => this.Component.Disease.Value = value;
    }
  }
}
