using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class IndoorAbilityController : IAbilityController {
	private LocationItemComponent locationItemComponent;
	private AbilityItem abilityItem;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		locationItemComponent = this.abilityItem.Ability.Owner.GetComponent<LocationItemComponent>();
		if (locationItemComponent == null)
			return;
		locationItemComponent.OnChangeLocation += OnChangeLocation;
		OnChangeLocation(locationItemComponent, locationItemComponent.Location);
	}

	public void Shutdown() {
		if (locationItemComponent == null)
			return;
		locationItemComponent.OnChangeLocation -= OnChangeLocation;
		locationItemComponent = null;
	}

	private void OnChangeLocation(ILocationItemComponent sender, ILocationComponent location) {
		abilityItem.Active = locationItemComponent.IsIndoor;
	}
}