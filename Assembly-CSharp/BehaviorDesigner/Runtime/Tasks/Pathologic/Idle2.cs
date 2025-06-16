using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Idle (puts on closest navmesh position)")]
[TaskCategory("Pathologic")]
[TaskIcon("Pathologic_IdleIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(Idle2))]
public class Idle2 : IdleBase, IStub, ISerializeDataWrite, ISerializeDataRead {
	protected override void DoIdle(NpcState state, float primaryIdleProbability) {
		state.Idle(primaryIdleProbability, MakeObstacle.Value);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "PrimaryIdleProbability", primaryIdleProbability);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "IdleTime", idleTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomIdle", randomIdle);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomIdleMin", randomIdleMin);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomIdleMax", randomIdleMax);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "MakeObstacle", MakeObstacle);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		primaryIdleProbability =
			BehaviorTreeDataReadUtility.ReadShared(reader, "PrimaryIdleProbability", primaryIdleProbability);
		idleTime = BehaviorTreeDataReadUtility.ReadShared(reader, "IdleTime", idleTime);
		randomIdle = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomIdle", randomIdle);
		randomIdleMin = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomIdleMin", randomIdleMin);
		randomIdleMax = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomIdleMax", randomIdleMax);
		MakeObstacle = BehaviorTreeDataReadUtility.ReadShared(reader, "MakeObstacle", MakeObstacle);
	}
}