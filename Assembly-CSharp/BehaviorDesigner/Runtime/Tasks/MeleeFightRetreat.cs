// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.MeleeFightRetreat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private RetreatDescription description;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Escaping;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
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
      this.fighter = this.owner as NPCEnemy;
      this.startTime = Time.time;
      this.waitDuration = this.RetreatTime.Value;
      this.agent.enabled = true;
      this.npcState.FightIdle(false);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((UnityEngine.Object) this.description == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (typeof (MeleeFightRetreat).Name + " has no " + typeof (RetreatDescription).Name + " attached"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      this.owner.RotationTarget = (Transform) null;
      this.owner.RotateByPath = false;
      this.owner.RetreatAngle = new float?();
      Vector3 forward = this.owner.Enemy.transform.position - this.owner.transform.position;
      float magnitude = forward.magnitude;
      forward.Normalize();
      if (this.fighter.IsReacting)
      {
        if (this.fighter.IsAttacking || this.fighter.IsContrReacting)
          this.owner.transform.rotation = Quaternion.RotateTowards(this.owner.transform.rotation, Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.LookRotation(forward), 270f * Time.deltaTime);
        return TaskStatus.Running;
      }
      float? retreatDirection = PathfindingHelper.FindBestRetreatDirection(this.owner.transform.position, this.owner.Enemy.transform.position);
      Quaternion quaternion1 = Quaternion.LookRotation(forward);
      if (!retreatDirection.HasValue)
      {
        this.desiredWalkSpeed = 0.0f;
      }
      else
      {
        if ((double) magnitude < (double) this.description.RunDistance)
          this.desiredWalkSpeed = -2f;
        else if ((double) magnitude < (double) this.description.WalkDistance)
          this.desiredWalkSpeed = -1f;
        else if ((double) magnitude < (double) this.description.StopDistance)
          this.desiredWalkSpeed = 0.0f;
        if (this.Escaping.Value && (double) magnitude > (double) this.description.EscapeDistance || (double) this.RetreatTime.Value > 0.0 && (double) this.startTime + (double) this.waitDuration < (double) Time.time)
          return TaskStatus.Success;
        Quaternion quaternion2 = quaternion1 * Quaternion.AngleAxis(retreatDirection.Value, Vector3.up);
      }
      this.owner.DesiredWalkSpeed = this.desiredWalkSpeed;
      this.owner.RotationTarget = this.owner.Enemy.transform;
      if ((double) magnitude < (double) this.description.AttackDistance)
      {
        if (!this.fightAnimatorState.IsAttacking)
          this.attackCooldownTime -= deltaTime;
        if ((double) this.attackCooldownTime <= 0.0 && !this.fightAnimatorState.IsAttacking && !this.fightAnimatorState.IsReaction && !this.fightAnimatorState.IsQuickBlock)
        {
          if ((double) magnitude < 0.60000002384185791 * (double) Fight.Description.NPCHitDistance)
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
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteUnity<RetreatDescription>(writer, "Description", this.description);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Escaping", this.Escaping);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RetreatTime", this.RetreatTime);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.description = BehaviorTreeDataReadUtility.ReadUnity<RetreatDescription>(reader, "Description", this.description);
      this.Escaping = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Escaping", this.Escaping);
      this.RetreatTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RetreatTime", this.RetreatTime);
    }
  }
}
