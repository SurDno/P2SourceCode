using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
  [FactoryProxy(typeof (Distance))]
  [TaskCategory("Basic/Vector3")]
  [TaskDescription("Returns the distance between two Vector3s.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Distance : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The first Vector3")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 firstVector3;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The second Vector3")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 secondVector3;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance")]
    [RequiredField]
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
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "FirstVector3", this.firstVector3);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "SecondVector3", this.secondVector3);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StoreResult", this.storeResult);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.firstVector3 = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "FirstVector3", this.firstVector3);
      this.secondVector3 = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "SecondVector3", this.secondVector3);
      this.storeResult = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StoreResult", this.storeResult);
    }

    public override TaskStatus OnUpdate()
    {
      this.storeResult.Value = Vector3.Distance(this.firstVector3.Value, this.secondVector3.Value);
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.firstVector3 = this.secondVector3 = (SharedVector3) Vector3.zero;
      this.storeResult = (SharedFloat) 0.0f;
    }
  }
}
