using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMWaterSupplyController))]
public class WaterSupplyController :
	VMWaterSupplyController,
	IInitialiseComponentFromHierarchy,
	IInitialiseEvents {
	public override string GetComponentTypeName() {
		return "WaterSupplyControllerComponent";
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		var name = target.Name;
	}
}