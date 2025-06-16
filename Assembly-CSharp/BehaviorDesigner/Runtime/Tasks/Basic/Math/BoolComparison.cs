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
  [FactoryProxy(typeof (BoolComparison))]
  [TaskCategory("Basic/Math")]
  [TaskDescription("Performs a comparison between two bools.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BoolComparison : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The first bool")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool bool1;
    [Tooltip("The second bool")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool bool2;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Bool1", bool1);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Bool2", bool2);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      bool1 = BehaviorTreeDataReadUtility.ReadShared(reader, "Bool1", bool1);
      bool2 = BehaviorTreeDataReadUtility.ReadShared(reader, "Bool2", bool2);
    }

    public override TaskStatus OnUpdate()
    {
      return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnReset()
    {
      bool1.Value = false;
      bool2.Value = false;
    }
  }
}
