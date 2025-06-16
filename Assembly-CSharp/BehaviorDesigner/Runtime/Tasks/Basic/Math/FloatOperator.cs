using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math;

[FactoryProxy(typeof(FloatOperator))]
[TaskCategory("Basic/Math")]
[TaskDescription("Performs a math operation on two floats: Add, Subtract, Multiply, Divide, Min, or Max.")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FloatOperator : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("The operation to perform")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public Operation operation;

	[Tooltip("The first float")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat float1;

	[Tooltip("The second float")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat float2;

	[Tooltip("The variable to store the result")] [DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat storeResult;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		DefaultDataWriteUtility.WriteEnum(writer, "Operation", operation);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Float1", float1);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Float2", float2);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "StoreResult", storeResult);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		operation = DefaultDataReadUtility.ReadEnum<Operation>(reader, "Operation");
		float1 = BehaviorTreeDataReadUtility.ReadShared(reader, "Float1", float1);
		float2 = BehaviorTreeDataReadUtility.ReadShared(reader, "Float2", float2);
		storeResult = BehaviorTreeDataReadUtility.ReadShared(reader, "StoreResult", storeResult);
	}

	public override TaskStatus OnUpdate() {
		switch (operation) {
			case Operation.Add:
				storeResult.Value = float1.Value + float2.Value;
				break;
			case Operation.Subtract:
				storeResult.Value = float1.Value - float2.Value;
				break;
			case Operation.Multiply:
				storeResult.Value = float1.Value * float2.Value;
				break;
			case Operation.Divide:
				storeResult.Value = float1.Value / float2.Value;
				break;
			case Operation.Min:
				storeResult.Value = Mathf.Min(float1.Value, float2.Value);
				break;
			case Operation.Max:
				storeResult.Value = Mathf.Max(float1.Value, float2.Value);
				break;
			case Operation.Modulo:
				storeResult.Value = float1.Value % float2.Value;
				break;
		}

		return TaskStatus.Success;
	}

	public override void OnReset() {
		operation = Operation.Add;
		float1.Value = 0.0f;
		float2.Value = 0.0f;
		storeResult.Value = 0.0f;
	}

	public enum Operation {
		Add,
		Subtract,
		Multiply,
		Divide,
		Min,
		Max,
		Modulo
	}
}