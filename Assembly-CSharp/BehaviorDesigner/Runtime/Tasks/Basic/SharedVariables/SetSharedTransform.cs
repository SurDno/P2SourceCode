using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
  [FactoryProxy(typeof (SetSharedTransform))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Sets the SharedTransform variable to the specified object. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetSharedTransform : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set the SharedTransform to. If null the variable will be set to the current Transform")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform targetValue;
    [RequiredField]
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The SharedTransform to set")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform targetVariable;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "TargetValue", this.targetValue);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "TargetVariable", this.targetVariable);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetValue = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "TargetValue", this.targetValue);
      this.targetVariable = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "TargetVariable", this.targetVariable);
    }

    public override TaskStatus OnUpdate()
    {
      this.targetVariable.Value = (UnityEngine.Object) this.targetValue.Value != (UnityEngine.Object) null ? this.targetValue.Value : this.transform;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.targetValue = (SharedTransform) null;
      this.targetVariable = (SharedTransform) null;
    }
  }
}
