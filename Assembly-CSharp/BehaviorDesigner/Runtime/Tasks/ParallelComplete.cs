using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

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
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
      DefaultDataWriteUtility.WriteEnum(writer, "AbortType", abortType);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
      abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
    }

    public override void OnAwake() => executionStatus = new TaskStatus[children.Count];

    public override void OnChildStarted(int childIndex)
    {
      ++currentChildIndex;
      executionStatus[childIndex] = TaskStatus.Running;
    }

    public override bool CanRunParallelChildren() => true;

    public override int CurrentChildIndex() => currentChildIndex;

    public override bool CanExecute() => currentChildIndex < children.Count;

    public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
    {
      executionStatus[childIndex] = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      currentChildIndex = 0;
      for (int index = 0; index < executionStatus.Length; ++index)
        executionStatus[index] = TaskStatus.Inactive;
    }

    public override TaskStatus OverrideStatus(TaskStatus status)
    {
      for (int index = 0; index < executionStatus.Length; ++index)
      {
        if (executionStatus[index] == TaskStatus.Success || executionStatus[index] == TaskStatus.Failure)
          return executionStatus[index];
        if (executionStatus[index] == TaskStatus.Inactive)
          return TaskStatus.Success;
      }
      return TaskStatus.Running;
    }

    public override void OnEnd()
    {
      for (int index = 0; index < executionStatus.Length; ++index)
        executionStatus[index] = TaskStatus.Inactive;
      currentChildIndex = 0;
    }
  }
}
