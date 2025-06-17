using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

[RequireComponent(typeof (NPCWeaponService))]
[RequireComponent(typeof (Pivot))]
[DisallowMultipleComponent]
public class NPCEnemy : EnemyBase
{
  private const float PunchCollectionTime = 120f;
  [Header("You can use ComputeNpc to fill this!")]
  [SerializeField]
  protected Animator animator;
  [SerializeField]
  private AnimatorEventProxy animatorEventProxy;
  [SerializeField]
  private NPCWeaponService weaponService;
  [Header("Other")]
  [Tooltip("Обычно реакции на удары накладываются аддитивно, чтобы проигрываться поверх боевых. Но для персонажей, которые не дерутся могут применяться неаддитивные анимации реакций. В этих анимациях возможны сильные смещения.")]
  [SerializeField]
  private bool nonAdditiveReaction;
  private FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private AnimatorState45 animatorState;
  private float staggerTimeLeft;
  private float rotationSpeed = 270f;
  private bool isAiming;
  private float aimingDeltaRotation;
  private EnemyBase counterReactionEnemy;
  private bool canPlayReactionAnimation = true;
  private bool wasDodge;
  private EnemyBase prePunchEnemy;
  private List<float> punches = [];

  public float TimeFromLastHit { get; private set; }

  public override Vector3 Velocity => GetComponent<Rigidbody>().velocity;

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    if (weaponService != null)
      weaponService.TriggerAction(weaponAction);
    if (weaponAction != WeaponActionEnum.JabAttack && weaponAction != WeaponActionEnum.KnockDown && weaponAction != WeaponActionEnum.StepAttack && weaponAction != WeaponActionEnum.Uppercut)
      return;
    TimeFromLastHit = 0.0f;
  }

  public void TriggerThrowBomb(int range)
  {
    currentWalkSpeed = 0.0f;
    if (!(animator != null))
      return;
    animator.SetFloat("Fight.WalkSpeed", 0.0f);
    animatorState.SetTrigger("Fight.Triggers/ThrowBomb");
    animator.SetInteger("Fight.ThrowRange", range);
  }

  private void OnWeaponShot(
    WeaponEnum weapon,
    IEntity entity,
    ShotType shotType,
    ReactionType reactionType)
  {
    FirePunchEvent(weapon, entity, shotType, reactionType);
  }

  [Inspected]
  public float AttackCooldownTimeLeft { get; set; }

  [Inspected]
  public EnemyBase CounterReactionEnemy
  {
    get => counterReactionEnemy;
    set => counterReactionEnemy = value;
  }

  [Inspected]
  public float QuickBlockProbability { get; set; }

  [Inspected]
  public float DodgeProbability { get; set; }

  [Inspected]
  public bool CanPlayReactionAnimation
  {
    get => canPlayReactionAnimation;
    set => canPlayReactionAnimation = value;
  }

  [Inspected]
  public EnemyBase PrePunchEnemy
  {
    get => prePunchEnemy;
    set => prePunchEnemy = value;
  }

  [Inspected]
  public float ContrReaction
  {
    get => animator != null ? animator.GetInteger("Fight.ContrReaction1") : 0.0f;
    set
    {
      if (!(animator != null))
        return;
      animator.SetFloat("Fight.ContrReaction", value);
      animator.SetInteger("Fight.ContrReaction1", (int) value);
    }
  }

  public bool IsReacting => fightAnimatorState != null && fightAnimatorState.IsReaction;

  public bool IsContrReacting => false;

  public bool IsAttacking => fightAnimatorState != null && fightAnimatorState.IsAttacking;

  public bool IsPushing => fightAnimatorState != null && fightAnimatorState.IsPushing;

  public bool IsQuickBlock => fightAnimatorState != null && fightAnimatorState.IsQuickBlock;

  public override bool IsStagger => fightAnimatorState != null && fightAnimatorState.IsStagger;

  public bool IsDodge => fightAnimatorState != null && fightAnimatorState.IsDodge;

  public bool IsSurrender => fightAnimatorState != null && fightAnimatorState.IsSurrender;

  public int GetPunchesCount(float perTime)
  {
    if (punches.Count == 0)
      return 0;
    int punchesCount = 0;
    for (int index = punches.Count - 1; index >= 0 && punches[index] <= (double) perTime; --index)
      ++punchesCount;
    return punchesCount;
  }

  private void UpdatePunches()
  {
    int index = 0;
    while (index < punches.Count)
    {
      punches[index] += Time.deltaTime;
      if (punches[index] > 120.0)
        punches.RemoveAt(index);
      else
        ++index;
    }
  }

  private void ApplyCurrentBlockStance()
  {
    if (!(animator != null))
      return;
    animator.SetFloat("Fight.BlockStance", BlockStance ? 1f : 0.0f);
  }

  private void UpdateBlockStance()
  {
    if (!(animator != null))
      return;
    animator.SetFloat("Fight.BlockStance", Mathf.MoveTowards(animator.GetFloat("Fight.BlockStance"), BlockStance ? 1f : 0.0f, Time.deltaTime / 0.5f));
  }

  private void Start()
  {
    if (weaponService != null)
    {
      weaponService.WeaponShootEvent += OnWeaponShot;
      weaponService.Weapon = WeaponEnum.Unknown;
    }
    fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    animatorState = AnimatorState45.GetAnimatorState(animator);
    if (animator != null)
    {
      animator.SetLayerWeight(animator.GetLayerIndex("Fight Push Reaction Layer"), 1f);
      animator.SetBool("Setup/NonAdditiveReactions", nonAdditiveReaction);
    }
    StaggerTime = 5f;
  }

  private void Update()
  {
    if (staggerTimeLeft > 0.0)
    {
      staggerTimeLeft -= Time.deltaTime;
      if (staggerTimeLeft < 0.0)
      {
        animatorState.SetTrigger("Fight.Triggers/CancelStagger");
      }
      else
      {
        BlockType = BlockTypeEnum.Stagger;
        return;
      }
    }
    UpdatePunches();
    UpdateBlockStance();
    if (animator != null)
      animator.SetFloat("Fight.Random", Random.value);
    transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    BlockType = BlockTypeEnum.NotBlocking;
    if (BlockStance)
      BlockType = BlockTypeEnum.Block;
    if (IsQuickBlock)
      BlockType = BlockTypeEnum.QuickBlock;
    if (IsDodge)
      BlockType = BlockTypeEnum.Dodge;
    if (IsSurrender)
      BlockType = BlockTypeEnum.Surrender;
    UpdateRotation();
    currentWalkSpeed = Mathf.MoveTowards(currentWalkSpeed, desiredWalkSpeed, Time.deltaTime / 0.25f);
    if (animator != null)
      animator.SetFloat("Fight.WalkSpeed", currentWalkSpeed);
    AttackCooldownTimeLeft -= Time.deltaTime;
    TimeFromLastHit += Time.deltaTime;
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    fightAnimatorState?.Reset();
  }

  private void SetRotation(Quaternion newRotation)
  {
    transform.rotation = Quaternion.Euler(0.0f, newRotation.eulerAngles.y, 0.0f);
  }

  public float GetAimingRotationDelta() => aimingDeltaRotation;

  public void SetAiming(bool aiming)
  {
    isAiming = aiming;
    if (!aiming)
      return;
    aimingDeltaRotation = 0.0f;
  }

  private void UpdateRotation()
  {
    if (IsDead || InstanceByRequest<EngineApplication>.Instance.IsPaused || RotationTarget == null || fightAnimatorState.Condition == FightAnimatorBehavior.AnimatorState.NPCCondition.EscapeBegin || fightAnimatorState.IsStagger)
      return;
    float? retreatAngle = RetreatAngle;
    if (!retreatAngle.HasValue)
    {
      if (RotateByPath)
      {
        SetRotation(Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(agent.steeringTarget - transform.position), rotationSpeed * Time.deltaTime));
      }
      else
      {
        Vector3 forward = RotationTarget.position - transform.position;
        if (forward == Vector3.zero)
          return;
        float magnitude = forward.magnitude;
        forward.Normalize();
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.LookRotation(forward), rotationSpeed * Time.deltaTime);
        if (isAiming)
          aimingDeltaRotation += Mathf.Abs(newRotation.eulerAngles.y - transform.rotation.eulerAngles.y);
        SetRotation(newRotation);
      }
    }
    else
    {
      Vector3 vector3 = RotationTarget.position - transform.position;
      if (vector3 == Vector3.zero)
        return;
      vector3.Normalize();
      Quaternion quaternion1 = Quaternion.LookRotation(-vector3);
      retreatAngle = RetreatAngle;
      if (retreatAngle.HasValue)
      {
        Quaternion quaternion2 = quaternion1;
        retreatAngle = RetreatAngle;
        Quaternion quaternion3 = Quaternion.AngleAxis(retreatAngle.Value, Vector3.up);
        quaternion1 = quaternion2 * quaternion3;
      }
      float rotationSpeed = this.rotationSpeed;
      float num1 = Mathf.InverseLerp(0.0f, 40f, Quaternion.Angle(transform.rotation, quaternion1));
      float num2 = Mathf.Lerp(0.0f, 1f, num1 * num1);
      float num3 = rotationSpeed * num2;
      if (fightAnimatorState.Condition == FightAnimatorBehavior.AnimatorState.NPCCondition.Escape)
        SetRotation(Quaternion.RotateTowards(transform.rotation, quaternion1, num3 * Time.deltaTime));
    }
  }

  public override void OnExternalAnimatorMove()
  {
    if (animator == null)
      return;
    float num = Time.deltaTime;
    if (animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
      num = Time.fixedDeltaTime;
    if (agent.isActiveAndEnabled && agent.isOnNavMesh)
    {
      Vector3 vector3_1 = Vector3.zero;
      if (Enemy != null)
        vector3_1 = Enemy.CalculateRepulseVelocity(this) * num;
      Vector3 vector3_2 = gameObject.transform.position + animator.deltaPosition + vector3_1;
      vector3_2.y = Mathf.MoveTowards(vector3_2.y, agent.nextPosition.y, num * 0.1f);
      agent.nextPosition = vector3_2;
      gameObject.transform.position = agent.nextPosition;
    }
    gameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * animator.angularVelocity.y * num, Vector3.up);
  }

  private void QuickBlock(EnemyBase enemy, ReactionType reactionType, WeaponEnum weapon)
  {
    if (CantBlock)
      return;
    float num = Random.value;
    if (weapon == WeaponEnum.Hands || weapon == WeaponEnum.Flashlight)
    {
      if (num < (double) DodgeProbability)
      {
        if (!Dodge(enemy))
          animatorState.SetTrigger("Fight.Triggers/QuickBlock");
      }
      else
        QuickBlockByHands(enemy);
    }
    else if (!Dodge(enemy))
      QuickBlockByHands(enemy);
    animatorState.SetTrigger("Fight.Triggers/CancelAttack");
  }

  private void QuickBlockByHands(EnemyBase enemy)
  {
    animatorState.SetTrigger("Fight.Triggers/QuickBlock");
  }

  private bool Dodge(EnemyBase enemy)
  {
    if (wasDodge)
    {
      wasDodge = false;
      return false;
    }
    float num = 1.5f;
    float y = Vector3.Cross(-enemy.transform.forward, transform.forward).y;
    bool flag1 = y < 0.0;
    bool flag2 = PathfindingHelper.IsFreeSpace(transform.position, transform.position - transform.forward * num);
    if (Mathf.Abs(y) < 0.3 & flag2)
    {
      animatorState.SetTrigger("Fight.Triggers/DodgeBack");
      pivot.PlaySound(Pivot.SoundEnum.StepBack);
      wasDodge = true;
      return true;
    }
    if (flag1)
    {
      if (PathfindingHelper.IsFreeSpace(transform.position, transform.position + transform.right * num))
      {
        animatorState.SetTrigger("Fight.Triggers/DodgeRight");
        pivot.PlaySound(Pivot.SoundEnum.StrafeRight);
        wasDodge = true;
        return true;
      }
    }
    else if (PathfindingHelper.IsFreeSpace(transform.position, transform.position - transform.right * num))
    {
      animatorState.SetTrigger("Fight.Triggers/DodgeLeft");
      pivot.PlaySound(Pivot.SoundEnum.StrafeLeft);
      wasDodge = true;
      return true;
    }
    if (flag2)
    {
      animatorState.SetTrigger("Fight.Triggers/DodgeBack");
      pivot.PlaySound(Pivot.SoundEnum.StepBack);
      wasDodge = true;
      return true;
    }
    wasDodge = false;
    return false;
  }

  private void Reaction(PunchTypeEnum punchType, EnemyBase enemy, ReactionType reactionType)
  {
    if (IsStagger)
      animatorState.SetTrigger("Fight.Triggers/CancelStagger");
    float reactionLayerWeight;
    switch (punchType)
    {
      case PunchTypeEnum.Light:
        reactionLayerWeight = 0.4f;
        break;
      case PunchTypeEnum.Moderate:
        reactionLayerWeight = 0.8f;
        break;
      case PunchTypeEnum.Strong:
        reactionLayerWeight = 1f;
        break;
      default:
        reactionLayerWeight = 0.2f;
        break;
    }
    ReactionBase(reactionLayerWeight, enemy, reactionType);
  }

  private void ReactionBase(float reactionLayerWeight, EnemyBase enemy, ReactionType reactionType)
  {
    ApplyCurrentBlockStance();
    if (weaponService != null)
      weaponService.ReactionLayerWeight = reactionLayerWeight;
    CaclulateRection(enemy, reactionType, out float fightReactionX, out float fightReactionY);
    if (animator != null)
    {
      animator.SetFloat("Fight.ReactionY", fightReactionY);
      animator.SetFloat("Fight.ReactionX", fightReactionX);
    }
    if (animatorState != null && CanPlayReactionAnimation)
      animatorState.SetTrigger("Fight.Triggers/Reaction");
    if ((bool) (Object) pivot)
      pivot.RagdollApplyImpulseToHead(25f * transform.TransformVector(new Vector3(-fightReactionX, 0.0f, -fightReactionY)));
    weaponService.PunchReaction(reactionType);
  }

  private void ReactionBaseEnemy(
    EnemyBase enemy,
    ReactionType reactionType,
    out float fightReactionX,
    out float fightReactionY)
  {
    Vector3 normalized = (enemy.transform.position - transform.position).normalized;
    float y = Vector3.Cross(normalized, transform.forward).y;
    if (Vector3.Dot(normalized, transform.forward) > 0.0)
    {
      if (Mathf.Abs(y) < (double) Mathf.Sin(0.87266463f))
      {
        fightReactionX = 0.0f;
        fightReactionY = 1f;
      }
      else
      {
        bool flag = y < 0.0;
        fightReactionX = flag ? 1f : -1f;
        fightReactionY = 0.0f;
      }
    }
    else
    {
      fightReactionX = 0.0f;
      fightReactionY = -1f;
    }
  }

  public override void Prepunch(ReactionType reactionType, WeaponEnum weapon, EnemyBase enemy)
  {
    if (IsCombatIgnored || weaponService == null || weaponService.Weapon == WeaponEnum.Unknown)
      return;
    FireWasWasPrepunchEvent();
    if (BlockStance || fightAnimatorState.IsCheating || fightAnimatorState.IsAttacking || IsQuickBlock)
      return;
    animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    prePunchEnemy = enemy;
    if (Random.value >= (double) QuickBlockProbability)
      return;
    QuickBlock(enemy, reactionType, weapon);
  }

  public override void PrepunchUppercut(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || weaponService == null || weaponService.Weapon == WeaponEnum.Unknown || IsSurrender || BlockStance || fightAnimatorState.IsCheating || fightAnimatorState.IsAttacking || IsQuickBlock)
      return;
    animatorState.ResetAllTriggers();
    animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    prePunchEnemy = enemy;
    if (Random.value >= QuickBlockProbability * (double) DodgeProbability)
      return;
    Dodge(enemy);
  }

  public override void Punch(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    if (IsAttacking)
    {
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    FireWasPunchedEvent(enemy);
    base.Punch(punchType, reactionType, weapon, enemy);
    punches.Add(0.0f);
    pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    PlayLipSync(CombatCryEnum.Hurt);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      case WeaponEnum.Revolver:
      case WeaponEnum.Rifle:
      case WeaponEnum.Samopal:
      case WeaponEnum.Shotgun:
        pivot.PlaySound(Pivot.SoundEnum.BulletHit);
        break;
      case WeaponEnum.Lockpick:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 1.5f);
        break;
    }
    Reaction(punchType, enemy, reactionType);
  }

  public override void PunchToSurrender(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored)
      return;
    FireWasPunchedToSurrenderEvent(enemy);
    base.PunchToSurrender(punchType, reactionType, weapon, enemy);
    punches.Add(0.0f);
    pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      case WeaponEnum.Lockpick:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 1.5f);
        break;
    }
    Reaction(punchType, enemy, reactionType);
  }

  public override void PunchToBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    if (IsAttacking)
    {
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    Reaction(punchType, enemy, reactionType);
    PlayLipSync(CombatCryEnum.Blocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.StabBlock);
        break;
      case WeaponEnum.Lockpick:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 0.66f);
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
    if (IsAttacking)
    {
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    PlayLipSync(CombatCryEnum.Blocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.StabBlock);
        break;
      case WeaponEnum.Lockpick:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 0.66f);
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
    if (IsAttacking)
    {
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    Reaction(PunchTypeEnum.Light, enemy, reactionType);
    pivot.PlaySound(Pivot.SoundEnum.HittedDodgeVocal, 0.66f);
    PlayLipSync(CombatCryEnum.HurtDodge);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.Stab, 0.33f);
        break;
      case WeaponEnum.Lockpick:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted);
        break;
    }
    FireWasPunchedToDodgeEvent(enemy);
  }

  public override void PunchToStagger(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (IsCombatIgnored || IsDead || IsFaint)
      return;
    if (IsAttacking)
    {
      animatorState.ResetAllTriggers();
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    FireWasPunchedToStaggerEvent(enemy);
    punches.Add(0.0f);
    pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      case WeaponEnum.Lockpick:
        pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        pivot.PlaySound(Pivot.SoundEnum.FaceHitted);
        break;
    }
    Reaction(punchType, enemy, reactionType);
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

  public override void Stagger(EnemyBase enemy)
  {
    if (IsCombatIgnored)
      return;
    staggerTimeLeft = StaggerTime;
    if (animatorState != null)
    {
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      animatorState.SetTrigger("Fight.Triggers/Stagger");
    }
    if (pivot != null)
    {
      pivot.PlaySound(Pivot.SoundEnum.StaggerVocal);
      pivot.PlaySound(Pivot.SoundEnum.BlockHitted);
      pivot.PlaySound(Pivot.SoundEnum.FaceHitted);
    }
    if (!(enemy != null))
      return;
    FireWasStaggeredEvent(enemy);
  }

  public override void Push(Vector3 velocity, EnemyBase enemy)
  {
    if (IsDead || IsFaint)
      return;
    animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    if (animator != null)
    {
      animator.SetFloat("Fight.ReactionY", 1f);
      animator.SetFloat("Fight.ReactionX", 0.0f);
    }
    if (CanPlayReactionAnimation)
      animatorState.SetTrigger("Fight.Triggers/Reaction");
    FireWasPushedEvent(enemy);
  }

  public override void PushMove(Vector3 direction)
  {
    gameObject.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
  }

  public void Fire() => animatorState.SetTrigger("Fight.Triggers/Fire");
}
