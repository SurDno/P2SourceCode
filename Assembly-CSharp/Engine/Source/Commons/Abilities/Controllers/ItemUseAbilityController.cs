using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using System;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ItemUseAbilityController : IAbilityController
  {
    private AbilityItem abilityItem;
    private StorableComponent storable;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.storable = this.abilityItem.Ability.Owner.GetComponent<StorableComponent>();
      if (this.storable == null)
        return;
      this.storable.UseEvent += new Action<IStorableComponent>(this.UseEvent);
    }

    public void Shutdown()
    {
      if (this.storable != null)
        this.storable.UseEvent -= new Action<IStorableComponent>(this.UseEvent);
      this.abilityItem = (AbilityItem) null;
      this.storable = (StorableComponent) null;
    }

    private void UseEvent(IStorableComponent sender)
    {
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }
  }
}
