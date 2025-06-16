using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Detectable", typeof(IDetectableComponent))]
[Depended("Position")]
public class VMDetectable : VMEngineComponent<IDetectableComponent> {
	public const string ComponentName = "Detectable";

	[Property("Enabled", "", false, true)]
	public bool DetectableEnabled {
		get => Component.IsEnabled;
		set => Component.IsEnabled = value;
	}
}