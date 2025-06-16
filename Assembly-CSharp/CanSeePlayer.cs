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
using System;

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
    return this.DetectType == DetectType.None || this.DetectType == detectable.VisibleDetectType;
  }

  public new void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Result", this.Result);
    BehaviorTreeDataWriteUtility.WriteShared<SharedTransformList>(writer, "ResultList", this.ResultList);
    DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "DetectType", this.DetectType);
  }

  public new void DataRead(IDataReader reader, Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Result", this.Result);
    this.ResultList = BehaviorTreeDataReadUtility.ReadShared<SharedTransformList>(reader, "ResultList", this.ResultList);
    this.DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
  }
}
