using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMBlueprintComponent))]
public class BlueprintComponent :
	VMBlueprintComponent,
	IInitialiseComponentFromHierarchy,
	IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(BlueprintComponent);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "Complete":
				Complete += () => target.RaiseFromEngineImpl();
				break;
			case "Attach":
				Attach += () => target.RaiseFromEngineImpl();
				break;
		}
	}
}