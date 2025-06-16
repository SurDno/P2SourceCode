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

[TaskCategory("Pathologic/Fight/Temp/Movement")]
[TaskDescription("Преследовать противника")]
[TaskIcon("{SkinColor}WaitIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(FightFollow))]
public class FightFollow : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat followTime = 0.0f;

	[Header("Передвижение")]
	[Tooltip("Если игрок удалился на эту дистанцию, но NPC переходит на бег.")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedFloat RunDistance = 5f;

	[Tooltip("Если игрок удалился на эту дистанцию, но NPC останавливается.")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedFloat StopDistance = 1.2f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedBool Aim;

	private NpcState npcState;

	public override void OnStart() {
		base.OnStart();
		if (npcState == null) {
			npcState = gameObject.GetComponent<NpcState>();
			if (npcState == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NpcState).Name + " engine component",
					gameObject);
				return;
			}
		}

		npcState.FightFollow(StopDistance.Value, RunDistance.Value, Aim.Value);
	}

	public override TaskStatus DoUpdate(float deltaTime) {
		if (followTime.Value > 0.0 && startTime + (double)waitDuration < Time.time)
			return TaskStatus.Success;
		if (npcState.CurrentNpcState != NpcStateEnum.FightFollow)
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

	public new void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowTime", followTime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RunDistance", RunDistance);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "StopDistance", StopDistance);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Aim", Aim);
	}

	public new void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		followTime = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowTime", followTime);
		RunDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "RunDistance", RunDistance);
		StopDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "StopDistance", StopDistance);
		Aim = BehaviorTreeDataReadUtility.ReadShared(reader, "Aim", Aim);
	}
}