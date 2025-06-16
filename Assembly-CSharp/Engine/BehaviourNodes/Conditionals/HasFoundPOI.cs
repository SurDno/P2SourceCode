using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("does parent sequence have poi object?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (HasFoundPOI))]
  public class HasFoundPOI : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.CustomTaskReference)]
    [DataWriteProxy(MemberEnum.CustomTaskReference)]
    [CopyableProxy()]
    [SerializeField]
    public POISequence ReferencedPOISequence;

    public override void OnStart()
    {
      if (ReferencedPOISequence != null)
        return;
      Debug.LogWarning((object) ("poi sequence not connected to has found poi node! " + (object) Owner.gameObject));
    }

    public override TaskStatus OnUpdate()
    {
      return ReferencedPOISequence != null ? ((UnityEngine.Object) ReferencedPOISequence.OutPOI != (UnityEngine.Object) null ? TaskStatus.Success : TaskStatus.Failure) : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteTaskReference(writer, "ReferencedPOISequence", ReferencedPOISequence);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      ReferencedPOISequence = BehaviorTreeDataReadUtility.ReadTaskReference(reader, "ReferencedPOISequence", ReferencedPOISequence);
    }
  }
}
