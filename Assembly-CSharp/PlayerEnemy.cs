using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Services;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

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
  public override bool IsStagger => animatorState.IsStagger;

  protected override void OnDisable()
  {
    base.OnDisable();
    animatorState?.Reset();
  }

  private void Awake()
  {
    StaggerTime = 5f;
    characterController = GetComponent<CharacterController>();
    weaponService = GetComponent<PlayerWeaponServiceNew>();
    weaponService.WeaponShootEvent += (weapon, weaponEntity, shotType, reactionType, shotSubtype) =>
    {
      switch (weapon)
      {
        case WeaponKind.Unknown:
          FirePunchEvent(WeaponEnum.Unknown, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Hands:
          FirePunchEvent(WeaponEnum.Hands, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Knife:
          FirePunchEvent(WeaponEnum.Knife, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Revolver:
          FirePunchEvent(WeaponEnum.Revolver, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Rifle:
          FirePunchEvent(WeaponEnum.Rifle, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Flashlight:
          FirePunchEvent(WeaponEnum.Flashlight, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Scalpel:
          FirePunchEvent(WeaponEnum.Scalpel, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Lockpick:
          FirePunchEvent(WeaponEnum.Lockpick, weaponEntity, shotType, reactionType, shotSubtype);
          break;
        case WeaponKind.Shotgun:
          FirePunchEvent(WeaponEnum.Shotgun, weaponEntity, shotType, reactionType, shotSubtype);
          break;
      }
    };
    animator = GetComponent<Animator>();
    animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
  }

  public override Vector3 Velocity => characterController.velocity;

  public override void Push(Vector3 velocity, EnemyBase enemy)
  {
    base.Push(velocity, enemy);
    FireWasPushedEvent(enemy);
  }

  private void Reaction(PunchTypeEnum punchType, EnemyBase enemy, ReactionType reactionType)
  {
    CaclulateRection(enemy, reactionType, out float fightReactionX, out float fightReactionY);
    if (fightReactionY < 0.0)
      fightReactionY = 0.0f;
    animator.SetFloat("Fight.ReactionY", fightReactionY);
    animator.SetFloat("Fight.ReactionX", fightReactionX);
    animator.SetTrigger("Fight.Triggers/Reaction");
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
    weaponService.ReactionLayerWeight = num;
    weaponService.Reaction();
    animator.SetTrigger("Triggers/Reaction");
  }

  public override void Punch(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    Reaction(punchType, enemy, reactionType);
    pivot.PlaySound(Pivot.SoundEnum.HittedVocal, protagonist: true);
    if ((punchType == PunchTypeEnum.Strong || punchType == PunchTypeEnum.Moderate) && Random.value < 0.20000000298023224)
      enemy.PlayLipSync(CombatCryEnum.PunchUnblocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.Stab, protagonist: true);
        break;
      case WeaponEnum.Rifle:
      case WeaponEnum.Samopal:
        pivot.PlaySound(Pivot.SoundEnum.BulletHit);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted, protagonist: true);
        break;
    }
    FireWasPunchedEvent(enemy);
  }

  public override void PunchToBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    Reaction(punchType, enemy, reactionType);
    enemy.PlayLipSync(CombatCryEnum.PunchBlocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.StabBlock, protagonist: true);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.BlockHitted, protagonist: true);
        break;
    }
    FireWasPunchedToBlockEvent(enemy);
  }

  public override void PunchToQuickBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    Reaction(punchType, enemy, reactionType);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.StabBlock, protagonist: true);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.BlockHitted, protagonist: true);
        break;
    }
    FireWasPunchedToQuickBlockEvent(enemy);
  }

  public override void PunchToDodge(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    FireWasPunchedToDodgeEvent(enemy);
  }

  public override void PunchLowStamina(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    FireWasLowStaminaPunchedEvent(enemy);
  }

  public override void PunchToStagger(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    FireWasPunchedToStaggerEvent(enemy);
    Reaction(punchType, enemy, reactionType);
    pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.StabBlock, protagonist: true);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted, protagonist: true);
        break;
    }
  }

  public override void Stagger(EnemyBase enemy)
  {
    enemy.PlayLipSync(CombatCryEnum.SuperPunch);
    staggerTimeLeft = StaggerTime;
    animator.SetTrigger("Fight.Triggers/Stagger");
    pivot.PlaySound(Pivot.SoundEnum.StaggerVocal, protagonist: true);
    pivot.PlaySound(Pivot.SoundEnum.StaggerNonVocal, protagonist: true);
    pivot.PlaySound(Pivot.SoundEnum.BlockHitted, protagonist: true);
    FireWasStaggeredEvent(enemy);
  }

  public override void KnockDown(EnemyBase enemy)
  {
    enemy.PlayLipSync(CombatCryEnum.Catch);
    animator.SetTrigger("Triggers/KnockDown");
    FireWasKnockDownedEvent(enemy);
  }

  private void Update()
  {
    BlockType = BlockStance ? BlockTypeEnum.Block : BlockTypeEnum.NotBlocking;
    if (staggerTimeLeft <= 0.0)
      return;
    staggerTimeLeft -= Time.deltaTime;
    if (staggerTimeLeft <= 0.0)
      animator.SetTrigger("Fight.Triggers/CancelStagger");
    else
      BlockType = BlockTypeEnum.Stagger;
  }

  private new void FixedUpdate()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    base.FixedUpdate();
  }
}
