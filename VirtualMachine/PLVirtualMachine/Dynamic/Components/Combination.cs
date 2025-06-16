using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMCombination))]
public class Combination : VMCombination, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(Combination);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) {
		IParam obj;
		if (!((IBlueprint)templateObject).TryGetProperty("Combination.CombinationData", out obj))
			return;
		CombinationData = (ObjectCombinationDataStruct)obj.Value;
	}

	public void InitialiseEvent(DynamicEvent target) {
		var name = target.Name;
	}
}