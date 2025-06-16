using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMMessangerComponent))]
public class MessangerComponent :
	VMMessangerComponent,
	IInitialiseComponentFromHierarchy,
	IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(MessangerComponent);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		var name = target.Name;
	}
}