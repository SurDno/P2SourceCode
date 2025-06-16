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
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The operation to perform")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public FloatComparison.Operation operation;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The first float")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat float1;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The second float")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat float2;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      DefaultDataWriteUtility.WriteEnum<FloatComparison.Operation>(writer, "Operation", this.operation);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Float1", this.float1);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Float2", this.float2);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.operation = DefaultDataReadUtility.ReadEnum<FloatComparison.Operation>(reader, "Operation");
      this.float1 = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Float1", this.float1);
      this.float2 = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Float2", this.float2);
    }

    public override TaskStatus OnUpdate()
    {
      switch (this.operation)
      {
        case FloatComparison.Operation.LessThan:
          return (double) this.float1.Value < (double) this.float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case FloatComparison.Operation.LessThanOrEqualTo:
          return (double) this.float1.Value <= (double) this.float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case FloatComparison.Operation.EqualTo:
          return (double) this.float1.Value == (double) this.float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case FloatComparison.Operation.NotEqualTo:
          return (double) this.float1.Value != (double) this.float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case FloatComparison.Operation.GreaterThanOrEqualTo:
          return (double) this.float1.Value >= (double) this.float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        case FloatComparison.Operation.GreaterThan:
          return (double) this.float1.Value > (double) this.float2.Value ? TaskStatus.Success : TaskStatus.Failure;
        default:
          return TaskStatus.Failure;
      }
    }

    public override void OnReset()
    {
      this.operation = FloatComparison.Operation.LessThan;
      this.float1.Value = 0.0f;
      this.float2.Value = 0.0f;
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
