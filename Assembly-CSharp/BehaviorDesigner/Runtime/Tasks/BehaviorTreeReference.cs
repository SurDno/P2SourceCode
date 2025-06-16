using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (BehaviorTreeReference))]
  [TaskDescription("Behavior Tree Reference allows you to run another behavior tree within the current behavior tree.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=53")]
  [TaskIcon("BehaviorTreeReferenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class BehaviorTreeReference : 
    BehaviorReference,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteUnityArray<ExternalBehaviorTree>(writer, "ExternalBehaviors", externalBehaviors);
      BehaviorTreeDataWriteUtility.WriteSharedArray(writer, "Variables", variables);
      DefaultDataWriteUtility.Write(writer, "Collapsed", collapsed);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      externalBehaviors = BehaviorTreeDataReadUtility.ReadUnityArray<ExternalBehaviorTree>(reader, "ExternalBehaviors", externalBehaviors);
      variables = BehaviorTreeDataReadUtility.ReadSharedArray(reader, "Variables", variables);
      collapsed = DefaultDataReadUtility.Read(reader, "Collapsed", collapsed);
    }
  }
}
