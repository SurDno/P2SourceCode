using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMDisease))]
public class Disease : VMDisease, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return "DiseaseComponent";
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		var name = target.Name;
	}
}