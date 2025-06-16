using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

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
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt PointIndex;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool InversePath = false;
    private bool success;

    public override void OnStart()
    {
      success = false;
      if (!(bool) (UnityEngine.Object) Target.Value)
        return;
      PatrolPath component = Target.Value.GetComponent<PatrolPath>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        if (component.GetPresetPath(PointIndex.Value, InversePath.Value) == null)
          return;
        success = true;
      }
      else
        Debug.LogError((object) (gameObject.name + " has wrong patrol path object! Needs PatrolPath script"));
    }

    public override TaskStatus OnUpdate() => success ? TaskStatus.Success : TaskStatus.Failure;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PointIndex", PointIndex);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InversePath", InversePath);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      PointIndex = BehaviorTreeDataReadUtility.ReadShared(reader, "PointIndex", PointIndex);
      InversePath = BehaviorTreeDataReadUtility.ReadShared(reader, "InversePath", InversePath);
    }
  }
}
