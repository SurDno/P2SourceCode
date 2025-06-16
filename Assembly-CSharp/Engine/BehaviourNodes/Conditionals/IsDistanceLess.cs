using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is distance to NPC less than?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsDistanceLess))]
  public class IsDistanceLess : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
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
    public SharedVector3 TargetPosition;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public float Distance;

    public override TaskStatus OnUpdate()
    {
      int num;
      if (Target == null || Target.Value == null)
      {
        if (TargetPosition != null)
        {
          Vector3 vector3 = TargetPosition.Value;
          num = 0;
        }
        else
          num = 1;
      }
      else
        num = 0;
      if (num != 0)
      {
        Debug.LogWarningFormat("{0}: target is null", gameObject.name);
        return TaskStatus.Failure;
      }
      return Target != null && Target.Value != null ? ((Target.Value.transform.position - gameObject.transform.position).magnitude < (double) Distance ? TaskStatus.Success : TaskStatus.Failure) : ((TargetPosition.Value - gameObject.transform.position).magnitude < (double) Distance ? TaskStatus.Success : TaskStatus.Failure);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetPosition", TargetPosition);
      DefaultDataWriteUtility.Write(writer, "Distance", Distance);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      TargetPosition = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetPosition", TargetPosition);
      Distance = DefaultDataReadUtility.Read(reader, "Distance", Distance);
    }
  }
}
