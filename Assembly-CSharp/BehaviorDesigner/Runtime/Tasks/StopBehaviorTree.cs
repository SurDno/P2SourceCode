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
  [FactoryProxy(typeof (StopBehaviorTree))]
  [TaskDescription("Pause or disable a behavior tree and return success after it has been stopped.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=21")]
  [TaskIcon("{SkinColor}StopBehaviorTreeIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class StopBehaviorTree : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The GameObject of the behavior tree that should be stopped. If null use the current behavior")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject behaviorGameObject;
    [Tooltip("The group of the behavior tree that should be stopped")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt group;
    [Tooltip("Should the behavior be paused or completely disabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool pauseBehavior = (SharedBool) false;
    private BehaviorTree behavior;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "BehaviorGameObject", this.behaviorGameObject);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "Group", this.group);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "PauseBehavior", this.pauseBehavior);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.behaviorGameObject = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "BehaviorGameObject", this.behaviorGameObject);
      this.group = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "Group", this.group);
      this.pauseBehavior = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "PauseBehavior", this.pauseBehavior);
    }

    public override void OnStart()
    {
      BehaviorTree[] components = this.GetDefaultGameObject(this.behaviorGameObject.Value).GetComponents<BehaviorTree>();
      if (components.Length == 1)
      {
        this.behavior = components[0];
      }
      else
      {
        if (components.Length <= 1 || !((UnityEngine.Object) this.behavior == (UnityEngine.Object) null))
          return;
        this.behavior = components[0];
      }
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.behavior == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      this.behavior.DisableBehavior(this.pauseBehavior.Value);
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      this.behaviorGameObject = (SharedGameObject) null;
      this.group = (SharedInt) 0;
      this.pauseBehavior = (SharedBool) false;
    }
  }
}
