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
  [FactoryProxy(typeof (ParallelSelector))]
  [TaskDescription("Similar to the selector task, the parallel selector task will return success as soon as a child task returns success. The difference is that the parallel task will run all of its children tasks simultaneously versus running each task one at a time. If one tasks returns success the parallel selector task will end all of the child tasks and return success. If every child task returns failure then the parallel selector task will return failure.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=28")]
  [TaskIcon("{SkinColor}ParallelSelectorIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ParallelSelector : Composite, IStub, ISerializeDataWrite, ISerializeDataRead
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
      bool flag = true;
      for (int index = 0; index < executionStatus.Length; ++index)
      {
        if (executionStatus[index] == TaskStatus.Running)
          flag = false;
        else if (executionStatus[index] == TaskStatus.Success)
          return TaskStatus.Success;
      }
      return flag ? TaskStatus.Failure : TaskStatus.Running;
    }

    public override void OnEnd()
    {
      for (int index = 0; index < executionStatus.Length; ++index)
        executionStatus[index] = TaskStatus.Inactive;
      currentChildIndex = 0;
    }
  }
}
