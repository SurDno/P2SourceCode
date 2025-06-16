using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components.Utilities;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Преследовать противника и атаковать по возможности")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightFollow))]
  public class MeleeFightFollow : MeleeFightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat followTime = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Aim;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private FollowDescription description;
    private float desiredWalkSpeed;
    private NPCEnemy fighter;
    private Vector3 lastPlayerPosition;
    private float attackCooldownTime;
    private float playerKnockdownCooldownLeft;

    private bool IsEnemyRunningAway()
    {
      return (double) this.owner.Enemy.Velocity.magnitude >= 0.5 && (double) Vector3.Dot(this.transform.forward, (this.owner.Enemy.transform.position - this.owner.transform.position).normalized) > 0.25;
    }

    public override void OnStart()
    {
      base.OnStart();
      this.fighter = this.owner as NPCEnemy;
      this.waitDuration = this.followTime.Value;
      this.desiredWalkSpeed = 0.0f;
      this.agent.enabled = true;
      this.npcState.FightIdle(this.Aim.Value);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((double) this.followTime.Value > 0.0 && (double) this.startTime + (double) this.waitDuration < (double) Time.time)
        return TaskStatus.Success;
      if ((UnityEngine.Object) this.description == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (typeof (MeleeFightFollow).Name + " has no " + typeof (FollowDescription).Name + " attached"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      this.UpdatePath();
      this.owner.RotationTarget = (Transform) null;
      this.owner.RotateByPath = false;
      this.owner.RetreatAngle = new float?();
      if ((bool) (UnityEngine.Object) this.fighter && (this.fighter.IsReacting || this.fighter.IsQuickBlock))
      {
        if (this.fighter.IsContrReacting && (UnityEngine.Object) this.fighter.CounterReactionEnemy != (UnityEngine.Object) null)
          this.owner.RotationTarget = this.fighter.CounterReactionEnemy.transform;
        else if (this.fighter.IsQuickBlock && (UnityEngine.Object) this.fighter.PrePunchEnemy != (UnityEngine.Object) null)
          this.owner.RotationTarget = this.fighter.PrePunchEnemy.transform;
        return TaskStatus.Running;
      }
      Vector3 lhs = this.owner.Enemy.transform.position - this.owner.transform.position;
      float magnitude = lhs.magnitude;
      lhs.Normalize();
      if ((bool) (UnityEngine.Object) this.fighter && (this.fighter.IsAttacking || this.fighter.IsContrReacting))
      {
        this.owner.RotationTarget = this.owner.Enemy.transform;
        return TaskStatus.Running;
      }
      float num = Vector3.Dot(lhs, this.owner.Enemy.Velocity);
      if (NavMesh.Raycast(this.owner.transform.position, this.owner.Enemy.transform.position, out NavMeshHit _, -1))
      {
        if (!this.agent.hasPath)
          return TaskStatus.Running;
        if ((double) this.agent.remainingDistance > (double) this.description.StopDistance)
        {
          this.desiredWalkSpeed = (double) num <= 1.0 ? (!this.IsEnemyRunningAway() ? ((double) this.agent.remainingDistance > (double) this.description.RunDistance ? 2f : 1f) : 2f) : 2f;
          this.owner.RotationTarget = this.owner.Enemy.transform;
          this.owner.RotateByPath = true;
          this.owner.RetreatAngle = new float?();
        }
      }
      else if ((double) magnitude > (double) this.description.StopDistance)
      {
        this.desiredWalkSpeed = (double) num <= 1.0 ? (!this.IsEnemyRunningAway() ? ((double) this.agent.remainingDistance > (double) this.description.RunDistance ? 2f : 1f) : 2f) : 2f;
        this.owner.RotationTarget = this.owner.Enemy.transform;
      }
      else
      {
        this.desiredWalkSpeed = 0.0f;
        if ((bool) (UnityEngine.Object) this.fighter && (this.fighter.IsContrReacting || !this.fighter.IsReacting))
          this.owner.RotationTarget = this.owner.Enemy.transform;
        this.animator.GetInteger("Fight.AttackType");
        if (this.fightAnimatorState.IsAttacking)
          this.owner.RotationTarget = this.owner.Enemy.transform;
      }
      this.owner.DesiredWalkSpeed = this.desiredWalkSpeed;
      this.agent.nextPosition = this.animator.rootPosition;
      if (this.owner.Enemy is PlayerEnemy && this.UpdatePlayerKnockDdown(this.owner.Enemy as PlayerEnemy) || (double) magnitude >= (double) this.description.AttackDistance)
        return TaskStatus.Running;
      if (this.owner.Enemy is PlayerEnemy && (double) this.owner.Enemy.BlockNormalizedTime > (double) this.description.PushIfBlockTimeMoreThan && !this.fightAnimatorState.IsPushing && !this.fightAnimatorState.IsReaction && !this.fightAnimatorState.IsAttacking)
      {
        if ((double) UnityEngine.Random.value < 0.5)
        {
          this.animatorState.SetTrigger("Fight.Triggers/Push");
        }
        else
        {
          this.animatorState.SetTrigger("Fight.Triggers/Attack");
          this.animator.SetInteger("Fight.AttackType", 8);
        }
        return TaskStatus.Running;
      }
      if (!this.fightAnimatorState.IsAttacking && !this.fightAnimatorState.IsPushing)
        this.attackCooldownTime -= deltaTime;
      if ((double) this.attackCooldownTime <= 0.0 && !this.fightAnimatorState.IsAttacking && !this.fightAnimatorState.IsPushing && !this.fightAnimatorState.IsReaction && !this.fightAnimatorState.IsQuickBlock)
      {
        if ((double) this.desiredWalkSpeed > 0.5)
        {
          this.animatorState.SetTrigger("Fight.Triggers/RunPunch");
          this.attackCooldownTime = 2f;
        }
        else if ((double) magnitude < 0.60000002384185791 * (double) Fight.Description.NPCHitDistance)
        {
          this.animatorState.SetTrigger("Fight.Triggers/Attack");
          this.animator.SetInteger("Fight.AttackType", 0);
          this.attackCooldownTime = this.description.PunchCooldownTime;
        }
        else if ((double) magnitude < 1.0 * (double) Fight.Description.NPCHitDistance)
        {
          this.animatorState.SetTrigger("Fight.Triggers/Attack");
          this.animator.SetInteger("Fight.AttackType", 1);
          this.attackCooldownTime = this.description.StepPunchCooldownTime;
        }
        else if ((double) magnitude < (double) this.description.AttackDistance)
        {
          this.animatorState.SetTrigger("Fight.Triggers/Attack");
          this.animator.SetInteger("Fight.AttackType", 3);
          this.animator.SetBool("Fight.AttackAfterCheat", (double) UnityEngine.Random.value > (double) this.description.CheatProbability);
          this.attackCooldownTime = this.description.TelegraphPunchCooldownTime;
        }
      }
      return TaskStatus.Running;
    }

    private void UpdatePath()
    {
      if ((double) (this.lastPlayerPosition - this.owner.Enemy.transform.position).magnitude <= 0.33000001311302185)
        return;
      if (!this.agent.isOnNavMesh)
        this.agent.Warp(this.transform.position);
      if (this.agent.isOnNavMesh)
        this.agent.destination = this.owner.Enemy.transform.position;
      NavMeshUtility.DrawPath(this.agent);
      this.lastPlayerPosition = this.owner.Enemy.transform.position;
    }

    private bool UpdatePlayerKnockDdown(PlayerEnemy player)
    {
      this.playerKnockdownCooldownLeft -= Time.fixedDeltaTime;
      if ((double) this.playerKnockdownCooldownLeft > 0.0)
        return false;
      Vector3 vector3 = player.transform.position - this.owner.transform.position;
      float magnitude = vector3.magnitude;
      Vector3 lhs = vector3 / magnitude;
      if ((double) magnitude > 5.0 || (double) Vector3.Dot(lhs, player.transform.forward) < 0.0 || (double) magnitude >= 3.0)
        return false;
      this.fighter.TriggerAction(WeaponActionEnum.KnockDown);
      this.playerKnockdownCooldownLeft = this.description.KnockDownCooldownTime;
      return true;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "FollowTime", this.followTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Aim", this.Aim);
      BehaviorTreeDataWriteUtility.WriteUnity<FollowDescription>(writer, "Description", this.description);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.followTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "FollowTime", this.followTime);
      this.Aim = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Aim", this.Aim);
      this.description = BehaviorTreeDataReadUtility.ReadUnity<FollowDescription>(reader, "Description", this.description);
    }
  }
}
