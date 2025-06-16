using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (Repeater))]
  [TaskDescription("The repeater task will repeat execution of its child task until the child task has been run a specified number of times. It has the option of continuing to execute the child task even if the child task returns a failure.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=37")]
  [TaskIcon("{SkinColor}RepeaterIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Repeater : Decorator, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The number of times to repeat the execution of its child task")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt count = (SharedInt) 1;
    [Tooltip("Allows the repeater to repeat forever")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool repeatForever;
    [Tooltip("Should the task return if the child task returns a failure")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool endOnFailure;
    private int executionCount = 0;
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteTaskList<Task>(writer, "Children", this.children);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "Count", this.count);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RepeatForever", this.repeatForever);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "EndOnFailure", this.endOnFailure);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
      this.count = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "Count", this.count);
      this.repeatForever = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RepeatForever", this.repeatForever);
      this.endOnFailure = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "EndOnFailure", this.endOnFailure);
    }

    public override bool CanExecute()
    {
      return (this.repeatForever.Value || this.executionCount < this.count.Value) && (!this.endOnFailure.Value || this.endOnFailure.Value && this.executionStatus != TaskStatus.Failure);
    }

    public override void OnChildExecuted(TaskStatus childStatus)
    {
      ++this.executionCount;
      this.executionStatus = childStatus;
    }

    public override void OnEnd()
    {
      this.executionCount = 0;
      this.executionStatus = TaskStatus.Inactive;
    }

    public override void OnReset()
    {
      this.count = (SharedInt) 0;
      this.endOnFailure = (SharedBool) true;
    }
  }
}
