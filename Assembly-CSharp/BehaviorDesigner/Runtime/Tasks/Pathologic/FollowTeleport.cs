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

[TaskDescription("Follow")]
[TaskCategory("Pathologic/Movement")]
[TaskIcon("Pathologic_LongIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(FollowTeleport))]
public class FollowTeleport : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("Follow distance")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat FollowDistance = 2f;

	[Tooltip("Через это время будет предпринята очередная попытка телепортации, если почтальон не дошел до цели.")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedFloat TeleportRepeatTime = 30f;

	[Tooltip("Нужно ли ждать пока игрок посмотрит на посыльного?")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[SerializeField]
	public SharedBool WaitForPlayerFacesYou = true;

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

		npcState.MoveFollowTeleport(FollowDistance.Value, TeleportRepeatTime.Value, WaitForPlayerFacesYou.Value);
	}

	public override TaskStatus OnUpdate() {
		if (npcState == null || npcState.CurrentNpcState != NpcStateEnum.MoveFollowTeleport)
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
		BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowDistance", FollowDistance);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "TeleportRepeatTime", TeleportRepeatTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "WaitForPlayerFacesYou", WaitForPlayerFacesYou);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		FollowDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowDistance", FollowDistance);
		TeleportRepeatTime = BehaviorTreeDataReadUtility.ReadShared(reader, "TeleportRepeatTime", TeleportRepeatTime);
		WaitForPlayerFacesYou =
			BehaviorTreeDataReadUtility.ReadShared(reader, "WaitForPlayerFacesYou", WaitForPlayerFacesYou);
	}
}