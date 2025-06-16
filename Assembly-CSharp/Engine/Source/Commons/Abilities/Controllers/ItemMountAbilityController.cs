using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Inspectors;
using System;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ItemMountAbilityController : IAbilityController
  {
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected InventoryGroup group;
    private AbilityItem abilityItem;
    private StorableComponent storable;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.storable = this.abilityItem.Ability.Owner.GetComponent<StorableComponent>();
      if (this.storable == null)
        return;
      this.storable.ChangeStorageEvent += new Action<IStorableComponent>(this.ChangeStorageEvent);
      this.CheckItem();
    }

    public void Shutdown()
    {
      if (this.storable != null)
      {
        this.storable.ChangeStorageEvent -= new Action<IStorableComponent>(this.ChangeStorageEvent);
        this.abilityItem.Active = false;
      }
      this.abilityItem = (AbilityItem) null;
      this.storable = (StorableComponent) null;
    }

    private void CheckItem()
    {
      IInventoryComponent container = this.storable.Container;
      this.abilityItem.Active = container != null && container.GetGroup() == this.group;
    }

    private void ChangeStorageEvent(IStorableComponent sender) => this.CheckItem();
  }
}
