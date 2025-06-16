using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMInteractable))]
public class Interactable : VMInteractable, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return "Interactive";
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) {
		IParam obj;
		if (!((IBlueprint)templateObject).TryGetProperty("Interactive.ObjectName", out obj))
			return;
		ObjectName = (ITextRef)obj.Value;
	}

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "BeginIteractEvent":
				BeginIteractEvent += (p1, p2, p3) => target.RaiseFromEngineImpl(p1, p2, p3);
				break;
			case "EndIteractEvent":
				EndIteractEvent += (p1, p2, p3) => target.RaiseFromEngineImpl(p1, p2, p3);
				break;
		}
	}
}