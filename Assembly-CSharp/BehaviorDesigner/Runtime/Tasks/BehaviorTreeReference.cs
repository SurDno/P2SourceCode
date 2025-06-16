using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;

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
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteUnityArray<ExternalBehaviorTree>(writer, "ExternalBehaviors", this.externalBehaviors);
      BehaviorTreeDataWriteUtility.WriteSharedArray<SharedNamedVariable>(writer, "Variables", this.variables);
      DefaultDataWriteUtility.Write(writer, "Collapsed", this.collapsed);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.externalBehaviors = BehaviorTreeDataReadUtility.ReadUnityArray<ExternalBehaviorTree>(reader, "ExternalBehaviors", this.externalBehaviors);
      this.variables = BehaviorTreeDataReadUtility.ReadSharedArray<SharedNamedVariable>(reader, "Variables", this.variables);
      this.collapsed = DefaultDataReadUtility.Read(reader, "Collapsed", this.collapsed);
    }
  }
}
