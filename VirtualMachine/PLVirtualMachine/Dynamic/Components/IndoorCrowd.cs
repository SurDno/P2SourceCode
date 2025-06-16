using Cofe.Proxies;
using PLVirtualMachine.Objects;
using VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMIndoorCrowd))]
public class IndoorCrowd : VMIndoorCrowd, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return "IndoorCrowdComponent";
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "NeedCreateObjectEvent":
				NeedCreateObjectEvent += p1 => target.RaiseFromEngineImpl(p1);
				break;
			case "NeedDeleteObjectEvent":
				NeedDeleteObjectEvent += p1 => target.RaiseFromEngineImpl(p1);
				break;
		}
	}
}