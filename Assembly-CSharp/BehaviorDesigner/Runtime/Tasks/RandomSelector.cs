using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (RandomSelector))]
  [TaskDescription("Similar to the selector task, the random selector task will return success as soon as a child task returns success.  The difference is that the random selector class will run its children in a random order. The selector task is deterministic in that it will always run the tasks from left to right within the tree. The random selector task shuffles the child tasks up and then begins execution in a random order. Other than that the random selector class is the same as the selector class. It will continue running tasks until a task completes successfully. If no child tasks return success then it will return failure.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=30")]
  [TaskIcon("{SkinColor}RandomSelectorIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RandomSelector : Composite, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Seed the random number generator to make things easier to debug")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public int seed;
    [Tooltip("Do we want to use the seed?")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public bool useSeed;
    private List<int> childIndexList = new List<int>();
    private Stack<int> childrenExecutionOrder = new Stack<int>();
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
      DefaultDataWriteUtility.WriteEnum(writer, "AbortType", abortType);
      DefaultDataWriteUtility.Write(writer, "Seed", seed);
      DefaultDataWriteUtility.Write(writer, "UseSeed", useSeed);
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
      seed = DefaultDataReadUtility.Read(reader, "Seed", seed);
      useSeed = DefaultDataReadUtility.Read(reader, "UseSeed", useSeed);
    }

    public override void OnAwake()
    {
      if (useSeed)
        Random.InitState(seed);
      childIndexList.Clear();
      for (int index = 0; index < children.Count; ++index)
        childIndexList.Add(index);
    }

    public override void OnStart() => ShuffleChilden();

    public override int CurrentChildIndex() => childrenExecutionOrder.Peek();

    public override bool CanExecute()
    {
      return childrenExecutionOrder.Count > 0 && executionStatus != TaskStatus.Success;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      if (childrenExecutionOrder.Count > 0)
        childrenExecutionOrder.Pop();
      executionStatus = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      childrenExecutionOrder.Clear();
      executionStatus = TaskStatus.Inactive;
      ShuffleChilden();
    }

    public override void OnEnd()
    {
      executionStatus = TaskStatus.Inactive;
      childrenExecutionOrder.Clear();
    }

    public override void OnReset()
    {
      seed = 0;
      useSeed = false;
    }

    private void ShuffleChilden()
    {
      for (int count = childIndexList.Count; count > 0; --count)
      {
        int index = Random.Range(0, count);
        int childIndex = childIndexList[index];
        childrenExecutionOrder.Push(childIndex);
        childIndexList[index] = childIndexList[count - 1];
        childIndexList[count - 1] = childIndex;
      }
    }
  }
}
