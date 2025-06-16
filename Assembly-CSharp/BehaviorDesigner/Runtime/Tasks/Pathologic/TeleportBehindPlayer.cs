using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Settings;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Find Point Behind Player")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (TeleportBehindPlayer))]
  public class TeleportBehindPlayer : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat MinSearchDistance = 3f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat MaxSearchDistance = 18f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool UsePlayerVisionAngle = true;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedInt NumberOfTries = 5;
    private IEntity entity;
    private IEntity player;
    private NpcState npcState;
    private float lastTryTime;
    private float timeBetweenTries = 0.3f;
    private int triesMade;
    private bool wasRestartBehaviourAfterTeleport;

    public override void OnStart()
    {
      entity = EntityUtility.GetEntity(gameObject);
      player = ServiceLocator.GetService<ISimulation>().Player;
      triesMade = 0;
      lastTryTime = Time.time;
      npcState = gameObject.GetComponent<NpcState>();
      if (!(bool) (Object) npcState)
        return;
      wasRestartBehaviourAfterTeleport = npcState.RestartBehaviourAfterTeleport;
      npcState.RestartBehaviourAfterTeleport = false;
    }

    public override void OnEnd()
    {
      if ((bool) (Object) npcState)
        npcState.RestartBehaviourAfterTeleport = wasRestartBehaviourAfterTeleport;
      triesMade = 0;
    }

    public override TaskStatus OnUpdate()
    {
      if (entity == null || player == null || ((IEntityView) player).GameObject == null)
        return TaskStatus.Failure;
      if (Time.time >= lastTryTime + (double) timeBetweenTries)
      {
        Vector3 vector3 = CountRandomPoint(((IEntityView) player).GameObject);
        if (TryPoint(vector3, ((IEntityView) player).GameObject.transform.position))
        {
          ILocationComponent location = player.GetComponent<LocationItemComponent>()?.Location;
          entity.GetComponent<NavigationComponent>().TeleportTo(location, vector3, Quaternion.LookRotation(((IEntityView) player).GameObject.transform.position - vector3));
          return TaskStatus.Success;
        }
        ++triesMade;
        lastTryTime = Time.time;
        if (triesMade > NumberOfTries.Value)
          return TaskStatus.Failure;
      }
      return TaskStatus.Running;
    }

    private Vector3 CountRandomPoint(GameObject player)
    {
      float num1 = UsePlayerVisionAngle.Value ? InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value : 0.0f;
      float num2 = Random.Range(num1 - 180f, 180f - num1);
      float y = player.transform.rotation.eulerAngles.y + num2;
      float num3 = Random.Range(MinSearchDistance.Value, MaxSearchDistance.Value);
      return player.transform.position + Quaternion.Euler(0.0f, y, 0.0f) * Vector3.back * num3;
    }

    private bool TryPoint(Vector3 point, Vector3 playerPosition)
    {
      NavMeshHit hit;
      if (!NavMesh.SamplePosition(point, out hit, 1f, -1))
        return false;
      point = hit.position;
      NavMeshPath path = new NavMeshPath();
      NavMesh.CalculatePath(point, playerPosition, -1, path);
      return path.status == NavMeshPathStatus.PathComplete && NavMeshUtility.GetPathLength(path) <= MaxSearchDistance.Value * 2.0;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "MinSearchDistance", MinSearchDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "MaxSearchDistance", MaxSearchDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "UsePlayerVisionAngle", UsePlayerVisionAngle);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "NumberOfTries", NumberOfTries);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      MinSearchDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "MinSearchDistance", MinSearchDistance);
      MaxSearchDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "MaxSearchDistance", MaxSearchDistance);
      UsePlayerVisionAngle = BehaviorTreeDataReadUtility.ReadShared(reader, "UsePlayerVisionAngle", UsePlayerVisionAngle);
      NumberOfTries = BehaviorTreeDataReadUtility.ReadShared(reader, "NumberOfTries", NumberOfTries);
    }
  }
}
