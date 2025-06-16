using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud;

[TaskDescription("Attack")]
[TaskCategory("Pathologic/PlagueCloud")]
[TaskIcon("Pathologic_LongIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(Attack))]
public class Attack : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransform Target;

	[Tooltip("Meters/sec")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat MovementSpeed = 5f;

	[Tooltip("Stop distance")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat StopDistance = 0.0f;

	[Tooltip("Rocket Mode")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedBool RocketMode = false;

	[Tooltip("Rocket overshoot")] [DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat RocketOvershoot = 2f;

	private Vector3 targetPos;
	private NpcState npcState;

	public override void OnStart() {
		if (npcState == null) {
			npcState = gameObject.GetComponent<NpcState>();
			if (npcState == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NpcState).Name + " engine component",
					gameObject);
				return;
			}
		}

		if (Target.Value == null)
			Debug.LogWarningFormat("{0} : null target", gameObject.name);
		else {
			targetPos = Target.Value.transform.position;
			if (RocketMode.Value)
				targetPos += (targetPos - transform.position).normalized * RocketOvershoot.Value;
			if (npcState == null || Target.Value == null)
				return;
			npcState.MoveFollow(Target.Value, 0.0f);
		}
	}

	public override TaskStatus OnUpdate() {
		if (npcState == null || Target.Value == null || npcState.CurrentNpcState != NpcStateEnum.MoveFollow)
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
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "MovementSpeed", MovementSpeed);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "StopDistance", StopDistance);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RocketMode", RocketMode);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RocketOvershoot", RocketOvershoot);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
		MovementSpeed = BehaviorTreeDataReadUtility.ReadShared(reader, "MovementSpeed", MovementSpeed);
		StopDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "StopDistance", StopDistance);
		RocketMode = BehaviorTreeDataReadUtility.ReadShared(reader, "RocketMode", RocketMode);
		RocketOvershoot = BehaviorTreeDataReadUtility.ReadShared(reader, "RocketOvershoot", RocketOvershoot);
	}
}