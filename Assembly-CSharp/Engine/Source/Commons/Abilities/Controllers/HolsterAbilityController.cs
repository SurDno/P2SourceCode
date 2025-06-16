using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class HolsterAbilityController : IAbilityController
  {
    [DataReadProxy(MemberEnum.None, Name = "Weapon")]
    [DataWriteProxy(MemberEnum.None, Name = "Weapon")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected WeaponKind weaponKind = WeaponKind.Unknown;
    private AbilityItem abilityItem;
    private StorableComponent storable;
    private IAttackerPlayerComponent attacker;

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
      this.Cleanup();
      if (this.storable != null)
      {
        this.storable.ChangeStorageEvent -= new Action<IStorableComponent>(this.ChangeStorageEvent);
        this.abilityItem.Active = false;
      }
      this.abilityItem = (AbilityItem) null;
      this.storable = (StorableComponent) null;
    }

    private void ChangeStorageEvent(IStorableComponent sender) => this.CheckItem();

    private void CheckItem()
    {
      this.Cleanup();
      IStorageComponent storage = this.storable.Storage;
      if (storage == null)
        return;
      this.attacker = storage.Owner.GetComponent<IAttackerPlayerComponent>();
      if (this.attacker != null)
      {
        this.attacker.WeaponHolsterStartEvent += new Action<WeaponKind>(this.WeaponHolsterStartEvent);
        this.attacker.WeaponUnholsterEndEvent += new Action<WeaponKind>(this.WeaponUnholsterEndEvent);
        this.UpdateAbility();
      }
    }

    private void Cleanup()
    {
      if (this.attacker == null)
        return;
      this.attacker.WeaponHolsterStartEvent -= new Action<WeaponKind>(this.WeaponHolsterStartEvent);
      this.attacker.WeaponUnholsterEndEvent -= new Action<WeaponKind>(this.WeaponUnholsterEndEvent);
      this.attacker = (IAttackerPlayerComponent) null;
      this.UpdateAbility();
    }

    private void WeaponHolsterStartEvent(WeaponKind weapon) => this.UpdateAbility();

    private void WeaponUnholsterEndEvent(WeaponKind weapon) => this.UpdateAbility();

    private void UpdateAbility()
    {
      this.abilityItem.Active = this.attacker != null && this.attacker.CurrentWeapon == this.weaponKind;
    }
  }
}
