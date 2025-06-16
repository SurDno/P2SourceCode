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
  [FactoryProxy(typeof (Selector))]
  [TaskDescription("The selector task is similar to an \"or\" operation. It will return success as soon as one of its child tasks return success. If a child task returns failure then it will sequentially run the next task. If no child task returns success then it will return failure.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=26")]
  [TaskIcon("{SkinColor}SelectorIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Selector : Composite, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private int currentChildIndex = 0;
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList<Task>(writer, "Children", this.children);
      DefaultDataWriteUtility.WriteEnum<AbortType>(writer, "AbortType", this.abortType);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
      this.abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
    }

    public override int CurrentChildIndex() => this.currentChildIndex;

    public override bool CanExecute()
    {
      return this.currentChildIndex < this.children.Count && this.executionStatus != TaskStatus.Success;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      ++this.currentChildIndex;
      this.executionStatus = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      this.currentChildIndex = childIndex;
      this.executionStatus = TaskStatus.Inactive;
    }

    public override void OnEnd()
    {
      this.executionStatus = TaskStatus.Inactive;
      this.currentChildIndex = 0;
    }
  }
}
