using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMController))]
public class Controller : VMController, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(Controller);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "BeginControllIteractEvent":
				BeginControllIteractEvent += (p1, p2, p3) => target.RaiseFromEngineImpl(p1, p2, p3);
				break;
			case "EndControllIteractEvent":
				EndControllIteractEvent += (p1, p2, p3) => target.RaiseFromEngineImpl(p1, p2, p3);
				break;
		}
	}
}