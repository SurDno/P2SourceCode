using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ShootAbilityController : IAbilityController
  {
    [DataReadProxy(Name = "Weapon")]
    [DataWriteProxy(Name = "Weapon")]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected WeaponKind weaponKind;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ShotType shot;
    private AbilityItem abilityItem;
    private IEntity owner;
    private StorableComponent storable;
    private IAttackerPlayerComponent attacker;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      owner = this.abilityItem.Ability.Owner;
      storable = owner.GetComponent<StorableComponent>();
      if (storable == null)
        return;
      storable.ChangeStorageEvent += ChangeStorageEvent;
      CheckItem();
    }

    public void Shutdown()
    {
      Cleanup();
      if (storable != null)
      {
        storable.ChangeStorageEvent -= ChangeStorageEvent;
        abilityItem.Active = false;
      }
      abilityItem = null;
      storable = null;
    }

    private void ChangeStorageEvent(IStorableComponent sender) => CheckItem();

    private void CheckItem()
    {
      Cleanup();
      if (storable.Storage == null)
        ;
    }

    private void Cleanup()
    {
      if (attacker == null)
        return;
      attacker = null;
    }

    private void OnWeaponShootStartEvent(
      WeaponKind weapon,
      IEntity weaponEntity,
      ShotType shotType)
    {
      if (owner != weaponEntity || shot != ShotType.None && shotType != shot)
        return;
      abilityItem.Active = true;
    }

    private void OnWeaponShootEndEvent(WeaponKind weapon, IEntity weaponEntity, ShotType shotType)
    {
      if (owner != weaponEntity || shot != ShotType.None && shotType != shot)
        return;
      abilityItem.Active = false;
    }
  }
}
