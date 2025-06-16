using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMBoundCharacterComponent))]
public class BoundCharacterComponent :
	VMBoundCharacterComponent,
	IInitialiseComponentFromHierarchy,
	IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(BoundCharacterComponent);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		if (!(target.Name == "OnChangeBoundHealthState"))
			return;
		OnChangeBoundHealthState += p1 => target.RaiseFromEngineImpl(p1);
	}
}