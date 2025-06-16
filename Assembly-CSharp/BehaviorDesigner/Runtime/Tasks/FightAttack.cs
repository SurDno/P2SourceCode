using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/Fight/Melee")]
[TaskDescription("Преследовать противника и атаковать по возможности")]
[TaskIcon("{SkinColor}WaitIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(FightAttack))]
public class FightAttack : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedFloat attackTime = 0.0f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedBool commonAttacks = true;

	[Tooltip("Столько игрок должен простоять в блоке (минимум), чтобы Npc его попытался пробить")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	private SharedBool ruinBlocks = true;

	[Tooltip("Столько противник должен простоять в блоке (минимум), чтобы Npc его попытался пробить")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	private SharedFloat ruinBlockMinTime = 3f;

	[Tooltip("Столько противник должен простоять в блоке (максимум), чтобы Npc его попытался пробить")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	private SharedFloat ruinBlockMaxTime = 4f;

	[Tooltip(
		"Примерно с такой вероятностью при каждом старте ноды будет выставлять нулевое время ruinBlockTime. То есть Npc неожиданно пробьет блок без ожидания.")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	private SharedFloat zeroRuinBlockProbability = 0.05f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedBool attackInMovement = true;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedFloat counterAttacksProbability = 1f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedBool knockDowns = true;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedFloat attackCooldownTime = 2.5f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	private FollowDescription description;

	private float ruinBlockTime;
	private bool firstAttack;
	private float playerKnockdownCooldownLeft;

	private bool IsEnemyRunningAway() {
		return owner.Enemy.Velocity.magnitude >= 0.5 && Vector3.Dot(transform.forward,
			(owner.Enemy.transform.position - owner.transform.position).normalized) > 0.25;
	}

	private void NextRuinBlockTime() {
		if (Random.value < (double)zeroRuinBlockProbability.Value)
			ruinBlockTime = 0.0f;
		else
			ruinBlockTime = Random.Range(ruinBlockMinTime.Value, ruinBlockMaxTime.Value);
	}

	public override TaskStatus DoUpdate(float deltaTime) {
		if (owner.Enemy == null)
			return TaskStatus.Failure;
		if (owner.IsReacting || owner.IsDodge || owner.IsQuickBlock || owner.IsStagger || (knockDowns.Value &&
			    owner.Enemy is PlayerEnemy && UpdatePlayerKnockDdown(owner.Enemy as PlayerEnemy, deltaTime)))
			return TaskStatus.Running;
		var magnitude = (owner.Enemy.transform.position - owner.transform.position).magnitude;
		if (magnitude > description.AttackDistance * 2.0)
			firstAttack = true;
		if (magnitude < (double)description.AttackDistance &&
		    PathfindingHelper.IsFreeSpace(owner.Enemy.transform.position, owner.transform.position)) {
			if (firstAttack) {
				firstAttack = false;
				if (owner.AttackCooldownTimeLeft <= 0.0)
					owner.AttackCooldownTimeLeft = Random.value * 0.5f;
			}

			var flag = EnemyIsAttacking();
			if (ruinBlocks.Value && owner.Enemy is PlayerEnemy && owner.AttackCooldownTimeLeft <= 0.0 && !flag &&
			    owner.Enemy.BlockNormalizedTime > (double)ruinBlockTime && !owner.IsPushing && !owner.IsReacting &&
			    !owner.IsAttacking) {
				owner.TriggerAction(WeaponActionEnum.Uppercut);
				NextRuinBlockTime();
				owner.Enemy.BlockNormalizedTime = 0.0f;
				return TaskStatus.Running;
			}

			if (owner.AttackCooldownTimeLeft <= 0.0 && !owner.IsAttacking && !owner.IsPushing && !owner.IsReacting &&
			    !owner.IsQuickBlock && !flag) {
				if (attackInMovement.Value && owner.DesiredWalkSpeed > 0.5) {
					owner.TriggerAction(WeaponActionEnum.RunAttack);
					owner.AttackCooldownTimeLeft = attackCooldownTime.Value;
				} else if (commonAttacks.Value)
					Attack(magnitude);
			}
		}

		return TaskStatus.Running;
	}

	private void Attack(float distanceToEnemy) {
		if (distanceToEnemy < 1.0) {
			owner.TriggerAction(WeaponActionEnum.JabAttack);
			owner.AttackCooldownTimeLeft = description.PunchCooldownTime;
			owner.AttackCooldownTimeLeft = attackCooldownTime.Value;
		} else if (distanceToEnemy < 1.3999999761581421) {
			owner.TriggerAction(WeaponActionEnum.StepAttack);
			owner.AttackCooldownTimeLeft = description.StepPunchCooldownTime;
			owner.AttackCooldownTimeLeft = attackCooldownTime.Value;
		} else {
			if (distanceToEnemy >= (double)description.AttackDistance)
				return;
			owner.TriggerAction(WeaponActionEnum.TelegraphAttack);
			owner.AttackCooldownTimeLeft = description.TelegraphPunchCooldownTime;
			owner.AttackCooldownTimeLeft = attackCooldownTime.Value;
		}
	}

	private bool UpdatePlayerKnockDdown(PlayerEnemy player, float deltaTime) {
		playerKnockdownCooldownLeft -= deltaTime;
		if (playerKnockdownCooldownLeft > 0.0)
			return false;
		var vector3 = player.transform.position - owner.transform.position;
		var magnitude = vector3.magnitude;
		var lhs = vector3 / magnitude;
		if (magnitude > 5.0 || Vector3.Dot(lhs, player.transform.forward) < 0.0 || magnitude >= 2.5)
			return false;
		owner.TriggerAction(WeaponActionEnum.KnockDown);
		playerKnockdownCooldownLeft = description.KnockDownCooldownTime;
		return true;
	}

	public override void OnStart() {
		base.OnStart();
		waitDuration = attackTime.Value;
		NextRuinBlockTime();
		firstAttack = true;
		if (!(bool)(Object)owner)
			return;
		owner.WasPunchedEvent += Owner_WasPunchedEvent;
		owner.WasPunchedToStaggerEvent += Owner_WasPunchedEvent;
		owner.WasPunchedToDodgeEvent += Owner_WasPunchedEvent;
	}

	private void Owner_PunchEvent(
		IEntity entity,
		ShotType shotType,
		ReactionType reactionType,
		WeaponEnum weaponEnum) {
		owner.AttackCooldownTimeLeft = attackCooldownTime.Value;
	}

	private void Owner_WasPunchedEvent(EnemyBase enemy) {
		if (owner.IsAttacking || !(enemy == owner.Enemy) || counterAttacksProbability.Value <= (double)Random.value)
			return;
		owner.AttackCooldownTimeLeft = 0.0f;
	}

	public override void OnEnd() {
		if ((bool)(Object)owner.Enemy) {
			owner.Enemy.WasPunchedEvent -= Owner_WasPunchedEvent;
			owner.WasPunchedToStaggerEvent -= Owner_WasPunchedEvent;
			owner.WasPunchedToDodgeEvent -= Owner_WasPunchedEvent;
		}

		base.OnEnd();
	}

	private bool EnemyIsAttacking() {
		if (!(owner.Enemy is NPCEnemy))
			return false;
		var enemy = owner.Enemy as NPCEnemy;
		return enemy.IsAttacking && enemy.Enemy == owner;
	}

	public new void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "AttackTime", attackTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "CommonAttacks", commonAttacks);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RuinBlocks", ruinBlocks);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RuinBlockMinTime", ruinBlockMinTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RuinBlockMaxTime", ruinBlockMaxTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "ZeroRuinBlockProbability", zeroRuinBlockProbability);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "AttackInMovement", attackInMovement);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "CounterAttacksProbability", counterAttacksProbability);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "KnockDowns", knockDowns);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "AttackCooldownTime", attackCooldownTime);
		BehaviorTreeDataWriteUtility.WriteUnity(writer, "Description", description);
	}

	public new void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		attackTime = BehaviorTreeDataReadUtility.ReadShared(reader, "AttackTime", attackTime);
		commonAttacks = BehaviorTreeDataReadUtility.ReadShared(reader, "CommonAttacks", commonAttacks);
		ruinBlocks = BehaviorTreeDataReadUtility.ReadShared(reader, "RuinBlocks", ruinBlocks);
		ruinBlockMinTime = BehaviorTreeDataReadUtility.ReadShared(reader, "RuinBlockMinTime", ruinBlockMinTime);
		ruinBlockMaxTime = BehaviorTreeDataReadUtility.ReadShared(reader, "RuinBlockMaxTime", ruinBlockMaxTime);
		zeroRuinBlockProbability =
			BehaviorTreeDataReadUtility.ReadShared(reader, "ZeroRuinBlockProbability", zeroRuinBlockProbability);
		attackInMovement = BehaviorTreeDataReadUtility.ReadShared(reader, "AttackInMovement", attackInMovement);
		counterAttacksProbability =
			BehaviorTreeDataReadUtility.ReadShared(reader, "CounterAttacksProbability", counterAttacksProbability);
		knockDowns = BehaviorTreeDataReadUtility.ReadShared(reader, "KnockDowns", knockDowns);
		attackCooldownTime = BehaviorTreeDataReadUtility.ReadShared(reader, "AttackCooldownTime", attackCooldownTime);
		description = BehaviorTreeDataReadUtility.ReadUnity(reader, "Description", description);
	}
}