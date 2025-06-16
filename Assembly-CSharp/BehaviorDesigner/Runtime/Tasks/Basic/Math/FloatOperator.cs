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
  [FactoryProxy(typeof (FloatOperator))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Performs a math operation on two floats: Add, Subtract, Multiply, Divide, Min, or Max.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FloatOperator : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The operation to perform")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public FloatOperator.Operation operation;
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
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The variable to store the result")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat storeResult;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      DefaultDataWriteUtility.WriteEnum<FloatOperator.Operation>(writer, "Operation", this.operation);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Float1", this.float1);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Float2", this.float2);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StoreResult", this.storeResult);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.operation = DefaultDataReadUtility.ReadEnum<FloatOperator.Operation>(reader, "Operation");
      this.float1 = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Float1", this.float1);
      this.float2 = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Float2", this.float2);
      this.storeResult = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StoreResult", this.storeResult);
    }

    public override TaskStatus OnUpdate()
    {
      switch (this.operation)
      {
        case FloatOperator.Operation.Add:
          this.storeResult.Value = this.float1.Value + this.float2.Value;
          break;
        case FloatOperator.Operation.Subtract:
          this.storeResult.Value = this.float1.Value - this.float2.Value;
          break;
        case FloatOperator.Operation.Multiply:
          this.storeResult.Value = this.float1.Value * this.float2.Value;
          break;
        case FloatOperator.Operation.Divide:
          this.storeResult.Value = this.float1.Value / this.float2.Value;
          break;
        case FloatOperator.Operation.Min:
          this.storeResult.Value = Mathf.Min(this.float1.Value, this.float2.Value);
          break;
        case FloatOperator.Operation.Max:
          this.storeResult.Value = Mathf.Max(this.float1.Value, this.float2.Value);
          break;
        case FloatOperator.Operation.Modulo:
          this.storeResult.Value = this.float1.Value % this.float2.Value;
          break;
      }
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.operation = FloatOperator.Operation.Add;
      this.float1.Value = 0.0f;
      this.float2.Value = 0.0f;
      this.storeResult.Value = 0.0f;
    }

    public enum Operation
    {
      Add,
      Subtract,
      Multiply,
      Divide,
      Min,
      Max,
      Modulo,
    }
  }
}
