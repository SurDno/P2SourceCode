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
  [FactoryProxy(typeof (SetSharedFloat))]
  [TaskCategory("Basic/SharedVariable")]
  [TaskDescription("Sets the SharedFloat variable to the specified object. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetSharedFloat : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The value to set the SharedFloat to")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat targetValue;
    [RequiredField]
    [Tooltip("The SharedFloat to set")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat targetVariable;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetValue", targetValue);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetVariable", targetVariable);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      targetValue = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetValue", targetValue);
      targetVariable = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetVariable", targetVariable);
    }

    public override TaskStatus OnUpdate()
    {
      targetVariable.Value = targetValue.Value;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      targetValue = 0.0f;
      targetVariable = 0.0f;
    }
  }
}
