using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Rotate To target rotation (exits when angle is small enough)")]
[TaskCategory("Pathologic")]
[TaskIcon("Pathologic_LongIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(RotateTo))]
public class RotateTo : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedQuaternion TargetRotation;

	private NpcState npcState;

	public override void OnAwake() {
		npcState = gameObject.GetComponent<NpcState>();
		if (!(npcState == null))
			return;
		Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NpcState).Name + " engine component");
	}

	public override void OnStart() {
		if (npcState == null)
			return;
		npcState.Rotate(TargetRotation.Value);
	}

	public override TaskStatus OnUpdate() {
		if (npcState == null || npcState.CurrentNpcState != NpcStateEnum.Rotate)
			return TaskStatus.Failure;
		switch (npcState.Status) {
			case NpcStateStatusEnum.Success:
				return TaskStatus.Success;
			case NpcStateStatusEnum.Failed:
				return TaskStatus.Failure;
			default:
				return TaskStatus.Running;
		}
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetRotation", TargetRotation);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		TargetRotation = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetRotation", TargetRotation);
	}
}