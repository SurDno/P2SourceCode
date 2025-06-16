using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Next patrol point is preseted path part")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (NextPatrolPointIsPathPart))]
  public class NextPatrolPointIsPathPart : 
    Conditional,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt PointIndex;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool InversePath = (SharedBool) false;
    private bool success = false;

    public override void OnStart()
    {
      this.success = false;
      if (!(bool) (UnityEngine.Object) this.Target.Value)
        return;
      PatrolPath component = this.Target.Value.GetComponent<PatrolPath>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        if (component.GetPresetPath(this.PointIndex.Value, this.InversePath.Value) == null)
          return;
        this.success = true;
      }
      else
        Debug.LogError((object) (this.gameObject.name + " has wrong patrol path object! Needs PatrolPath script"));
    }

    public override TaskStatus OnUpdate() => this.success ? TaskStatus.Success : TaskStatus.Failure;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PointIndex", this.PointIndex);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "InversePath", this.InversePath);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.PointIndex = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PointIndex", this.PointIndex);
      this.InversePath = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "InversePath", this.InversePath);
    }
  }
}
