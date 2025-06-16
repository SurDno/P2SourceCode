using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
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
  private bool nonAdditiveReaction = false;
  private FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private AnimatorState45 animatorState;
  private float staggerTimeLeft;
  private float rotationSpeed = 270f;
  private bool isAiming;
  private float aimingDeltaRotation;
  private EnemyBase counterReactionEnemy;
  private bool canPlayReactionAnimation = true;
  private bool wasDodge = false;
  private EnemyBase prePunchEnemy;
  private List<float> punches = new List<float>();

  public float TimeFromLastHit { get; private set; }

  public override Vector3 Velocity => this.GetComponent<Rigidbody>().velocity;

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
      this.weaponService.TriggerAction(weaponAction);
    if (weaponAction != WeaponActionEnum.JabAttack && weaponAction != WeaponActionEnum.KnockDown && weaponAction != WeaponActionEnum.StepAttack && weaponAction != WeaponActionEnum.Uppercut)
      return;
    this.TimeFromLastHit = 0.0f;
  }

  public void TriggerThrowBomb(int range)
  {
    this.currentWalkSpeed = 0.0f;
    if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
      return;
    this.animator.SetFloat("Fight.WalkSpeed", 0.0f);
    this.animatorState.SetTrigger("Fight.Triggers/ThrowBomb");
    this.animator.SetInteger("Fight.ThrowRange", range);
  }

  private void OnWeaponShot(
    WeaponEnum weapon,
    IEntity entity,
    ShotType shotType,
    ReactionType reactionType)
  {
    this.FirePunchEvent(weapon, entity, shotType, reactionType);
  }

  [Inspected]
  public float AttackCooldownTimeLeft { get; set; }

  [Inspected]
  public EnemyBase CounterReactionEnemy
  {
    get => this.counterReactionEnemy;
    set => this.counterReactionEnemy = value;
  }

  [Inspected]
  public float QuickBlockProbability { get; set; }

  [Inspected]
  public float DodgeProbability { get; set; }

  [Inspected]
  public bool CanPlayReactionAnimation
  {
    get => this.canPlayReactionAnimation;
    set => this.canPlayReactionAnimation = value;
  }

  [Inspected]
  public EnemyBase PrePunchEnemy
  {
    get => this.prePunchEnemy;
    set => this.prePunchEnemy = value;
  }

  [Inspected]
  public float ContrReaction
  {
    get
    {
      return (UnityEngine.Object) this.animator != (UnityEngine.Object) null ? (float) this.animator.GetInteger("Fight.ContrReaction1") : 0.0f;
    }
    set
    {
      if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
        return;
      this.animator.SetFloat("Fight.ContrReaction", value);
      this.animator.SetInteger("Fight.ContrReaction1", (int) value);
    }
  }

  public bool IsReacting => this.fightAnimatorState != null && this.fightAnimatorState.IsReaction;

  public bool IsContrReacting => false;

  public bool IsAttacking => this.fightAnimatorState != null && this.fightAnimatorState.IsAttacking;

  public bool IsPushing => this.fightAnimatorState != null && this.fightAnimatorState.IsPushing;

  public bool IsQuickBlock
  {
    get => this.fightAnimatorState != null && this.fightAnimatorState.IsQuickBlock;
  }

  public override bool IsStagger
  {
    get => this.fightAnimatorState != null && this.fightAnimatorState.IsStagger;
  }

  public bool IsDodge => this.fightAnimatorState != null && this.fightAnimatorState.IsDodge;

  public bool IsSurrender => this.fightAnimatorState != null && this.fightAnimatorState.IsSurrender;

  public int GetPunchesCount(float perTime)
  {
    if (this.punches.Count == 0)
      return 0;
    int punchesCount = 0;
    for (int index = this.punches.Count - 1; index >= 0 && (double) this.punches[index] <= (double) perTime; --index)
      ++punchesCount;
    return punchesCount;
  }

  private void UpdatePunches()
  {
    int index = 0;
    while (index < this.punches.Count)
    {
      this.punches[index] += Time.deltaTime;
      if ((double) this.punches[index] > 120.0)
        this.punches.RemoveAt(index);
      else
        ++index;
    }
  }

  private void ApplyCurrentBlockStance()
  {
    if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
      return;
    this.animator.SetFloat("Fight.BlockStance", this.BlockStance ? 1f : 0.0f);
  }

  private void UpdateBlockStance()
  {
    if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
      return;
    this.animator.SetFloat("Fight.BlockStance", Mathf.MoveTowards(this.animator.GetFloat("Fight.BlockStance"), this.BlockStance ? 1f : 0.0f, Time.deltaTime / 0.5f));
  }

  private void Start()
  {
    if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
    {
      this.weaponService.WeaponShootEvent += new Action<WeaponEnum, IEntity, ShotType, ReactionType>(this.OnWeaponShot);
      this.weaponService.Weapon = WeaponEnum.Unknown;
    }
    this.fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
    {
      this.animator.SetLayerWeight(this.animator.GetLayerIndex("Fight Push Reaction Layer"), 1f);
      this.animator.SetBool("Setup/NonAdditiveReactions", this.nonAdditiveReaction);
    }
    this.StaggerTime = 5f;
  }

  private void Update()
  {
    if ((double) this.staggerTimeLeft > 0.0)
    {
      this.staggerTimeLeft -= Time.deltaTime;
      if ((double) this.staggerTimeLeft < 0.0)
      {
        this.animatorState.SetTrigger("Fight.Triggers/CancelStagger");
      }
      else
      {
        this.BlockType = BlockTypeEnum.Stagger;
        return;
      }
    }
    this.UpdatePunches();
    this.UpdateBlockStance();
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
      this.animator.SetFloat("Fight.Random", UnityEngine.Random.value);
    this.transform.rotation = Quaternion.Euler(0.0f, this.transform.rotation.eulerAngles.y, 0.0f);
    this.BlockType = BlockTypeEnum.NotBlocking;
    if (this.BlockStance)
      this.BlockType = BlockTypeEnum.Block;
    if (this.IsQuickBlock)
      this.BlockType = BlockTypeEnum.QuickBlock;
    if (this.IsDodge)
      this.BlockType = BlockTypeEnum.Dodge;
    if (this.IsSurrender)
      this.BlockType = BlockTypeEnum.Surrender;
    this.UpdateRotation();
    this.currentWalkSpeed = Mathf.MoveTowards(this.currentWalkSpeed, this.desiredWalkSpeed, Time.deltaTime / 0.25f);
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
      this.animator.SetFloat("Fight.WalkSpeed", this.currentWalkSpeed);
    this.AttackCooldownTimeLeft -= Time.deltaTime;
    this.TimeFromLastHit += Time.deltaTime;
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    this.fightAnimatorState?.Reset();
  }

  private void SetRotation(Quaternion newRotation)
  {
    this.transform.rotation = Quaternion.Euler(0.0f, newRotation.eulerAngles.y, 0.0f);
  }

  public float GetAimingRotationDelta() => this.aimingDeltaRotation;

  public void SetAiming(bool aiming)
  {
    this.isAiming = aiming;
    if (!aiming)
      return;
    this.aimingDeltaRotation = 0.0f;
  }

  private void UpdateRotation()
  {
    if (this.IsDead || InstanceByRequest<EngineApplication>.Instance.IsPaused || (UnityEngine.Object) this.RotationTarget == (UnityEngine.Object) null || this.fightAnimatorState.Condition == FightAnimatorBehavior.AnimatorState.NPCCondition.EscapeBegin || this.fightAnimatorState.IsStagger)
      return;
    float? retreatAngle = this.RetreatAngle;
    if (!retreatAngle.HasValue)
    {
      if (this.RotateByPath)
      {
        this.SetRotation(Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(this.agent.steeringTarget - this.transform.position), this.rotationSpeed * Time.deltaTime));
      }
      else
      {
        Vector3 forward = this.RotationTarget.position - this.transform.position;
        if (forward == Vector3.zero)
          return;
        float magnitude = forward.magnitude;
        forward.Normalize();
        Quaternion newRotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.LookRotation(forward), this.rotationSpeed * Time.deltaTime);
        if (this.isAiming)
          this.aimingDeltaRotation += Mathf.Abs(newRotation.eulerAngles.y - this.transform.rotation.eulerAngles.y);
        this.SetRotation(newRotation);
      }
    }
    else
    {
      Vector3 vector3 = this.RotationTarget.position - this.transform.position;
      if (vector3 == Vector3.zero)
        return;
      vector3.Normalize();
      Quaternion quaternion1 = Quaternion.LookRotation(-vector3);
      retreatAngle = this.RetreatAngle;
      if (retreatAngle.HasValue)
      {
        Quaternion quaternion2 = quaternion1;
        retreatAngle = this.RetreatAngle;
        Quaternion quaternion3 = Quaternion.AngleAxis(retreatAngle.Value, Vector3.up);
        quaternion1 = quaternion2 * quaternion3;
      }
      float rotationSpeed = this.rotationSpeed;
      float num1 = Mathf.InverseLerp(0.0f, 40f, Quaternion.Angle(this.transform.rotation, quaternion1));
      float num2 = Mathf.Lerp(0.0f, 1f, num1 * num1);
      float num3 = rotationSpeed * num2;
      if (this.fightAnimatorState.Condition == FightAnimatorBehavior.AnimatorState.NPCCondition.Escape)
        this.SetRotation(Quaternion.RotateTowards(this.transform.rotation, quaternion1, num3 * Time.deltaTime));
    }
  }

  public override void OnExternalAnimatorMove()
  {
    if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
      return;
    float num = Time.deltaTime;
    if (this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
      num = Time.fixedDeltaTime;
    if (this.agent.isActiveAndEnabled && this.agent.isOnNavMesh)
    {
      Vector3 vector3_1 = Vector3.zero;
      if ((UnityEngine.Object) this.Enemy != (UnityEngine.Object) null)
        vector3_1 = this.Enemy.CalculateRepulseVelocity((EnemyBase) this) * num;
      Vector3 vector3_2 = this.gameObject.transform.position + this.animator.deltaPosition + vector3_1;
      vector3_2.y = Mathf.MoveTowards(vector3_2.y, this.agent.nextPosition.y, num * 0.1f);
      this.agent.nextPosition = vector3_2;
      this.gameObject.transform.position = this.agent.nextPosition;
    }
    this.gameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * this.animator.angularVelocity.y * num, Vector3.up);
  }

  private void QuickBlock(EnemyBase enemy, ReactionType reactionType, WeaponEnum weapon)
  {
    if (this.CantBlock)
      return;
    float num = UnityEngine.Random.value;
    if (weapon == WeaponEnum.Hands || weapon == WeaponEnum.Flashlight)
    {
      if ((double) num < (double) this.DodgeProbability)
      {
        if (!this.Dodge(enemy))
          this.animatorState.SetTrigger("Fight.Triggers/QuickBlock");
      }
      else
        this.QuickBlockByHands(enemy);
    }
    else if (!this.Dodge(enemy))
      this.QuickBlockByHands(enemy);
    this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
  }

  private void QuickBlockByHands(EnemyBase enemy)
  {
    this.animatorState.SetTrigger("Fight.Triggers/QuickBlock");
  }

  private bool Dodge(EnemyBase enemy)
  {
    if (this.wasDodge)
    {
      this.wasDodge = false;
      return false;
    }
    float num = 1.5f;
    float y = Vector3.Cross(-enemy.transform.forward, this.transform.forward).y;
    bool flag1 = (double) y < 0.0;
    bool flag2 = PathfindingHelper.IsFreeSpace(this.transform.position, this.transform.position - this.transform.forward * num);
    if ((double) Mathf.Abs(y) < 0.3 & flag2)
    {
      this.animatorState.SetTrigger("Fight.Triggers/DodgeBack");
      this.pivot.PlaySound(Pivot.SoundEnum.StepBack);
      this.wasDodge = true;
      return true;
    }
    if (flag1)
    {
      if (PathfindingHelper.IsFreeSpace(this.transform.position, this.transform.position + this.transform.right * num))
      {
        this.animatorState.SetTrigger("Fight.Triggers/DodgeRight");
        this.pivot.PlaySound(Pivot.SoundEnum.StrafeRight);
        this.wasDodge = true;
        return true;
      }
    }
    else if (PathfindingHelper.IsFreeSpace(this.transform.position, this.transform.position - this.transform.right * num))
    {
      this.animatorState.SetTrigger("Fight.Triggers/DodgeLeft");
      this.pivot.PlaySound(Pivot.SoundEnum.StrafeLeft);
      this.wasDodge = true;
      return true;
    }
    if (flag2)
    {
      this.animatorState.SetTrigger("Fight.Triggers/DodgeBack");
      this.pivot.PlaySound(Pivot.SoundEnum.StepBack);
      this.wasDodge = true;
      return true;
    }
    this.wasDodge = false;
    return false;
  }

  private void Reaction(PunchTypeEnum punchType, EnemyBase enemy, ReactionType reactionType)
  {
    if (this.IsStagger)
      this.animatorState.SetTrigger("Fight.Triggers/CancelStagger");
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
    this.ReactionBase(reactionLayerWeight, enemy, reactionType);
  }

  private void ReactionBase(float reactionLayerWeight, EnemyBase enemy, ReactionType reactionType)
  {
    this.ApplyCurrentBlockStance();
    if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
      this.weaponService.ReactionLayerWeight = reactionLayerWeight;
    float fightReactionX;
    float fightReactionY;
    this.CaclulateRection(enemy, reactionType, out fightReactionX, out fightReactionY);
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
    {
      this.animator.SetFloat("Fight.ReactionY", fightReactionY);
      this.animator.SetFloat("Fight.ReactionX", fightReactionX);
    }
    if (this.animatorState != null && this.CanPlayReactionAnimation)
      this.animatorState.SetTrigger("Fight.Triggers/Reaction");
    if ((bool) (UnityEngine.Object) this.pivot)
      this.pivot.RagdollApplyImpulseToHead(25f * this.transform.TransformVector(new Vector3(-fightReactionX, 0.0f, -fightReactionY)));
    this.weaponService.PunchReaction(reactionType);
  }

  private void ReactionBaseEnemy(
    EnemyBase enemy,
    ReactionType reactionType,
    out float fightReactionX,
    out float fightReactionY)
  {
    Vector3 normalized = (enemy.transform.position - this.transform.position).normalized;
    float y = Vector3.Cross(normalized, this.transform.forward).y;
    if ((double) Vector3.Dot(normalized, this.transform.forward) > 0.0)
    {
      if ((double) Mathf.Abs(y) < (double) Mathf.Sin(0.87266463f))
      {
        fightReactionX = 0.0f;
        fightReactionY = 1f;
      }
      else
      {
        bool flag = (double) y < 0.0;
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
    if (this.IsCombatIgnored || (UnityEngine.Object) this.weaponService == (UnityEngine.Object) null || this.weaponService.Weapon == WeaponEnum.Unknown)
      return;
    this.FireWasWasPrepunchEvent();
    if (this.BlockStance || this.fightAnimatorState.IsCheating || this.fightAnimatorState.IsAttacking || this.IsQuickBlock)
      return;
    this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    this.prePunchEnemy = enemy;
    if ((double) UnityEngine.Random.value >= (double) this.QuickBlockProbability)
      return;
    this.QuickBlock(enemy, reactionType, weapon);
  }

  public override void PrepunchUppercut(
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || (UnityEngine.Object) this.weaponService == (UnityEngine.Object) null || this.weaponService.Weapon == WeaponEnum.Unknown || this.IsSurrender || this.BlockStance || this.fightAnimatorState.IsCheating || this.fightAnimatorState.IsAttacking || this.IsQuickBlock)
      return;
    this.animatorState.ResetAllTriggers();
    this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    this.prePunchEnemy = enemy;
    if ((double) UnityEngine.Random.value >= (double) this.QuickBlockProbability * (double) this.DodgeProbability)
      return;
    this.Dodge(enemy);
  }

  public override void Punch(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    if (this.IsAttacking)
    {
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    this.FireWasPunchedEvent(enemy);
    base.Punch(punchType, reactionType, weapon, enemy);
    this.punches.Add(0.0f);
    this.pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    this.PlayLipSync(CombatCryEnum.Hurt);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      case WeaponEnum.Revolver:
      case WeaponEnum.Rifle:
      case WeaponEnum.Samopal:
      case WeaponEnum.Shotgun:
        this.pivot.PlaySound(Pivot.SoundEnum.BulletHit);
        break;
      case WeaponEnum.Lockpick:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 1.5f);
        break;
    }
    this.Reaction(punchType, enemy, reactionType);
  }

  public override void PunchToSurrender(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored)
      return;
    this.FireWasPunchedToSurrenderEvent(enemy);
    base.PunchToSurrender(punchType, reactionType, weapon, enemy);
    this.punches.Add(0.0f);
    this.pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      case WeaponEnum.Lockpick:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 1.5f);
        break;
    }
    this.Reaction(punchType, enemy, reactionType);
  }

  public override void PunchToBlock(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    if (this.IsAttacking)
    {
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    this.Reaction(punchType, enemy, reactionType);
    this.PlayLipSync(CombatCryEnum.Blocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.StabBlock);
        break;
      case WeaponEnum.Lockpick:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 0.66f);
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
    if (this.IsAttacking)
    {
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    this.PlayLipSync(CombatCryEnum.Blocked);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.StabBlock);
        break;
      case WeaponEnum.Lockpick:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted, 0.66f);
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
    if (this.IsAttacking)
    {
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    this.Reaction(PunchTypeEnum.Light, enemy, reactionType);
    this.pivot.PlaySound(Pivot.SoundEnum.HittedDodgeVocal, 0.66f);
    this.PlayLipSync(CombatCryEnum.HurtDodge);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab, 0.33f);
        break;
      case WeaponEnum.Lockpick:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted);
        break;
    }
    this.FireWasPunchedToDodgeEvent(enemy);
  }

  public override void PunchToStagger(
    PunchTypeEnum punchType,
    ReactionType reactionType,
    WeaponEnum weapon,
    EnemyBase enemy)
  {
    if (this.IsCombatIgnored || this.IsDead || this.IsFaint)
      return;
    if (this.IsAttacking)
    {
      this.animatorState.ResetAllTriggers();
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    }
    this.FireWasPunchedToStaggerEvent(enemy);
    this.punches.Add(0.0f);
    this.pivot.PlaySound(Pivot.SoundEnum.HittedVocal);
    switch (weapon)
    {
      case WeaponEnum.Knife:
      case WeaponEnum.Scalpel:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      case WeaponEnum.Lockpick:
        this.pivot.PlaySound(Pivot.SoundEnum.Stab);
        break;
      default:
        this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted);
        break;
    }
    this.Reaction(punchType, enemy, reactionType);
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

  public override void Stagger(EnemyBase enemy)
  {
    if (this.IsCombatIgnored)
      return;
    this.staggerTimeLeft = this.StaggerTime;
    if (this.animatorState != null)
    {
      this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      this.animatorState.SetTrigger("Fight.Triggers/Stagger");
    }
    if ((UnityEngine.Object) this.pivot != (UnityEngine.Object) null)
    {
      this.pivot.PlaySound(Pivot.SoundEnum.StaggerVocal);
      this.pivot.PlaySound(Pivot.SoundEnum.BlockHitted);
      this.pivot.PlaySound(Pivot.SoundEnum.FaceHitted);
    }
    if (!((UnityEngine.Object) enemy != (UnityEngine.Object) null))
      return;
    this.FireWasStaggeredEvent(enemy);
  }

  public override void Push(Vector3 velocity, EnemyBase enemy)
  {
    if (this.IsDead || this.IsFaint)
      return;
    this.animatorState.SetTrigger("Fight.Triggers/CancelAttack");
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
    {
      this.animator.SetFloat("Fight.ReactionY", 1f);
      this.animator.SetFloat("Fight.ReactionX", 0.0f);
    }
    if (this.CanPlayReactionAnimation)
      this.animatorState.SetTrigger("Fight.Triggers/Reaction");
    this.FireWasPushedEvent(enemy);
  }

  public override void PushMove(Vector3 direction)
  {
    this.gameObject.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
  }

  public void Fire() => this.animatorState.SetTrigger("Fight.Triggers/Fire");
}
