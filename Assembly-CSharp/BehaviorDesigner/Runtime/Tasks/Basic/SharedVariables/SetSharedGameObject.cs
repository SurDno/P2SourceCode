using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables;

[FactoryProxy(typeof(SetSharedGameObject))]
[TaskCategory("Basic/SharedVariable")]
[TaskDescription("Sets the SharedGameObject variable to the specified object. Returns Success.")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class SetSharedGameObject : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("The value to set the SharedGameObject to. If null the variable will be set to the current GameObject")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedGameObject targetValue;

	[RequiredField]
	[Tooltip("The SharedGameObject to set")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedGameObject targetVariable;

	[Tooltip("Can the target value be null?")] [DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedBool valueCanBeNull;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetValue", targetValue);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetVariable", targetVariable);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "ValueCanBeNull", valueCanBeNull);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		targetValue = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetValue", targetValue);
		targetVariable = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetVariable", targetVariable);
		valueCanBeNull = BehaviorTreeDataReadUtility.ReadShared(reader, "ValueCanBeNull", valueCanBeNull);
	}

	public override TaskStatus OnUpdate() {
		targetVariable.Value = targetValue.Value != null || valueCanBeNull.Value ? targetValue.Value : gameObject;
		return TaskStatus.Success;
	}

	public override void OnReset() {
		valueCanBeNull = false;
		targetValue = null;
		targetVariable = null;
	}
}