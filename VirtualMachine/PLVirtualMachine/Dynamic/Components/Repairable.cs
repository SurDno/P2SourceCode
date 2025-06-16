using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMRepairable))]
public class Repairable : VMRepairable, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return "RepairableComponent";
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		if (!(target.Name == "OnChangeDurability"))
			return;
		OnChangeDurability += p1 => target.RaiseFromEngineImpl(p1);
	}
}