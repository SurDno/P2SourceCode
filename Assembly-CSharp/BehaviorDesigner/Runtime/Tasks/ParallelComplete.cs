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
  [FactoryProxy(typeof (ParallelComplete))]
  [TaskDescription("Similar to the parallel selector task, except the parallel complete task will return the child status as soon as the child returns success or failure.The child tasks are executed simultaneously.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=136")]
  [TaskIcon("{SkinColor}ParallelCompleteIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ParallelComplete : Composite, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private int currentChildIndex;
    private TaskStatus[] executionStatus;

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

    public override void OnAwake() => this.executionStatus = new TaskStatus[this.children.Count];

    public override void OnChildStarted(int childIndex)
    {
      ++this.currentChildIndex;
      this.executionStatus[childIndex] = TaskStatus.Running;
    }

    public override bool CanRunParallelChildren() => true;

    public override int CurrentChildIndex() => this.currentChildIndex;

    public override bool CanExecute() => this.currentChildIndex < this.children.Count;

    public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
    {
      this.executionStatus[childIndex] = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      this.currentChildIndex = 0;
      for (int index = 0; index < this.executionStatus.Length; ++index)
        this.executionStatus[index] = TaskStatus.Inactive;
    }

    public override TaskStatus OverrideStatus(TaskStatus status)
    {
      for (int index = 0; index < this.executionStatus.Length; ++index)
      {
        if (this.executionStatus[index] == TaskStatus.Success || this.executionStatus[index] == TaskStatus.Failure)
          return this.executionStatus[index];
        if (this.executionStatus[index] == TaskStatus.Inactive)
          return TaskStatus.Success;
      }
      return TaskStatus.Running;
    }

    public override void OnEnd()
    {
      for (int index = 0; index < this.executionStatus.Length; ++index)
        this.executionStatus[index] = TaskStatus.Inactive;
      this.currentChildIndex = 0;
    }
  }
}
