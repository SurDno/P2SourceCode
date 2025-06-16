using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/Fight/Melee")]
[TaskDescription("Перейти в блокирующую стойку")]
[TaskIcon("{SkinColor}WaitIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(MeleeFightBlockStanceDisable))]
public class MeleeFightBlockStanceDisable : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	private NPCEnemy owner;

	public override TaskStatus OnUpdate() {
		if (owner == null) {
			owner = gameObject.GetComponentNonAlloc<NPCEnemy>();
			if (owner == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NPCEnemy).Name + " engine component",
					gameObject);
				return TaskStatus.Failure;
			}
		}

		owner.BlockStance = false;
		return TaskStatus.Success;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
	}
}