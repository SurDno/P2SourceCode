using System;
using Cofe.Loggers;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("RepairableComponent", typeof(IRepairableComponent))]
public class VMRepairable : VMEngineComponent<IRepairableComponent> {
	public const string ComponentName = "RepairableComponent";

	[Property("Durability", "", false, 1f, false)]
	public float Health {
		get {
			if (Component != null)
				return Component.Durability.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Durability.Value = value;
		}
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.Durability.ChangeValueEvent -= ChangeDurabilityValueEvent;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.Durability.ChangeValueEvent += ChangeDurabilityValueEvent;
	}

	private void ChangeDurabilityValueEvent(float value) {
		var changeDurability = OnChangeDurability;
		if (changeDurability == null)
			return;
		changeDurability(value);
	}

	[Event("Change durability", "Value")] public event Action<float> OnChangeDurability;
}