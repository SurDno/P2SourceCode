using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Преследовать противника держа дистанцию")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightSanitarFollow))]
  public class MeleeFightSanitarFollow : 
    MeleeFightBase,
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
    public SharedBool Aim;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat followTime = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private SanitarFollowDescription description;
    private float desiredWalkSpeed;
    private Vector3 lastPlayerPosition;

    public override void OnStart()
    {
      base.OnStart();
      waitDuration = followTime.Value;
      startTime = Time.time;
      agent.enabled = true;
      npcState.FightIdle(Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      if (followTime.Value > 0.0 && startTime + (double) waitDuration < Time.time)
        return TaskStatus.Success;
      if (description == null)
      {
        Debug.LogWarning(typeof (MeleeFightSanitarFollow).Name + " has no " + typeof (SanitarFollowDescription).Name + " attached", gameObject);
        return TaskStatus.Failure;
      }
      owner.RotationTarget = null;
      owner.RotateByPath = false;
      owner.RetreatAngle = new float?();
      if (Target.Value == null)
        return TaskStatus.Success;
      UpdatePath();
      Vector3 vector3 = Target.Value.position - owner.transform.position;
      float magnitude = vector3.magnitude;
      vector3.Normalize();
      if (NavMesh.Raycast(owner.transform.position, Target.Value.position, out NavMeshHit _, -1))
      {
        if (!agent.hasPath)
          return TaskStatus.Running;
        if (agent.remainingDistance > (double) description.RunDistance)
        {
          owner.RotationTarget = Target.Value;
          owner.RotateByPath = true;
          owner.RetreatAngle = new float?();
        }
      }
      desiredWalkSpeed = agent.remainingDistance <= (double) description.KeepDistance ? (agent.remainingDistance <= (double) description.RetreatDistance ? -1f : 0.0f) : (agent.remainingDistance > (double) description.RunDistance ? 2f : 1f);
      owner.DesiredWalkSpeed = desiredWalkSpeed;
      owner.RotationTarget = Target.Value;
      agent.nextPosition = animator.rootPosition;
      return TaskStatus.Running;
    }

    private void UpdatePath()
    {
      if ((lastPlayerPosition - Target.Value.position).magnitude <= (double) description.RetreatDistance)
        return;
      if (!agent.isOnNavMesh)
        agent.Warp(transform.position);
      if (agent.isOnNavMesh)
        agent.destination = Target.Value.position;
      lastPlayerPosition = Target.Value.position;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Aim", Aim);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowTime", followTime);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Description", description);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      Aim = BehaviorTreeDataReadUtility.ReadShared(reader, "Aim", Aim);
      followTime = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowTime", followTime);
      description = BehaviorTreeDataReadUtility.ReadUnity(reader, "Description", description);
    }
  }
}
