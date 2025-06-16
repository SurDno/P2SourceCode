using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/Fight/Tuorial Suok")]
[TaskDescription("Атаковать только RuinBlock")]
[TaskIcon("{SkinColor}WaitIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(TutorialSuokAttackRuinBlock))]
public class TutorialSuokAttackRuinBlock :
	FightBase,
	IStub,
	ISerializeDataWrite,
	ISerializeDataRead {
	private float nextAttackTime;

	public override void OnStart() {
		base.OnStart();
		nextAttackTime = Time.time;
	}

	public override TaskStatus OnUpdate() {
		if (owner.gameObject == null || !owner.gameObject.activeSelf || owner?.Enemy == null || owner.IsReacting)
			return TaskStatus.Failure;
		if (Time.time < (double)nextAttackTime ||
		    (owner.Enemy.transform.position - owner.transform.position).magnitude >= 2.0 ||
		    !(owner.Enemy is PlayerEnemy))
			return TaskStatus.Running;
		var enemy = (PlayerEnemy)owner.Enemy;
		if (owner.IsPushing || owner.IsReacting || owner.IsAttacking)
			return TaskStatus.Running;
		nextAttackTime = Time.time + Random.Range(4f, 6f);
		owner.TriggerAction(WeaponActionEnum.Uppercut);
		owner.RotationTarget = null;
		owner.RotateByPath = false;
		return TaskStatus.Running;
	}

	public new void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
	}

	public new void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
	}
}