using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/Fight/Melee")]
[TaskDescription("атака бомбами")]
[TaskIcon("{SkinColor}WaitIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(FightBombAttack))]
public class FightBombAttack : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	private SharedFloat followTime = 0.0f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	private SharedFloat throwCooldown = 5f;

	private float attackCooldownTime;

	public override void OnStart() {
		base.OnStart();
		waitDuration = followTime.Value;
	}

	public override TaskStatus DoUpdate(float deltaTime) {
		if (owner.gameObject == null || !owner.gameObject.activeSelf || owner?.Enemy == null)
			return TaskStatus.Failure;
		if (owner.IsReacting)
			return TaskStatus.Running;
		var magnitude = (owner.Enemy.transform.position - owner.transform.position).magnitude;
		if (!owner.IsPushing && !owner.IsPushing)
			attackCooldownTime -= deltaTime;
		if (attackCooldownTime <= 0.0 && !owner.IsAttacking && !owner.IsPushing && !owner.IsReacting &&
		    !owner.IsQuickBlock) {
			var outAngle = 0.0f;
			if (CanThrowBomb(out outAngle)) {
				owner.EnqueueProjectileThrow(outAngle, 15f);
				if (outAngle * 57.295780181884766 > 25.0)
					owner.TriggerThrowBomb(2);
				else if (outAngle * 57.295780181884766 > 15.0)
					owner.TriggerThrowBomb(1);
				else
					owner.TriggerThrowBomb(0);
				attackCooldownTime = throwCooldown.Value;
			}
		}

		return TaskStatus.Running;
	}

	private bool CanThrowBomb(out float outAngle) {
		var v = 15f;
		var h = 1.5f;
		var vector3 = owner.Enemy.transform.position - owner.transform.position;
		float angle1;
		float angle2;
		if (BomberHelper.CalcThrowAngles(v, vector3.magnitude, h, out angle1, out angle2)) {
			var startPosition = owner.transform.position + Vector3.up * h;
			var normalized = vector3.normalized;
			var num = float.MinValue;
			if (BomberHelper.SphereCastParabola(angle1, v, h, startPosition, normalized))
				num = angle1;
			else if (BomberHelper.SphereCastParabola(angle2, v, h, startPosition, normalized))
				num = angle2;
			if (num != -3.4028234663852886E+38) {
				outAngle = num;
				return true;
			}
		}

		outAngle = 0.0f;
		return false;
	}

	public new void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowTime", followTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "ThrowCooldown", throwCooldown);
	}

	public new void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		followTime = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowTime", followTime);
		throwCooldown = BehaviorTreeDataReadUtility.ReadShared(reader, "ThrowCooldown", throwCooldown);
	}
}