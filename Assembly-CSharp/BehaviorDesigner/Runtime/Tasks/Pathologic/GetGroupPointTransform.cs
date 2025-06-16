using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Sequence that locks Group Point")]
[TaskIcon("{SkinColor}SequenceIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(GetGroupPointTransform))]
public class GetGroupPointTransform : Composite, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedTransform GroupPointTransform;

	private GroupPoint point;
	private int currentChildIndex;
	protected TaskStatus executionStatus = TaskStatus.Inactive;

	public override int CurrentChildIndex() {
		return currentChildIndex;
	}

	public override bool CanExecute() {
		return currentChildIndex < children.Count && executionStatus != TaskStatus.Failure;
	}

	public override void OnChildExecuted(TaskStatus childStatus) {
		++currentChildIndex;
		executionStatus = childStatus;
	}

	public override void OnConditionalAbort(int childIndex) {
		currentChildIndex = childIndex;
		executionStatus = TaskStatus.Inactive;
	}

	public override void OnStart() {
		var service = ServiceLocator.GetService<GroupPointsService>();
		if (point != null) {
			service.AddPoint(point);
			point = null;
		}

		point = service.GetFreePoint();
		if (!(point != null))
			return;
		GroupPointTransform.Value = point.transform;
	}

	public override TaskStatus OnUpdate() {
		return !(bool)(Object)GroupPointTransform.Value ? TaskStatus.Failure : TaskStatus.Success;
	}

	public override void OnEnd() {
		executionStatus = TaskStatus.Inactive;
		currentChildIndex = 0;
		if (!(point != null))
			return;
		ServiceLocator.GetService<GroupPointsService>().AddPoint(point);
		point = null;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
		DefaultDataWriteUtility.WriteEnum(writer, "AbortType", abortType);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "GroupPointTransform", GroupPointTransform);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
		abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
		GroupPointTransform =
			BehaviorTreeDataReadUtility.ReadShared(reader, "GroupPointTransform", GroupPointTransform);
	}
}