using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
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
  [TaskDescription("Move To by path cloud")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MoveByPathCloud))]
  public class MoveByPathCloud : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransformList Path;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool InversePath = false;
    protected EngineBehavior behavior;
    protected NpcState npcState;
    protected NavMeshAgent agent;
    protected bool inited;
    private bool canInvert = false;

    public override void OnStart()
    {
      if (!inited)
      {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain NavMeshAgent unity component", gameObject);
          return;
        }
        npcState = gameObject.GetComponent<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
          return;
        }
        inited = true;
      }
      List<Vector3> path = new List<Vector3>();
      foreach (Transform transform in Path.Value)
        path.Add(transform.position);
      npcState.MoveByPathCloud(path);
    }

    public override TaskStatus OnUpdate()
    {
      if (!inited || npcState.Status == NpcStateStatusEnum.Failed)
        return TaskStatus.Failure;
      if (npcState.Status != NpcStateStatusEnum.Success)
        return TaskStatus.Running;
      if (canInvert)
        InversePath.Value = !InversePath.Value;
      return TaskStatus.Success;
    }

    public override void OnDrawGizmos() => NavMeshUtility.DrawPath(agent);

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Path", Path);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InversePath", InversePath);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Path = BehaviorTreeDataReadUtility.ReadShared(reader, "Path", Path);
      InversePath = BehaviorTreeDataReadUtility.ReadShared(reader, "InversePath", InversePath);
    }
  }
}
