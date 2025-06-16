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
[TaskDescription("Преследовать противника и атаковать по возможности")]
[TaskIcon("{SkinColor}WaitIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(MeleeFightSetQuickBlockProfile))]
public class MeleeFightSetQuickBlockProfile :
	Action,
	IStub,
	ISerializeDataWrite,
	ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat followTime = 0.0f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	private QuickBlockDescription description;

	private NPCEnemy owner;

	public override TaskStatus OnUpdate() {
		if (description == null) {
			Debug.LogWarning(typeof(MeleeFightFollow).Name + " has no " + typeof(FollowDescription).Name + " attached",
				gameObject);
			return TaskStatus.Failure;
		}

		if (this.owner == null) {
			this.owner = gameObject.GetComponentNonAlloc<NPCEnemy>();
			if (this.owner == null)
				return TaskStatus.Failure;
		}

		var owner = this.owner;
		owner.QuickBlockProbability = description.QuickBlockProbability;
		owner.DodgeProbability = description.DodgeProbability;
		return TaskStatus.Success;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowTime", followTime);
		BehaviorTreeDataWriteUtility.WriteUnity(writer, "Description", description);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		followTime = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowTime", followTime);
		description = BehaviorTreeDataReadUtility.ReadUnity(reader, "Description", description);
	}
}