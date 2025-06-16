using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class OpenContainerAbilityController : IAbilityController {
	private PlayerControllerComponent controller;
	private AbilityItem abilityItem;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		controller = this.abilityItem.Ability.Owner.GetComponent<PlayerControllerComponent>();
		if (controller == null)
			return;
		controller.OpenContainerEvent += OpenContainerEvent;
	}

	public void Shutdown() {
		if (controller == null)
			return;
		controller.OpenContainerEvent -= OpenContainerEvent;
		controller = null;
	}

	private void OpenContainerEvent(IInventoryComponent container) {
		abilityItem.Active = true;
		abilityItem.Active = false;
	}
}