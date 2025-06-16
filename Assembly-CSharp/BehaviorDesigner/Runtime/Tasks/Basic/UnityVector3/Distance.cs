using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3;

[FactoryProxy(typeof(Distance))]
[TaskCategory("Basic/Vector3")]
[TaskDescription("Returns the distance between two Vector3s.")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Distance : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("The first Vector3")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedVector3 firstVector3;

	[Tooltip("The second Vector3")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedVector3 secondVector3;

	[Tooltip("The distance")] [RequiredField] [DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat storeResult;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "FirstVector3", firstVector3);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "SecondVector3", secondVector3);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "StoreResult", storeResult);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		firstVector3 = BehaviorTreeDataReadUtility.ReadShared(reader, "FirstVector3", firstVector3);
		secondVector3 = BehaviorTreeDataReadUtility.ReadShared(reader, "SecondVector3", secondVector3);
		storeResult = BehaviorTreeDataReadUtility.ReadShared(reader, "StoreResult", storeResult);
	}

	public override TaskStatus OnUpdate() {
		storeResult.Value = Vector3.Distance(firstVector3.Value, secondVector3.Value);
		return TaskStatus.Success;
	}

	public override void OnReset() {
		firstVector3 = secondVector3 = Vector3.zero;
		storeResult = 0.0f;
	}
}