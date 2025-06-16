using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components;

[FactoryProxy(typeof(VMAttackerPlayer))]
public class AttackerPlayer :
	VMAttackerPlayer,
	IInitialiseComponentFromHierarchy,
	IInitialiseEvents {
	public override string GetComponentTypeName() {
		return nameof(AttackerPlayer);
	}

	public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject) { }

	public void InitialiseEvent(DynamicEvent target) {
		switch (target.Name) {
			case "HandsHolsteredEvent":
				HandsHolsteredEvent += p1 => target.RaiseFromEngineImpl(p1);
				break;
			case "HandsUnholsteredEvent":
				HandsUnholsteredEvent += p1 => target.RaiseFromEngineImpl(p1);
				break;
		}
	}
}