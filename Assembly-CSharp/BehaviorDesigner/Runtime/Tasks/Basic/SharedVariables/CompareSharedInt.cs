using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
  [FactoryProxy(typeof (CompareSharedInt))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class CompareSharedInt : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The first variable to compare")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt variable;
    [Tooltip("The variable to compare to")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedInt compareTo;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Variable", variable);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "CompareTo", compareTo);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      variable = BehaviorTreeDataReadUtility.ReadShared(reader, "Variable", variable);
      compareTo = BehaviorTreeDataReadUtility.ReadShared(reader, "CompareTo", compareTo);
    }

    public override TaskStatus OnUpdate()
    {
      return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnReset()
    {
      variable = 0;
      compareTo = 0;
    }
  }
}
