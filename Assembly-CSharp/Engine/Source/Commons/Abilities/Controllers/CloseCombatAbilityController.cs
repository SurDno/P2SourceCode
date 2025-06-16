// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.CloseCombatAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Effects.Values;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CloseCombatAbilityController : IAbilityController, IAbilityValueContainer
  {
    [DataReadProxy(MemberEnum.None, Name = "Punch")]
    [DataWriteProxy(MemberEnum.None, Name = "Punch")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ShotType punchType;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ShotSubtypeEnum punchSubtype;
    [DataReadProxy(MemberEnum.None, Name = "weapon")]
    [DataWriteProxy(MemberEnum.None, Name = "weapon")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected WeaponEnum weaponKind;
    [Inspected]
    private StorableComponent storable;
    private AbilityItem abilityItem;
    private EnemyBase attacker;
    private IEntity itemOwner;
    private Dictionary<AbilityValueNameEnum, IAbilityValue> values = new Dictionary<AbilityValueNameEnum, IAbilityValue>();

    public ReactionType ReactionType { get; set; }

    public WeaponEnum WeaponKind => this.weaponKind;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      this.Subcribe();
    }

    private void Subcribe()
    {
      this.Unsubscribe();
      this.storable = this.abilityItem.Ability.Owner.GetComponent<StorableComponent>();
      if (this.storable == null)
        return;
      this.storable.ChangeStorageEvent += new Action<IStorableComponent>(this.ChangeStorageEvent);
      if (this.storable.Storage == null)
        return;
      this.itemOwner = this.storable.Storage.Owner;
      if (this.itemOwner == null)
        return;
      ((IEntityView) this.itemOwner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChanged);
      GameObject gameObject = ((IEntityView) this.itemOwner).GameObject;
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        return;
      AbilityCache abilityCache = this.abilityItem.Ability.AbilityCache;
      if ((UnityEngine.Object) abilityCache.EnemyBase != (UnityEngine.Object) null && (UnityEngine.Object) abilityCache.EnemyBase.gameObject == (UnityEngine.Object) gameObject)
        this.attacker = abilityCache.EnemyBase;
      else
        abilityCache.EnemyBase = this.attacker = gameObject.GetComponentNonAlloc<EnemyBase>();
      if ((UnityEngine.Object) this.attacker == (UnityEngine.Object) null)
        return;
      this.attacker.PunchEvent += new Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum>(this.OnPunchEvent);
    }

    private void Unsubscribe()
    {
      if ((UnityEngine.Object) this.attacker != (UnityEngine.Object) null)
      {
        this.attacker.PunchEvent -= new Action<IEntity, ShotType, ReactionType, WeaponEnum, ShotSubtypeEnum>(this.OnPunchEvent);
        this.attacker = (EnemyBase) null;
      }
      if (this.itemOwner != null)
      {
        ((IEntityView) this.itemOwner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChanged);
        this.itemOwner = (IEntity) null;
      }
      if (this.storable == null)
        return;
      this.storable.ChangeStorageEvent -= new Action<IStorableComponent>(this.ChangeStorageEvent);
      this.storable = (StorableComponent) null;
    }

    public void Shutdown()
    {
      if (this.storable != null)
        this.abilityItem.Active = false;
      this.Unsubscribe();
    }

    private void ChangeStorageEvent(IStorableComponent sender) => this.Subcribe();

    private void OnGameObjectChanged() => this.Subcribe();

    private void OnPunchEvent(
      IEntity weaponEntity,
      ShotType punch,
      ReactionType reactionType,
      WeaponEnum weapon,
      ShotSubtypeEnum subtype = ShotSubtypeEnum.None)
    {
      if (weapon != this.weaponKind || weaponEntity != null && weaponEntity != this.storable.Owner || punch != this.punchType && this.punchType != ShotType.None || subtype != this.punchSubtype && this.punchSubtype != ShotSubtypeEnum.None)
        return;
      this.ReactionType = reactionType;
      this.PrepareData();
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }

    private void PrepareData()
    {
      this.values.Clear();
      NPCWeaponService component = ((IEntityView) this.itemOwner).GameObject.GetComponent<NPCWeaponService>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return;
      Vector3 knifeSpeed = component.KnifeSpeed;
      Vector3 knifePosition = component.KnifePosition;
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      Vector3 vector3 = GameCamera.Instance.CameraTransform.InverseTransformVector(knifeSpeed);
      float magnitude = vector3.magnitude;
      float num1 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
      Vector3 viewportPoint = camera.WorldToViewportPoint(knifePosition);
      if ((double) viewportPoint.z < 0.0)
        viewportPoint *= -1f;
      float num2 = Mathf.Clamp01(viewportPoint.x) - 0.5f;
      float num3 = Mathf.Clamp01(viewportPoint.y) - 0.5f;
      float max = 0.4f;
      Vector2 vector2 = new Vector2(Mathf.Clamp(num2, -max, max), Mathf.Clamp(num3, -max, max));
      this.values.Add(AbilityValueNameEnum.ScarPosition, (IAbilityValue) new AbilityValue<Vector2>()
      {
        Value = vector2
      });
      this.values.Add(AbilityValueNameEnum.ScarAngle, (IAbilityValue) new AbilityValue<float>()
      {
        Value = num1
      });
      this.values.Add(AbilityValueNameEnum.ScarScale, (IAbilityValue) new AbilityValue<float>()
      {
        Value = 0.5f
      });
    }

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct
    {
      IAbilityValue abilityValue;
      this.values.TryGetValue(parameter, out abilityValue);
      return abilityValue as IAbilityValue<T>;
    }
  }
}
