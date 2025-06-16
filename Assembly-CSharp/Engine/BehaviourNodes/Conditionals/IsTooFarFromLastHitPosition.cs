using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals;

[TaskDescription("Is distance from last hit position more than?")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(IsTooFarFromLastHitPosition))]
public class IsTooFarFromLastHitPosition :
	Conditional,
	IStub,
	ISerializeDataWrite,
	ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public float Distance;

	private CombatServiceCharacterInfo owner;

	public override void OnAwake() {
		var component = gameObject.GetComponent<EnemyBase>();
		owner = ServiceLocator.GetService<CombatService>().GetCharacterInfo(component);
	}

	public override TaskStatus OnUpdate() {
		return owner == null
			? TaskStatus.Failure
			:
			(owner.LastGotHitPosition - gameObject.transform.position).magnitude > (double)Distance
				?
				TaskStatus.Success
				: TaskStatus.Failure;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		DefaultDataWriteUtility.Write(writer, "Distance", Distance);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Distance = DefaultDataReadUtility.Read(reader, "Distance", Distance);
	}
}