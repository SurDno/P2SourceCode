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
  [FactoryProxy(typeof (SendEvent))]
  [TaskDescription("Sends an event to the behavior tree, returns success after sending the event.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=121")]
  [TaskIcon("{SkinColor}SendEventIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SendEvent : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The GameObject of the behavior tree that should have the event sent to it. If null use the current behavior")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject targetGameObject;
    [Tooltip("The event to send")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedString eventName;
    [Tooltip("The group of the behavior tree that the event should be sent to")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt group;
    [Tooltip("Optionally specify a first argument to send")]
    [SharedRequired]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVariable argument1;
    [Tooltip("Optionally specify a second argument to send")]
    [SharedRequired]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVariable argument2;
    private BehaviorTree behaviorTree;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "TargetGameObject", this.targetGameObject);
      BehaviorTreeDataWriteUtility.WriteShared<SharedString>(writer, "EventName", this.eventName);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "Group", this.group);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVariable>(writer, "Argument1", this.argument1);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVariable>(writer, "Argument2", this.argument2);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetGameObject = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "TargetGameObject", this.targetGameObject);
      this.eventName = BehaviorTreeDataReadUtility.ReadShared<SharedString>(reader, "EventName", this.eventName);
      this.group = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "Group", this.group);
      this.argument1 = BehaviorTreeDataReadUtility.ReadShared<SharedVariable>(reader, "Argument1", this.argument1);
      this.argument2 = BehaviorTreeDataReadUtility.ReadShared<SharedVariable>(reader, "Argument2", this.argument2);
    }

    public override void OnStart()
    {
      BehaviorTree[] components = this.GetDefaultGameObject(this.targetGameObject.Value).GetComponents<BehaviorTree>();
      if (components.Length == 1)
      {
        this.behaviorTree = components[0];
      }
      else
      {
        if (components.Length <= 1 || !((UnityEngine.Object) this.behaviorTree == (UnityEngine.Object) null))
          return;
        this.behaviorTree = components[0];
      }
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.behaviorTree == (UnityEngine.Object) null)
        return TaskStatus.Success;
      if (this.argument1 == null || this.argument1.IsNone)
        this.behaviorTree.SendEvent(this.eventName.Value);
      else if (this.argument2 == null || this.argument2.IsNone)
        this.behaviorTree.SendEvent<object>(this.eventName.Value, this.argument1.GetValue());
      else
        this.behaviorTree.SendEvent<object, object>(this.eventName.Value, this.argument1.GetValue(), this.argument2.GetValue());
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.targetGameObject = (SharedGameObject) null;
      this.eventName = (SharedString) "";
    }
  }
}
