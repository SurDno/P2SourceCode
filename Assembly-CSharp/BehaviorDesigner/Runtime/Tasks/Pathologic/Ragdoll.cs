using System;
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
  [TaskDescription("Enable ragdoll.")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Ragdoll))]
  public class Ragdoll : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat WaitTime = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool InternalCollisions = true;
    private NpcState npcState;
    private float startTime;
    private float pauseTime;

    public override void OnAwake()
    {
      npcState = gameObject.GetComponent<NpcState>();
      if (!(npcState == null))
        return;
      Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
    }

    public override void OnStart()
    {
      if (npcState == null)
        return;
      startTime = Time.time;
      npcState.Ragdoll(InternalCollisions.Value);
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        pauseTime = Time.time;
      else
        startTime += Time.time - pauseTime;
    }

    public override TaskStatus OnUpdate()
    {
      if (npcState == null || npcState.CurrentNpcState != NpcStateEnum.Ragdoll)
        return TaskStatus.Failure;
      if (WaitTime.Value > 0.0 && startTime + (double) WaitTime.Value < Time.time)
        return TaskStatus.Success;
      switch (npcState.Status)
      {
        case NpcStateStatusEnum.Success:
          return TaskStatus.Success;
        case NpcStateStatusEnum.Failed:
          return TaskStatus.Failure;
        default:
          return TaskStatus.Running;
      }
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "WaitTime", WaitTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InternalCollisions", InternalCollisions);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      WaitTime = BehaviorTreeDataReadUtility.ReadShared(reader, "WaitTime", WaitTime);
      InternalCollisions = BehaviorTreeDataReadUtility.ReadShared(reader, "InternalCollisions", InternalCollisions);
    }
  }
}
