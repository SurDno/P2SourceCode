using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ItemUseAbilityController : IAbilityController {
	private AbilityItem abilityItem;
	private StorableComponent storable;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		storable = this.abilityItem.Ability.Owner.GetComponent<StorableComponent>();
		if (storable == null)
			return;
		storable.UseEvent += UseEvent;
	}

	public void Shutdown() {
		if (storable != null)
			storable.UseEvent -= UseEvent;
		abilityItem = null;
		storable = null;
	}

	private void UseEvent(IStorableComponent sender) {
		abilityItem.Active = true;
		abilityItem.Active = false;
	}
}