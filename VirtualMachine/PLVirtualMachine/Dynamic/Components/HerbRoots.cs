using Cofe.Proxies;
using PLVirtualMachine.Objects;
using VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMHerbRoots))]
public class HerbRoots : VMHerbRoots, IInitialiseComponentFromHierarchy, IInitialiseEvents {
	public override string GetComponentTypeName() {
		return "HerbRootsComponent";
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "TriggerEnterEvent":
				TriggerEnterEvent += () => target.RaiseFromEngineImpl();
				break;
			case "TriggerLeaveEvent":
				TriggerLeaveEvent += () => target.RaiseFromEngineImpl();
				break;
			case "ActivateStartEvent":
				ActivateStartEvent += () => target.RaiseFromEngineImpl();
				break;
			case "ActivateEndEvent":
				ActivateEndEvent += () => target.RaiseFromEngineImpl();
				break;
			case "HerbSpawnEvent":
				HerbSpawnEvent += () => target.RaiseFromEngineImpl();
				break;
			case "LastHerbSpawnEvent":
				LastHerbSpawnEvent += () => target.RaiseFromEngineImpl();
				break;
		}
	}
}