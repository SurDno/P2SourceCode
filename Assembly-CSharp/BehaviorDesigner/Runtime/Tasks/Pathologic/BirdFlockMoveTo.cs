using System;
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
  [TaskDescription("Bird Flock Move To")]
  [TaskCategory("Pathologic/BirdFlock")]
  [TaskIcon("Pathologic_CrowIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (BirdFlockMoveTo))]
  public class BirdFlockMoveTo : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedVector3 Target;
    [Tooltip("Adds to target")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedVector3 Offset;
    protected EngineBehavior behavior;

    public override TaskStatus OnUpdate()
    {
      if (behavior == null)
      {
        behavior = gameObject.GetComponent<EngineBehavior>();
        if (behavior == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (EngineBehavior).Name + " unity component", gameObject);
          return TaskStatus.Failure;
        }
      }
      Vector3 direction = Offset.Value + Target.Value - gameObject.transform.position;
      float magnitude = direction.magnitude;
      behavior.Move(direction, magnitude);
      return magnitude < 1.0 ? TaskStatus.Success : TaskStatus.Running;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Offset", Offset);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      Offset = BehaviorTreeDataReadUtility.ReadShared(reader, "Offset", Offset);
    }
  }
}
