using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMLocation))]
public class Location : VMLocation, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(Location);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "OnHibernationChange":
				OnHibernationChange += () => target.RaiseFromEngineImpl();
				break;
			case "OnPlayerInside":
				OnPlayerInside += p1 => target.RaiseFromEngineImpl(p1);
				break;
		}
	}
}