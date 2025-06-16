using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using System;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class OutdoorAbilityController : IAbilityController
  {
    private LocationItemComponent locationItemComponent;
    private AbilityItem abilityItem;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.locationItemComponent = this.abilityItem.Ability.Owner.GetComponent<LocationItemComponent>();
      if (this.locationItemComponent == null)
        return;
      this.locationItemComponent.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.OnChangeLocation);
      this.OnChangeLocation((ILocationItemComponent) this.locationItemComponent, this.locationItemComponent.Location);
    }

    public void Shutdown()
    {
      if (this.locationItemComponent == null)
        return;
      this.locationItemComponent.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.OnChangeLocation);
      this.locationItemComponent = (LocationItemComponent) null;
    }

    private void OnChangeLocation(ILocationItemComponent sender, ILocationComponent location)
    {
      this.abilityItem.Active = !this.locationItemComponent.IsIndoor;
    }
  }
}
