using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[FactoryProxy(typeof(Repeater))]
[TaskDescription(
	"The repeater task will repeat execution of its child task until the child task has been run a specified number of times. It has the option of continuing to execute the child task even if the child task returns a failure.")]
[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=37")]
[TaskIcon("{SkinColor}RepeaterIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Repeater : Decorator, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("The number of times to repeat the execution of its child task")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedInt count = 1;

	[Tooltip("Allows the repeater to repeat forever")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedBool repeatForever;

	[Tooltip("Should the task return if the child task returns a failure")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[SerializeField]
	public SharedBool endOnFailure;

	private int executionCount;
	private TaskStatus executionStatus = TaskStatus.Inactive;

	public new void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Count", count);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "RepeatForever", repeatForever);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "EndOnFailure", endOnFailure);
	}

	public new void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
		count = BehaviorTreeDataReadUtility.ReadShared(reader, "Count", count);
		repeatForever = BehaviorTreeDataReadUtility.ReadShared(reader, "RepeatForever", repeatForever);
		endOnFailure = BehaviorTreeDataReadUtility.ReadShared(reader, "EndOnFailure", endOnFailure);
	}

	public override bool CanExecute() {
		return (repeatForever.Value || executionCount < count.Value) &&
		       (!endOnFailure.Value || (endOnFailure.Value && executionStatus != TaskStatus.Failure));
	}

	public override void OnChildExecuted(TaskStatus childStatus) {
		++executionCount;
		executionStatus = childStatus;
	}

	public override void OnEnd() {
		executionCount = 0;
		executionStatus = TaskStatus.Inactive;
	}

	public override void OnReset() {
		count = 0;
		endOnFailure = true;
	}
}