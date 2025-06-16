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
  [FactoryProxy(typeof (SetBool))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Sets a bool value")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetBool : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The bool value to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool boolValue;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The variable to store the result")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool storeResult;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "BoolValue", this.boolValue);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "StoreResult", this.storeResult);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.boolValue = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "BoolValue", this.boolValue);
      this.storeResult = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "StoreResult", this.storeResult);
    }

    public override TaskStatus OnUpdate()
    {
      this.storeResult.Value = this.boolValue.Value;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.boolValue.Value = false;
      this.storeResult.Value = false;
    }
  }
}
