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

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get path from transform")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetPathFromTransform))]
  public class GetPathFromTransform : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransformList Result;
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
      PathPart component1 = Target.Value.GetComponent<PathPart>();
      if (component1 != null)
      {
        Result.Value = component1.PointsList;
        success = true;
      }
      else
      {
        PatrolPath component2 = Target.Value.GetComponent<PatrolPath>();
        if (component2 != null)
        {
          List<Transform> presetPath = component2.GetPresetPath(PointIndex.Value, InversePath.Value);
          if (presetPath != null)
          {
            Result.Value = presetPath;
            success = true;
            return;
          }
        }
        Debug.LogError(gameObject.name + "  has wrong path!");
      }
    }

    public override TaskStatus OnUpdate()
    {
      return !success ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
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
      Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      PointIndex = BehaviorTreeDataReadUtility.ReadShared(reader, "PointIndex", PointIndex);
      InversePath = BehaviorTreeDataReadUtility.ReadShared(reader, "InversePath", InversePath);
    }
  }
}
