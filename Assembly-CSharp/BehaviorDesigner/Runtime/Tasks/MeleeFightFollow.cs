using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components.Utilities;
using Scripts.Tools.Serializations.Converters;

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
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat followTime = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool Aim;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private FollowDescription description;
    private float desiredWalkSpeed;
    private NPCEnemy fighter;
    private Vector3 lastPlayerPosition;
    private float attackCooldownTime;
    private float playerKnockdownCooldownLeft;

    private bool IsEnemyRunningAway()
    {
      return (double) owner.Enemy.Velocity.magnitude >= 0.5 && (double) Vector3.Dot(transform.forward, (owner.Enemy.transform.position - owner.transform.position).normalized) > 0.25;
    }

    public override void OnStart()
    {
      base.OnStart();
      fighter = owner as NPCEnemy;
      waitDuration = followTime.Value;
      desiredWalkSpeed = 0.0f;
      agent.enabled = true;
      npcState.FightIdle(Aim.Value);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if (followTime.Value > 0.0 && startTime + (double) waitDuration < (double) Time.time)
        return TaskStatus.Success;
      if ((UnityEngine.Object) description == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (typeof (MeleeFightFollow).Name + " has no " + typeof (FollowDescription).Name + " attached"), (UnityEngine.Object) gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      UpdatePath();
      owner.RotationTarget = (Transform) null;
      owner.RotateByPath = false;
      owner.RetreatAngle = new float?();
      if ((bool) (UnityEngine.Object) fighter && (fighter.IsReacting || fighter.IsQuickBlock))
      {
        if (fighter.IsContrReacting && (UnityEngine.Object) fighter.CounterReactionEnemy != (UnityEngine.Object) null)
          owner.RotationTarget = fighter.CounterReactionEnemy.transform;
        else if (fighter.IsQuickBlock && (UnityEngine.Object) fighter.PrePunchEnemy != (UnityEngine.Object) null)
          owner.RotationTarget = fighter.PrePunchEnemy.transform;
        return TaskStatus.Running;
      }
      Vector3 lhs = owner.Enemy.transform.position - owner.transform.position;
      float magnitude = lhs.magnitude;
      lhs.Normalize();
      if ((bool) (UnityEngine.Object) fighter && (fighter.IsAttacking || fighter.IsContrReacting))
      {
        owner.RotationTarget = owner.Enemy.transform;
        return TaskStatus.Running;
      }
      float num = Vector3.Dot(lhs, owner.Enemy.Velocity);
      if (NavMesh.Raycast(owner.transform.position, owner.Enemy.transform.position, out NavMeshHit _, -1))
      {
        if (!agent.hasPath)
          return TaskStatus.Running;
        if ((double) agent.remainingDistance > description.StopDistance)
        {
          desiredWalkSpeed = num <= 1.0 ? (!IsEnemyRunningAway() ? ((double) agent.remainingDistance > description.RunDistance ? 2f : 1f) : 2f) : 2f;
          owner.RotationTarget = owner.Enemy.transform;
          owner.RotateByPath = true;
          owner.RetreatAngle = new float?();
        }
      }
      else if (magnitude > (double) description.StopDistance)
      {
        desiredWalkSpeed = num <= 1.0 ? (!IsEnemyRunningAway() ? ((double) agent.remainingDistance > description.RunDistance ? 2f : 1f) : 2f) : 2f;
        owner.RotationTarget = owner.Enemy.transform;
      }
      else
      {
        desiredWalkSpeed = 0.0f;
        if ((bool) (UnityEngine.Object) fighter && (fighter.IsContrReacting || !fighter.IsReacting))
          owner.RotationTarget = owner.Enemy.transform;
        animator.GetInteger("Fight.AttackType");
        if (fightAnimatorState.IsAttacking)
          owner.RotationTarget = owner.Enemy.transform;
      }
      owner.DesiredWalkSpeed = desiredWalkSpeed;
      agent.nextPosition = animator.rootPosition;
      if (owner.Enemy is PlayerEnemy && UpdatePlayerKnockDdown(owner.Enemy as PlayerEnemy) || magnitude >= (double) description.AttackDistance)
        return TaskStatus.Running;
      if (owner.Enemy is PlayerEnemy && owner.Enemy.BlockNormalizedTime > (double) description.PushIfBlockTimeMoreThan && !fightAnimatorState.IsPushing && !fightAnimatorState.IsReaction && !fightAnimatorState.IsAttacking)
      {
        if ((double) UnityEngine.Random.value < 0.5)
        {
          animatorState.SetTrigger("Fight.Triggers/Push");
        }
        else
        {
          animatorState.SetTrigger("Fight.Triggers/Attack");
          animator.SetInteger("Fight.AttackType", 8);
        }
        return TaskStatus.Running;
      }
      if (!fightAnimatorState.IsAttacking && !fightAnimatorState.IsPushing)
        attackCooldownTime -= deltaTime;
      if (attackCooldownTime <= 0.0 && !fightAnimatorState.IsAttacking && !fightAnimatorState.IsPushing && !fightAnimatorState.IsReaction && !fightAnimatorState.IsQuickBlock)
      {
        if (desiredWalkSpeed > 0.5)
        {
          animatorState.SetTrigger("Fight.Triggers/RunPunch");
          attackCooldownTime = 2f;
        }
        else if (magnitude < 0.60000002384185791 * Fight.Description.NPCHitDistance)
        {
          animatorState.SetTrigger("Fight.Triggers/Attack");
          animator.SetInteger("Fight.AttackType", 0);
          attackCooldownTime = description.PunchCooldownTime;
        }
        else if (magnitude < 1.0 * Fight.Description.NPCHitDistance)
        {
          animatorState.SetTrigger("Fight.Triggers/Attack");
          animator.SetInteger("Fight.AttackType", 1);
          attackCooldownTime = description.StepPunchCooldownTime;
        }
        else if (magnitude < (double) description.AttackDistance)
        {
          animatorState.SetTrigger("Fight.Triggers/Attack");
          animator.SetInteger("Fight.AttackType", 3);
          animator.SetBool("Fight.AttackAfterCheat", (double) UnityEngine.Random.value > description.CheatProbability);
          attackCooldownTime = description.TelegraphPunchCooldownTime;
        }
      }
      return TaskStatus.Running;
    }

    private void UpdatePath()
    {
      if ((double) (lastPlayerPosition - owner.Enemy.transform.position).magnitude <= 0.33000001311302185)
        return;
      if (!agent.isOnNavMesh)
        agent.Warp(transform.position);
      if (agent.isOnNavMesh)
        agent.destination = owner.Enemy.transform.position;
      NavMeshUtility.DrawPath(agent);
      lastPlayerPosition = owner.Enemy.transform.position;
    }

    private bool UpdatePlayerKnockDdown(PlayerEnemy player)
    {
      playerKnockdownCooldownLeft -= Time.fixedDeltaTime;
      if (playerKnockdownCooldownLeft > 0.0)
        return false;
      Vector3 vector3 = player.transform.position - owner.transform.position;
      float magnitude = vector3.magnitude;
      Vector3 lhs = vector3 / magnitude;
      if (magnitude > 5.0 || (double) Vector3.Dot(lhs, player.transform.forward) < 0.0 || magnitude >= 3.0)
        return false;
      fighter.TriggerAction(WeaponActionEnum.KnockDown);
      playerKnockdownCooldownLeft = description.KnockDownCooldownTime;
      return true;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowTime", followTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Aim", Aim);
      BehaviorTreeDataWriteUtility.WriteUnity<FollowDescription>(writer, "Description", description);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      followTime = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowTime", followTime);
      Aim = BehaviorTreeDataReadUtility.ReadShared(reader, "Aim", Aim);
      description = BehaviorTreeDataReadUtility.ReadUnity<FollowDescription>(reader, "Description", description);
    }
  }
}
