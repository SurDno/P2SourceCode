using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
  [FactoryProxy(typeof (SetInt))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Sets an int value")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetInt : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The int value to set")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt intValue;
    [Tooltip("The variable to store the result")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedInt storeResult;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "IntValue", intValue);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StoreResult", storeResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      intValue = BehaviorTreeDataReadUtility.ReadShared(reader, "IntValue", intValue);
      storeResult = BehaviorTreeDataReadUtility.ReadShared(reader, "StoreResult", storeResult);
    }

    public override TaskStatus OnUpdate()
    {
      storeResult.Value = intValue.Value;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      intValue.Value = 0;
      storeResult.Value = 0;
    }
  }
}
