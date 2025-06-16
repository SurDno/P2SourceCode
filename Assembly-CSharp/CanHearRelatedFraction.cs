// Decompiled with JetBrains decompiler
// Type: CanHearRelatedFraction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[TaskDescription("Can see member of related fraction")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (CanHearRelatedFraction))]
public class CanHearRelatedFraction : CanHear, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public FractionRelationEnum Relation;
  private List<FractionEnum> relatedFractions;

  public override TaskStatus OnUpdate()
  {
    this.relatedFractions = RelatedFractionUtility.GetFraction(this.entity, this.Relation);
    return base.OnUpdate();
  }

  protected override bool Filter(DetectableComponent detectable)
  {
    if (detectable.IsDisposed || this.relatedFractions == null)
      return false;
    IParameter<bool> byName = detectable?.Owner?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.Dead);
    return (byName == null || !byName.Value) && this.relatedFractions.Contains(FractionsHelper.GetTargetFraction(detectable.Owner, this.entity));
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
    DefaultDataWriteUtility.WriteEnum<FractionRelationEnum>(writer, "Relation", this.Relation);
  }

  public new void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Result", this.Result);
    this.ResultList = BehaviorTreeDataReadUtility.ReadShared<SharedTransformList>(reader, "ResultList", this.ResultList);
    this.DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
    this.Relation = DefaultDataReadUtility.ReadEnum<FractionRelationEnum>(reader, "Relation");
  }
}
