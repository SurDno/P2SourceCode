using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
  [FactoryProxy(typeof (FloatComparison))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Performs comparison between two floats: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FloatComparison : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The operation to perform")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public Operation operation;
    [Tooltip("The first float")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat float1;
    [Tooltip("The second float")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat float2;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      DefaultDataWriteUtility.WriteEnum(writer, "Operation", operation);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Float1", float1);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Float2", float2);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      operation = DefaultDataReadUtility.ReadEnum<Operation>(reader, "Operation");
      float1 = BehaviorTreeDataReadUtility.ReadShared(reader, "Float1", float1);
      float2 = BehaviorTreeDataReadUtility.ReadShared(reader, "Float2", float2);
    }

    public override TaskStatus OnUpdate()
    {
      switch (operation)
      {
        case Operation.LessThan:
          return float1.Value < (double) float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case Operation.LessThanOrEqualTo:
          return float1.Value <= (double) float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case Operation.EqualTo:
          return float1.Value == (double) float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case Operation.NotEqualTo:
          return float1.Value != (double) float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case Operation.GreaterThanOrEqualTo:
          return float1.Value >= (double) float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case Operation.GreaterThan:
          return float1.Value > (double) float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        default:
          return TaskStatus.Failure;
      }
    }

    public override void OnReset()
    {
      operation = Operation.LessThan;
      float1.Value = 0.0f;
      float2.Value = 0.0f;
    }

    public enum Operation
    {
      LessThan,
      LessThanOrEqualTo,
      EqualTo,
      NotEqualTo,
      GreaterThanOrEqualTo,
      GreaterThan,
    }
  }
}
