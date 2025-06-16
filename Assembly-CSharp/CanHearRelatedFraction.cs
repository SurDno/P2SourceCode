using System;
using System.Collections.Generic;
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
using UnityEngine;

[TaskDescription("Can see member of related fraction")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(CanHearRelatedFraction))]
public class CanHearRelatedFraction : CanHear, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public FractionRelationEnum Relation;

	private List<FractionEnum> relatedFractions;

	public override TaskStatus OnUpdate() {
		relatedFractions = RelatedFractionUtility.GetFraction(entity, Relation);
		return base.OnUpdate();
	}

	protected override bool Filter(DetectableComponent detectable) {
		if (detectable.IsDisposed || relatedFractions == null)
			return false;
		var byName = detectable?.Owner?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.Dead);
		return (byName == null || !byName.Value) &&
		       relatedFractions.Contains(FractionsHelper.GetTargetFraction(detectable.Owner, entity));
	}

	public new void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "ResultList", ResultList);
		DefaultDataWriteUtility.WriteEnum(writer, "DetectType", DetectType);
		DefaultDataWriteUtility.WriteEnum(writer, "Relation", Relation);
	}

	public new void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
		ResultList = BehaviorTreeDataReadUtility.ReadShared(reader, "ResultList", ResultList);
		DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
		Relation = DefaultDataReadUtility.ReadEnum<FractionRelationEnum>(reader, "Relation");
	}
}