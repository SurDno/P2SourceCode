// Decompiled with JetBrains decompiler
// Type: NPCWeaponControllerBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using System;
using UnityEngine;

#nullable disable
public class NPCWeaponControllerBase : INPCWeaponController
{
  protected Animator animator;
  protected AnimatorState45 animatorState;
  private FightAnimatorBehavior.AnimatorState fightAnimatorState;
  protected float layersWeight = 0.0f;
  protected int walkLayerIndex;
  protected int attackLayerIndex;
  protected int reactionLayerIndex;
  protected NPCWeaponService service;
  private bool droped;
  protected IEntity item;
  protected bool weaponIsShown;

  public event Action<IEntity, ShotType, ReactionType> WeaponShootEvent;

  public virtual void Initialise(NPCWeaponService service)
  {
    this.service = service;
    this.animator = service.gameObject.GetComponent<Pivot>().GetAnimator();
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
    this.GetLayersIndices();
    this.layersWeight = 0.0f;
    this.SetLayers(0.0f);
  }

  public virtual void IndoorChanged()
  {
  }

  protected virtual void SetLayers(float weight, bool immediate = false)
  {
    if ((UnityEngine.Object) this.service == (UnityEngine.Object) null)
      return;
    if (this.walkLayerIndex != -1)
      this.service.AddNeededLayer(this.walkLayerIndex, weight);
    if (this.attackLayerIndex != -1)
      this.service.AddNeededLayer(this.attackLayerIndex, weight);
    if (this.reactionLayerIndex != -1)
      this.service.AddNeededLayer(this.reactionLayerIndex, weight);
    if (!immediate)
      return;
    this.service.ForceUpdateLayers();
  }

  protected virtual void GetLayersIndices()
  {
  }

  protected virtual void ShowWeapon(bool show) => this.weaponIsShown = show;

  public virtual void Activate()
  {
    this.animatorState.SetTrigger("Fight.Triggers/WeaponPrepare");
    this.SetLayers(1f);
  }

  public virtual void Shutdown() => this.SetLayers(0.0f);

  public virtual bool IsChangingWeapon() => (double) this.layersWeight < 1.0 || !this.weaponIsShown;

  protected void Drop()
  {
    if (!this.droped)
    {
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent != null)
        weaponShootEvent(this.item, ShotType.Drop, ReactionType.None);
    }
    this.droped = true;
  }

  protected void BombHit()
  {
    Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
    if (weaponShootEvent == null)
      return;
    weaponShootEvent(this.item, ShotType.Drop, ReactionType.None);
  }

  public void ActivateImmediate()
  {
    this.animatorState.SetTrigger("Fight.Triggers/CancelWeaponPrepare");
    this.layersWeight = 1f;
    this.SetLayers(1f, true);
    this.ShowWeapon(true);
  }

  public void ShutdownImmediate()
  {
    this.layersWeight = 0.0f;
    this.SetLayers(0.0f, true);
    this.ShowWeapon(false);
  }

  public bool Validate(GameObject gameObject) => throw new NotImplementedException();

  public virtual void Update()
  {
    float num = Mathf.MoveTowards(this.layersWeight, 1f, Time.deltaTime * 5f);
    if ((double) this.layersWeight == 0.0 && (double) num > 0.0)
      this.animatorState.SetTrigger("Fight.Triggers/WeaponPrepare");
    if ((double) this.layersWeight < 1.0 && (double) num >= 1.0)
      this.animatorState.SetTrigger("Fight.Triggers/WeaponOn");
    this.layersWeight = num;
  }

  public virtual void UpdateSilent()
  {
    float num = Mathf.MoveTowards(this.layersWeight, 0.0f, Time.deltaTime / 0.5f);
    if ((double) this.layersWeight > 0.5 && (double) num <= 0.5)
      this.ShowWeapon(false);
    this.layersWeight = num;
  }

  public void SetItem(IEntity item)
  {
    this.item = item;
    this.droped = false;
  }

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    switch (weaponAction)
    {
      case WeaponActionEnum.Uppercut:
        this.animatorState.SetTrigger("Fight.Triggers/Attack");
        this.animator.SetInteger("Fight.AttackType", 8);
        break;
      case WeaponActionEnum.JabAttack:
        this.animatorState.SetTrigger("Fight.Triggers/Attack");
        this.animator.SetInteger("Fight.AttackType", 0);
        break;
      case WeaponActionEnum.StepAttack:
        this.animatorState.SetTrigger("Fight.Triggers/Attack");
        this.animator.SetInteger("Fight.AttackType", 1);
        break;
      case WeaponActionEnum.TelegraphAttack:
        this.animatorState.SetTrigger("Fight.Triggers/Attack");
        this.animator.SetInteger("Fight.AttackType", 2);
        break;
      case WeaponActionEnum.RunAttack:
        this.animatorState.SetTrigger("Fight.Triggers/RunPunch");
        break;
      case WeaponActionEnum.Push:
        this.animatorState.SetTrigger("Fight.Triggers/Push");
        break;
      case WeaponActionEnum.KnockDown:
        Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
        if (weaponShootEvent == null)
          break;
        weaponShootEvent(this.item, ShotType.KnockDown, ReactionType.None);
        break;
      case WeaponActionEnum.SamopalAim:
        this.animatorState.SetTrigger("Fight.Triggers/AimSamopal");
        break;
      case WeaponActionEnum.SamopalFire:
        this.animatorState.SetTrigger("Fight.Triggers/FireSamopal");
        break;
      case WeaponActionEnum.RifleAim:
        this.animatorState.SetTrigger("Fight.Triggers/AimRifle");
        break;
      case WeaponActionEnum.RifleFire:
        this.animatorState.SetTrigger("Fight.Triggers/FireRifle");
        break;
      case WeaponActionEnum.ForcedSamopalDrop:
        this.animatorState.SetTrigger("Fight.Triggers/DropSamopal");
        break;
    }
  }

  public virtual void OnAnimatorEvent(string data)
  {
    if ((double) this.layersWeight < 0.5)
      return;
    ReactionType reactionType = !data.EndsWith(" Left") ? (!data.EndsWith(" Right") ? (!data.EndsWith(" Front") ? (!data.EndsWith(" Uppercut") ? ReactionType.Front : ReactionType.Uppercut) : ReactionType.Front) : ReactionType.Right) : ReactionType.Left;
    if (data.StartsWith("Hands.Punch.UltraLight"))
    {
      if (this.IsReactingToHit())
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.UltraLight, reactionType);
    }
    else if (data.StartsWith("Hands.Punch.Light"))
    {
      if (this.IsReactingToHit())
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Light, reactionType);
    }
    else if (data.StartsWith("Hands.Punch.Moderate"))
    {
      if (this.IsReactingToHit())
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Moderate, reactionType);
    }
    else if (data.StartsWith("Hands.Punch.Strong"))
    {
      if (this.IsReactingToHit())
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Strong, reactionType);
    }
    else if (data.StartsWith("Hands.Uppercut"))
    {
      if (this.IsReactingToHit())
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Uppercut, ReactionType.Uppercut);
    }
    else if (data.StartsWith("Hands.Push"))
    {
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Push, ReactionType.None);
    }
    else if (data.StartsWith("Hands.Prepunch"))
    {
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Prepunch, ReactionType.None);
    }
    else if (data.StartsWith("Bomb.Throw"))
    {
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Throw, ReactionType.None);
    }
    else if (data.StartsWith("Samopal.Hit"))
    {
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Fire, ReactionType.None);
    }
    else if (data.StartsWith("Samopal.Drop"))
      this.Drop();
    else if (data.StartsWith("Rifle.Hit"))
    {
      if (this.IsReactingToHit())
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent == null)
        return;
      weaponShootEvent(this.item, ShotType.Fire, ReactionType.None);
    }
    else
    {
      if (!data.StartsWith("Rifle.Punch"))
        return;
      Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
      if (weaponShootEvent != null)
        weaponShootEvent(this.item, ShotType.Moderate, ReactionType.Front);
    }
  }

  private bool IsReactingToHit()
  {
    return this.fightAnimatorState != null && (this.fightAnimatorState.IsReaction || this.fightAnimatorState.IsStagger);
  }

  public virtual void PunchReaction(ReactionType reactionType)
  {
  }
}
