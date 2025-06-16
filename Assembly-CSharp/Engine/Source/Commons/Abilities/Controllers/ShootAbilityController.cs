// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.ShootAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ShootAbilityController : IAbilityController
  {
    [DataReadProxy(MemberEnum.None, Name = "Weapon")]
    [DataWriteProxy(MemberEnum.None, Name = "Weapon")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected WeaponKind weaponKind;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ShotType shot;
    private AbilityItem abilityItem;
    private IEntity owner;
    private StorableComponent storable;
    private IAttackerPlayerComponent attacker;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.owner = this.abilityItem.Ability.Owner;
      this.storable = this.owner.GetComponent<StorableComponent>();
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
      if (this.storable.Storage == null)
        ;
    }

    private void Cleanup()
    {
      if (this.attacker == null)
        return;
      this.attacker = (IAttackerPlayerComponent) null;
    }

    private void OnWeaponShootStartEvent(
      WeaponKind weapon,
      IEntity weaponEntity,
      ShotType shotType)
    {
      if (this.owner != weaponEntity || this.shot != ShotType.None && shotType != this.shot)
        return;
      this.abilityItem.Active = true;
    }

    private void OnWeaponShootEndEvent(WeaponKind weapon, IEntity weaponEntity, ShotType shotType)
    {
      if (this.owner != weaponEntity || this.shot != ShotType.None && shotType != this.shot)
        return;
      this.abilityItem.Active = false;
    }
  }
}
