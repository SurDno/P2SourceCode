using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Preset idle")]
[TaskCategory("Pathologic")]
[TaskIcon("Pathologic_IdleIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(IdlePreset))]
public class IdlePreset : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	private NpcState npcState;

	public override void OnAwake() {
		npcState = gameObject.GetComponent<NpcState>();
		if (!(npcState == null))
			return;
		Debug.LogError(gameObject.name + ": doesn't contain " + typeof(NpcState).Name + " engine component");
	}

	public override void OnStart() {
		if (npcState == null)
			return;
		npcState.PresetIdle();
	}

	public override TaskStatus OnUpdate() {
		return npcState == null ? TaskStatus.Failure : TaskStatus.Running;
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