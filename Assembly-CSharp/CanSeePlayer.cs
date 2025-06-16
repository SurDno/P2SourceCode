using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

[TaskDescription("Can see Player (with Behaviour and Info engine components)")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (CanSeePlayer))]
public class CanSeePlayer : CanSee, IStub, ISerializeDataWrite, ISerializeDataRead
{
  protected override bool Filter(DetectableComponent detectable)
  {
    if (detectable.IsDisposed || detectable.Owner.GetComponent<IAttackerPlayerComponent>() == null)
      return false;
    return DetectType == DetectType.None || DetectType == detectable.VisibleDetectType;
  }

  public new void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "ResultList", ResultList);
    DefaultDataWriteUtility.WriteEnum(writer, "DetectType", DetectType);
  }

  public new void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
    ResultList = BehaviorTreeDataReadUtility.ReadShared(reader, "ResultList", ResultList);
    DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
  }
}
