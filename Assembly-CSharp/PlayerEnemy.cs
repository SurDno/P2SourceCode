// Decompiled with JetBrains decompiler
// Type: PlayerEnemy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Services;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PlayerWeaponServiceNew))]
public class PlayerEnemy : EnemyBase
{
  [Header("You can use ComputeNpc to fill this!")]
  private Animator animator;
  private FightAnimatorBehavior.AnimatorState animatorState;
  private float staggerTimeLeft;
  private bool pushStarted;
  private PlayerWeaponServiceNew weaponService;
  private CharacterController characterController;
  private Vector3 velocity;

  private void PushAnimationEvent()
  {
  }

  [Inspected]
  public override bool IsStagger => this.animatorState.IsStagger;

  protected override void OnDisable()
  {
    base.OnDisable();
    this.animatorState?.Reset();
  }

  private void Awake()
  {
    this.StaggerTime = 5f;
    this.characterController = this.GetComponent<CharacterController>();
    this.weaponService = this.GetComponent<PlayerWeaponServiceNew>();
    this.weaponService.WeaponShootEvent += (Action<WeaponKind, IEntity, ShotType, ReactionType, ShotSubtypeEnum>) ((weapon, weaponEntity, shotType, reactionType, shotSubtype) =>
    {
      switch (weapon)
      {
        case WeaponKind.Unknown:
          this.FirePunchEvent(WeaponEnum.Unknown, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Hands:
          this.FirePunchEvent(WeaponEnum.Hands, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Knife:
          this.FirePunchEvent(WeaponEnum.Knife, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Revolver:
          this.FirePunchEvent(WeaponEnum.Revolver, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Rifle:
          this.FirePunchEvent(WeaponEnum.Rifle, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Flashlight:
          this.FirePunchEvent(WeaponEnum.Flashlight, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Scalpel:
          this.FirePunchEvent(WeaponEnum.Scalpel, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Lockpick:
          this.FirePunchEvent(WeaponEnum.Lockpick, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Shotgun:
          this.FirePunchEvent(WeaponEnum.Shotgun, weaponEntity, shotType, reactionType, shotSubtype);
          break;
      }
    });
    this.animator = this.GetComponent<Animator>();
    this.animatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
  }

  public override Vector3 Velocity => this.characterController.velocity;

  public override void Push(Vector3 velocity, EnemyBase enemy)
  {
    base.Push(velocity, enemy);
    this.FireWasPushedEvent(enemy);
  }

  private void Reaction(PunchTypeEnum punchType, EnemyBase enemy, ReactionType reactionType)
  {
    float fightReactionX;
    float fightReactionY;
    this.CaclulateRection(enemy, reactionType, out fightReactionX, out fightReactionY);
    if ((double) fightReactionY < 0.0)
      fightReactionY = 0.0f;
    this.animator.SetFloat("Fight.ReactionY", fightReactionY);
    this.animator.SetFloat("Fight.ReactionX", fightReactionX);
    this.animator.SetTrigger("Fight.Triggers/Reaction");
    float num;
    switch (punchType)
    {
      case PunchTypeEnum.Light:
        num = 0.25f;
        break;
      case PunchTypeEnum.Moderate:
        num = 0.5f;
        break;
      case PunchTypeEnum.Strong:
        num = 0.75f;
        break;
      default:
        num = 0.4f;
        break;
    }
    this.weaponService.ReactionLayerWeight = num;
    this.weaponService.Reaction();
    this.animator.SetTrigger("Triggers/Reaction");
  }

  public override void Punch(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    this.Reaction(punchType, enemy, reactionType);
    this.pivot.PlaySound(Pivot.SoundEnum.HittedVocal, protagonist: true);
    if ((punchType == PunchTypeEnum.Strong || punchType == PunchTypeEnum.Moderate) && (double) UnityEngine.Random.value < 0.20000000298023224)
      enemy.PlayLipSync(CombatCryEnum.PunchUnblocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab, protagonist: true);
        break;
      case WeaponEnum.Rifle:
      case WeaponEnum.Samopal:
        this.pivot.PlaySound(Pivot.SoundEnum.BulletHit);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted, protagonist: true);
        break;
    }
    this.FireWasPunchedEvent(enemy);
  }

  public override void PunchToBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    this.Reaction(punchType, enemy, reactionType);
    enemy.PlayLipSync(CombatCryEnum.PunchBlocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.StabBlock, protagonist: true);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.BlockHitted, protagonist: true);
        break;
    }
    this.FireWasPunchedToBlockEvent(enemy);
  }

  public override void PunchToQuickBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    this.Reaction(punchType, enemy, reactionType);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.StabBlock, protagonist: true);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.BlockHitted, protagonist: true);
        break;
    }
    this.FireWasPunchedToQuickBlockEvent(enemy);
  }

  public override void PunchToDodge(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    this.FireWasPunchedToDodgeEvent(enemy);
  }

  public override void PunchLowStamina(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    this.FireWasLowStaminaPunchedEvent(enemy);
  }

  public override void PunchToStagger(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    this.FireWasPunchedToStaggerEvent(enemy);
    this.Reaction(punchType, enemy, reactionType);
    this.pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.StabBlock, protagonist: true);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted, protagonist: true);
        break;
    }
  }

  public override void Stagger(EnemyBase enemy)
  {
    enemy.PlayLipSync(CombatCryEnum.SuperPunch);
    this.staggerTimeLeft = this.StaggerTime;
    this.animator.SetTrigger("Fight.Triggers/Stagger");
    this.pivot.PlaySound(Pivot.SoundEnum.StaggerVocal, protagonist: true);
    this.pivot.PlaySound(Pivot.SoundEnum.StaggerNonVocal, protagonist: true);
    this.pivot.PlaySound(Pivot.SoundEnum.BlockHitted, protagonist: true);
    this.FireWasStaggeredEvent(enemy);
  }

  public override void KnockDown(EnemyBase enemy)
  {
    enemy.PlayLipSync(CombatCryEnum.Catch);
    this.animator.SetTrigger("Triggers/KnockDown");
    this.FireWasKnockDownedEvent(enemy);
  }

  private void Update()
  {
    this.BlockType = this.BlockStance ? BlockTypeEnum.Block : BlockTypeEnum.NotBlocking;
    if ((double) this.staggerTimeLeft <= 0.0)
      return;
    this.staggerTimeLeft -= Time.deltaTime;
    if ((double) this.staggerTimeLeft <= 0.0)
      this.animator.SetTrigger("Fight.Triggers/CancelStagger");
    else
      this.BlockType = BlockTypeEnum.Stagger;
  }

  private new void FixedUpdate()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    base.FixedUpdate();
  }
}
