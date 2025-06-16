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

[TaskDescription("Не работает!!! Send plague damage event to Info component.")]
[TaskCategory("Pathologic/Player")]
[TaskIcon("Pathologic_InstantIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(PlagueDamageEvent))]
public class PlagueDamageEvent : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransform WhoWillBeDamaged;

	[Tooltip("Use null if you are the damage source.")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedTransform WhoDamages;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat Amount = 1f;

	public override TaskStatus OnUpdate() {
		if (WhoWillBeDamaged.Value == null) {
			Debug.LogWarningFormat("{0}: WhoWillBeDamaged is null", gameObject.name);
			return TaskStatus.Failure;
		}

		if (EntityUtility.GetEntity(WhoDamages.Value == null ? gameObject : WhoDamages.Value.gameObject) == null) {
			Debug.LogWarningFormat("{0}: doesn't match any entity", gameObject.name);
			return TaskStatus.Failure;
		}

		if (EntityUtility.GetEntity(WhoWillBeDamaged.Value.gameObject) != null)
			return TaskStatus.Success;
		Debug.LogWarningFormat("{0}: doesn't match any entity", gameObject.name);
		return TaskStatus.Failure;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "WhoWillBeDamaged", WhoWillBeDamaged);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "WhoDamages", WhoDamages);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Amount", Amount);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		WhoWillBeDamaged = BehaviorTreeDataReadUtility.ReadShared(reader, "WhoWillBeDamaged", WhoWillBeDamaged);
		WhoDamages = BehaviorTreeDataReadUtility.ReadShared(reader, "WhoDamages", WhoDamages);
		Amount = BehaviorTreeDataReadUtility.ReadShared(reader, "Amount", Amount);
	}
}