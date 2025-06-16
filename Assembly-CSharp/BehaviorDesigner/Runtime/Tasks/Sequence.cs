using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks;

[FactoryProxy(typeof(Sequence))]
[TaskDescription(
	"The sequence task is similar to an \"and\" operation. It will return failure as soon as one of its child tasks return failure. If a child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.")]
[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=25")]
[TaskIcon("{SkinColor}SequenceIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Sequence : Composite, IStub, ISerializeDataWrite, ISerializeDataRead {
	private int currentChildIndex;
	private TaskStatus executionStatus = TaskStatus.Inactive;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
		DefaultDataWriteUtility.WriteEnum(writer, "AbortType", abortType);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
		abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
	}

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

	public override void OnEnd() {
		executionStatus = TaskStatus.Inactive;
		currentChildIndex = 0;
	}
}