using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Отступать от противника и атаковать по возможности")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightRetreat))]
  public class MeleeFightRetreat : MeleeFightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    private RetreatDescription description;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool Escaping;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat RetreatTime;
    private float desiredWalkSpeed;
    private float currentWalkSpeed;
    private float desiredRetreatAngle;
    private float currentRetreatAngle;
    private NPCEnemy fighter;
    private float attackCooldownTime;
    private const float testFrequency = 2f;

    public override void OnStart()
    {
      base.OnStart();
      fighter = owner as NPCEnemy;
      startTime = Time.time;
      waitDuration = RetreatTime.Value;
      agent.enabled = true;
      npcState.FightIdle(false);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if (description == null)
      {
        Debug.LogWarning(typeof (MeleeFightRetreat).Name + " has no " + typeof (RetreatDescription).Name + " attached", gameObject);
        return TaskStatus.Failure;
      }
      if (owner.Enemy == null)
        return TaskStatus.Failure;
      owner.RotationTarget = null;
      owner.RotateByPath = false;
      owner.RetreatAngle = new float?();
      Vector3 forward = owner.Enemy.transform.position - owner.transform.position;
      float magnitude = forward.magnitude;
      forward.Normalize();
      if (fighter.IsReacting)
      {
        if (fighter.IsAttacking || fighter.IsContrReacting)
          owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.LookRotation(forward), 270f * Time.deltaTime);
        return TaskStatus.Running;
      }
      float? retreatDirection = PathfindingHelper.FindBestRetreatDirection(owner.transform.position, owner.Enemy.transform.position);
      Quaternion quaternion1 = Quaternion.LookRotation(forward);
      if (!retreatDirection.HasValue)
      {
        desiredWalkSpeed = 0.0f;
      }
      else
      {
        if (magnitude < (double) description.RunDistance)
          desiredWalkSpeed = -2f;
        else if (magnitude < (double) description.WalkDistance)
          desiredWalkSpeed = -1f;
        else if (magnitude < (double) description.StopDistance)
          desiredWalkSpeed = 0.0f;
        if (Escaping.Value && magnitude > (double) description.EscapeDistance || RetreatTime.Value > 0.0 && startTime + (double) waitDuration < Time.time)
          return TaskStatus.Success;
        Quaternion quaternion2 = quaternion1 * Quaternion.AngleAxis(retreatDirection.Value, Vector3.up);
      }
      owner.DesiredWalkSpeed = desiredWalkSpeed;
      owner.RotationTarget = owner.Enemy.transform;
      if (magnitude < (double) description.AttackDistance)
      {
        if (!fightAnimatorState.IsAttacking)
          attackCooldownTime -= deltaTime;
        if (attackCooldownTime <= 0.0 && !fightAnimatorState.IsAttacking && !fightAnimatorState.IsReaction && !fightAnimatorState.IsQuickBlock)
        {
          if (magnitude < 0.60000002384185791 * Fight.Description.NPCHitDistance)
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
            animator.SetBool("Fight.AttackAfterCheat", Random.value > (double) description.CheatProbability);
            attackCooldownTime = description.TelegraphPunchCooldownTime;
          }
        }
      }
      return TaskStatus.Running;
    }

    public override void OnPause(bool paused)
    {
    }

    public override void OnReset()
    {
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Description", description);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Escaping", Escaping);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RetreatTime", RetreatTime);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      description = BehaviorTreeDataReadUtility.ReadUnity(reader, "Description", description);
      Escaping = BehaviorTreeDataReadUtility.ReadShared(reader, "Escaping", Escaping);
      RetreatTime = BehaviorTreeDataReadUtility.ReadShared(reader, "RetreatTime", RetreatTime);
    }
  }
}
