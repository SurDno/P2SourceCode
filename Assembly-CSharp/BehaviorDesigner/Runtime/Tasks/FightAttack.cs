// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.FightAttack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Преследовать противника и атаковать по возможности")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightAttack))]
  public class FightAttack : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat attackTime = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedBool commonAttacks = (SharedBool) true;
    [Tooltip("Столько игрок должен простоять в блоке (минимум), чтобы Npc его попытался пробить")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedBool ruinBlocks = (SharedBool) true;
    [Tooltip("Столько противник должен простоять в блоке (минимум), чтобы Npc его попытался пробить")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat ruinBlockMinTime = (SharedFloat) 3f;
    [Tooltip("Столько противник должен простоять в блоке (максимум), чтобы Npc его попытался пробить")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat ruinBlockMaxTime = (SharedFloat) 4f;
    [Tooltip("Примерно с такой вероятностью при каждом старте ноды будет выставлять нулевое время ruinBlockTime. То есть Npc неожиданно пробьет блок без ожидания.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat zeroRuinBlockProbability = (SharedFloat) 0.05f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedBool attackInMovement = (SharedBool) true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat counterAttacksProbability = (SharedFloat) 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedBool knockDowns = (SharedBool) true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat attackCooldownTime = (SharedFloat) 2.5f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private FollowDescription description;
    private float ruinBlockTime;
    private bool firstAttack;
    private float playerKnockdownCooldownLeft;

    private bool IsEnemyRunningAway()
    {
      return (double) this.owner.Enemy.Velocity.magnitude >= 0.5 && (double) Vector3.Dot(this.transform.forward, (this.owner.Enemy.transform.position - this.owner.transform.position).normalized) > 0.25;
    }

    private void NextRuinBlockTime()
    {
      if ((double) UnityEngine.Random.value < (double) this.zeroRuinBlockProbability.Value)
        this.ruinBlockTime = 0.0f;
      else
        this.ruinBlockTime = UnityEngine.Random.Range(this.ruinBlockMinTime.Value, this.ruinBlockMaxTime.Value);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if (this.owner.IsReacting || this.owner.IsDodge || this.owner.IsQuickBlock || this.owner.IsStagger || this.knockDowns.Value && this.owner.Enemy is PlayerEnemy && this.UpdatePlayerKnockDdown(this.owner.Enemy as PlayerEnemy, deltaTime))
        return TaskStatus.Running;
      float magnitude = (this.owner.Enemy.transform.position - this.owner.transform.position).magnitude;
      if ((double) magnitude > (double) this.description.AttackDistance * 2.0)
        this.firstAttack = true;
      if ((double) magnitude < (double) this.description.AttackDistance && PathfindingHelper.IsFreeSpace(this.owner.Enemy.transform.position, this.owner.transform.position))
      {
        if (this.firstAttack)
        {
          this.firstAttack = false;
          if ((double) this.owner.AttackCooldownTimeLeft <= 0.0)
            this.owner.AttackCooldownTimeLeft = UnityEngine.Random.value * 0.5f;
        }
        bool flag = this.EnemyIsAttacking();
        if (this.ruinBlocks.Value && this.owner.Enemy is PlayerEnemy && (double) this.owner.AttackCooldownTimeLeft <= 0.0 && !flag && (double) this.owner.Enemy.BlockNormalizedTime > (double) this.ruinBlockTime && !this.owner.IsPushing && !this.owner.IsReacting && !this.owner.IsAttacking)
        {
          this.owner.TriggerAction(WeaponActionEnum.Uppercut);
          this.NextRuinBlockTime();
          this.owner.Enemy.BlockNormalizedTime = 0.0f;
          return TaskStatus.Running;
        }
        if ((double) this.owner.AttackCooldownTimeLeft <= 0.0 && !this.owner.IsAttacking && !this.owner.IsPushing && !this.owner.IsReacting && !this.owner.IsQuickBlock && !flag)
        {
          if (this.attackInMovement.Value && (double) this.owner.DesiredWalkSpeed > 0.5)
          {
            this.owner.TriggerAction(WeaponActionEnum.RunAttack);
            this.owner.AttackCooldownTimeLeft = this.attackCooldownTime.Value;
          }
          else if (this.commonAttacks.Value)
            this.Attack(magnitude);
        }
      }
      return TaskStatus.Running;
    }

    private void Attack(float distanceToEnemy)
    {
      if ((double) distanceToEnemy < 1.0)
      {
        this.owner.TriggerAction(WeaponActionEnum.JabAttack);
        this.owner.AttackCooldownTimeLeft = this.description.PunchCooldownTime;
        this.owner.AttackCooldownTimeLeft = this.attackCooldownTime.Value;
      }
      else if ((double) distanceToEnemy < 1.3999999761581421)
      {
        this.owner.TriggerAction(WeaponActionEnum.StepAttack);
        this.owner.AttackCooldownTimeLeft = this.description.StepPunchCooldownTime;
        this.owner.AttackCooldownTimeLeft = this.attackCooldownTime.Value;
      }
      else
      {
        if ((double) distanceToEnemy >= (double) this.description.AttackDistance)
          return;
        this.owner.TriggerAction(WeaponActionEnum.TelegraphAttack);
        this.owner.AttackCooldownTimeLeft = this.description.TelegraphPunchCooldownTime;
        this.owner.AttackCooldownTimeLeft = this.attackCooldownTime.Value;
      }
    }

    private bool UpdatePlayerKnockDdown(PlayerEnemy player, float deltaTime)
    {
      this.playerKnockdownCooldownLeft -= deltaTime;
      if ((double) this.playerKnockdownCooldownLeft > 0.0)
        return false;
      Vector3 vector3 = player.transform.position - this.owner.transform.position;
      float magnitude = vector3.magnitude;
      Vector3 lhs = vector3 / magnitude;
      if ((double) magnitude > 5.0 || (double) Vector3.Dot(lhs, player.transform.forward) < 0.0 || (double) magnitude >= 2.5)
        return false;
      this.owner.TriggerAction(WeaponActionEnum.KnockDown);
      this.playerKnockdownCooldownLeft = this.description.KnockDownCooldownTime;
      return true;
    }

    public override void OnStart()
    {
      base.OnStart();
      this.waitDuration = this.attackTime.Value;
      this.NextRuinBlockTime();
      this.firstAttack = true;
      if (!(bool) (UnityEngine.Object) this.owner)
        return;
      this.owner.WasPunchedEvent += new Action<EnemyBase>(this.Owner_WasPunchedEvent);
      this.owner.WasPunchedToStaggerEvent += new Action<EnemyBase>(this.Owner_WasPunchedEvent);
      this.owner.WasPunchedToDodgeEvent += new Action<EnemyBase>(this.Owner_WasPunchedEvent);
    }

    private void Owner_PunchEvent(
      IEntity entity,
      ShotType shotType,
      ReactionType reactionType,
      WeaponEnum weaponEnum)
    {
      this.owner.AttackCooldownTimeLeft = this.attackCooldownTime.Value;
    }

    private void Owner_WasPunchedEvent(EnemyBase enemy)
    {
      if (this.owner.IsAttacking || !((UnityEngine.Object) enemy == (UnityEngine.Object) this.owner.Enemy) || (double) this.counterAttacksProbability.Value <= (double) UnityEngine.Random.value)
        return;
      this.owner.AttackCooldownTimeLeft = 0.0f;
    }

    public override void OnEnd()
    {
      if ((bool) (UnityEngine.Object) this.owner.Enemy)
      {
        this.owner.Enemy.WasPunchedEvent -= new Action<EnemyBase>(this.Owner_WasPunchedEvent);
        this.owner.WasPunchedToStaggerEvent -= new Action<EnemyBase>(this.Owner_WasPunchedEvent);
        this.owner.WasPunchedToDodgeEvent -= new Action<EnemyBase>(this.Owner_WasPunchedEvent);
      }
      base.OnEnd();
    }

    private bool EnemyIsAttacking()
    {
      if (!(this.owner.Enemy is NPCEnemy))
        return false;
      NPCEnemy enemy = this.owner.Enemy as NPCEnemy;
      return enemy.IsAttacking && (UnityEngine.Object) enemy.Enemy == (UnityEngine.Object) this.owner;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "AttackTime", this.attackTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "CommonAttacks", this.commonAttacks);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RuinBlocks", this.ruinBlocks);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RuinBlockMinTime", this.ruinBlockMinTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RuinBlockMaxTime", this.ruinBlockMaxTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "ZeroRuinBlockProbability", this.zeroRuinBlockProbability);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "AttackInMovement", this.attackInMovement);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "CounterAttacksProbability", this.counterAttacksProbability);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "KnockDowns", this.knockDowns);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "AttackCooldownTime", this.attackCooldownTime);
      BehaviorTreeDataWriteUtility.WriteUnity<FollowDescription>(writer, "Description", this.description);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.attackTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "AttackTime", this.attackTime);
      this.commonAttacks = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "CommonAttacks", this.commonAttacks);
      this.ruinBlocks = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RuinBlocks", this.ruinBlocks);
      this.ruinBlockMinTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RuinBlockMinTime", this.ruinBlockMinTime);
      this.ruinBlockMaxTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RuinBlockMaxTime", this.ruinBlockMaxTime);
      this.zeroRuinBlockProbability = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "ZeroRuinBlockProbability", this.zeroRuinBlockProbability);
      this.attackInMovement = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "AttackInMovement", this.attackInMovement);
      this.counterAttacksProbability = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "CounterAttacksProbability", this.counterAttacksProbability);
      this.knockDowns = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "KnockDowns", this.knockDowns);
      this.attackCooldownTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "AttackCooldownTime", this.attackCooldownTime);
      this.description = BehaviorTreeDataReadUtility.ReadUnity<FollowDescription>(reader, "Description", this.description);
    }
  }
}
