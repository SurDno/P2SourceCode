using Engine.Common.Components;
using Engine.Common.Components.Storable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Inventory", typeof(IInventoryComponent))]
public class VMInventory : VMEngineComponent<IInventoryComponent> {
	public const string ComponentName = "Inventory";

	[Property("Available", "", false, true, false)]
	public bool Available {
		get => Component.Available.Value;
		set => Component.Available.Value = value;
	}

	[Property("Enabled", "", false, true, false)]
	public bool Enabled {
		get => Component.Enabled.Value;
		set => Component.Enabled.Value = value;
	}

	[Property("Open state", "", false, ContainerOpenStateEnum.Open, false)]
	public ContainerOpenStateEnum OpenState {
		get => Component.OpenState.Value;
		set => Component.OpenState.Value = value;
	}

	[Property("Disease", "", false, 0.0f, false)]
	public float Disease {
		get => Component.Disease.Value;
		set => Component.Disease.Value = value;
	}
}