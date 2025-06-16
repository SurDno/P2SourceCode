using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System.Collections.Generic;
using UnityEngine;

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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public int seed = 0;
    [Tooltip("Do we want to use the seed?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public bool useSeed = false;
    private List<int> childIndexList = new List<int>();
    private Stack<int> childrenExecutionOrder = new Stack<int>();
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
      DefaultDataWriteUtility.Write(writer, "Seed", this.seed);
      DefaultDataWriteUtility.Write(writer, "UseSeed", this.useSeed);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
      this.abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
      this.seed = DefaultDataReadUtility.Read(reader, "Seed", this.seed);
      this.useSeed = DefaultDataReadUtility.Read(reader, "UseSeed", this.useSeed);
    }

    public override void OnAwake()
    {
      if (this.useSeed)
        UnityEngine.Random.InitState(this.seed);
      this.childIndexList.Clear();
      for (int index = 0; index < this.children.Count; ++index)
        this.childIndexList.Add(index);
    }

    public override void OnStart() => this.ShuffleChilden();

    public override int CurrentChildIndex() => this.childrenExecutionOrder.Peek();

    public override bool CanExecute()
    {
      return this.childrenExecutionOrder.Count > 0 && this.executionStatus != TaskStatus.Success;
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      if (this.childrenExecutionOrder.Count > 0)
        this.childrenExecutionOrder.Pop();
      this.executionStatus = childStatus;
    }

    public override void OnConditionalAbort(int childIndex)
    {
      this.childrenExecutionOrder.Clear();
      this.executionStatus = TaskStatus.Inactive;
      this.ShuffleChilden();
    }

    public override void OnEnd()
    {
      this.executionStatus = TaskStatus.Inactive;
      this.childrenExecutionOrder.Clear();
    }

    public override void OnReset()
    {
      this.seed = 0;
      this.useSeed = false;
    }

    private void ShuffleChilden()
    {
      for (int count = this.childIndexList.Count; count > 0; --count)
      {
        int index = UnityEngine.Random.Range(0, count);
        int childIndex = this.childIndexList[index];
        this.childrenExecutionOrder.Push(childIndex);
        this.childIndexList[index] = this.childIndexList[count - 1];
        this.childIndexList[count - 1] = childIndex;
      }
    }
  }
}
