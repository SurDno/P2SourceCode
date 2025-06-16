using Engine.Common.Components;
using Engine.Common.Components.Movable;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("CrowdItemComponent", typeof(ICrowdItemComponent))]
public class VMCrowdItem : VMEngineComponent<ICrowdItemComponent> {
	public const string ComponentName = "CrowdItemComponent";

	[Property("Area", "")] public AreaEnum Area => Component.Area;
}