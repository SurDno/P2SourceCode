using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Bird Flock Follow")]
  [TaskCategory("Pathologic/BirdFlock")]
  [TaskIcon("Pathologic_CrowIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (BirdFlockFollow))]
  public class BirdFlockFollow : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Adds to target position")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 Offset;
    protected EngineBehavior behavior;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.behavior == (UnityEngine.Object) null)
      {
        this.behavior = this.gameObject.GetComponent<EngineBehavior>();
        if ((UnityEngine.Object) this.behavior == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain Behavior unity component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      Vector3 direction = this.Offset.Value + this.Target.Value.transform.position - this.gameObject.transform.position;
      float magnitude = direction.magnitude;
      this.behavior.Move(direction, magnitude);
      return (double) magnitude < 1.0 ? TaskStatus.Success : TaskStatus.Running;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "Offset", this.Offset);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.Offset = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "Offset", this.Offset);
    }
  }
}
