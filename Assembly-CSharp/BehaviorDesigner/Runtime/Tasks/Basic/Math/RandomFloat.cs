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
  [FactoryProxy(typeof (RandomFloat))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Sets a random float value")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RandomFloat : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum amount")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat min;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum amount")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat max;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Is the maximum value inclusive?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public bool inclusive;
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
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Min", this.min);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Max", this.max);
      DefaultDataWriteUtility.Write(writer, "Inclusive", this.inclusive);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StoreResult", this.storeResult);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.min = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Min", this.min);
      this.max = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Max", this.max);
      this.inclusive = DefaultDataReadUtility.Read(reader, "Inclusive", this.inclusive);
      this.storeResult = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StoreResult", this.storeResult);
    }

    public override TaskStatus OnUpdate()
    {
      if (this.inclusive)
        this.storeResult.Value = UnityEngine.Random.Range(this.min.Value, this.max.Value + 1f);
      else
        this.storeResult.Value = UnityEngine.Random.Range(this.min.Value, this.max.Value);
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.min.Value = 0.0f;
      this.max.Value = 0.0f;
      this.inclusive = false;
      this.storeResult.Value = 0.0f;
    }
  }
}
