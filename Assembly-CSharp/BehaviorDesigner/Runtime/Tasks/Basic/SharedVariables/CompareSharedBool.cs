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
  [FactoryProxy(typeof (CompareSharedBool))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CompareSharedBool : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The first variable to compare")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool variable;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The variable to compare to")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool compareTo;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Variable", this.variable);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "CompareTo", this.compareTo);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.variable = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Variable", this.variable);
      this.compareTo = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "CompareTo", this.compareTo);
    }

    public override TaskStatus OnUpdate()
    {
      return this.variable.Value.Equals(this.compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnReset()
    {
      this.variable = (SharedBool) false;
      this.compareTo = (SharedBool) false;
    }
  }
}
