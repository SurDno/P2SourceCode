using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components.Utilities;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get closest patrol point index")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetClosestPatrolPoint))]
  public class GetClosestPatrolPoint : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform PatrolTransform;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt CurrentIndex;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Result;
    private bool inited;
    private PatrolPath patrolPath;
    private List<Transform> patrolPoints;

    public override void OnStart()
    {
      inited = false;
      patrolPath = PatrolTransform.Value.GetComponent<PatrolPath>();
      if (patrolPath == null)
      {
        Debug.LogErrorFormat("{0} has no patrol path", gameObject.name);
      }
      else
      {
        patrolPoints = patrolPath.PointsList;
        if (!patrolPath.RestartFromClosestPoint)
        {
          inited = true;
        }
        else
        {
          float num1 = float.PositiveInfinity;
          int num2 = 0;
          for (int index = 0; index < patrolPoints.Count; ++index)
          {
            if (NavMesh.SamplePosition(patrolPoints[index].position, out NavMeshHit hit, 1f, -1))
            {
              NavMeshPath path = new NavMeshPath();
              if (NavMesh.CalculatePath(hit.position, Owner.transform.position, -1, path))
              {
                float pathLength = NavMeshUtility.GetPathLength(path);
                if (pathLength < (double) num1)
                {
                  num1 = pathLength;
                  num2 = index;
                }
              }
            }
          }
          CurrentIndex.Value = num2;
          inited = true;
        }
      }
    }

    public override TaskStatus OnUpdate()
    {
      if (!inited || patrolPoints == null)
        return TaskStatus.Failure;
      if (patrolPoints.Count == 0)
      {
        Debug.LogErrorFormat("{0} has wrong patrol path in {1}", gameObject.name, PatrolTransform.Value.name);
        return TaskStatus.Failure;
      }
      if (CurrentIndex.Value >= patrolPoints.Count)
        CurrentIndex.Value = patrolPoints.Count - 1;
      Result.Value = patrolPoints[CurrentIndex.Value];
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PatrolTransform", PatrolTransform);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "CurrentIndex", CurrentIndex);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      PatrolTransform = BehaviorTreeDataReadUtility.ReadShared(reader, "PatrolTransform", PatrolTransform);
      CurrentIndex = BehaviorTreeDataReadUtility.ReadShared(reader, "CurrentIndex", CurrentIndex);
      Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
    }
  }
}
