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
  [TaskDescription("Follow")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Follow))]
  public class Follow : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Follow distance")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public float FollowDistance = 2f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Target;
    private NpcState npcState;

    public override void OnStart()
    {
      if (npcState == null)
      {
        npcState = gameObject.GetComponent<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
          return;
        }
      }
      if (Target.Value == null)
        return;
      npcState.MoveFollow(Target.Value, FollowDistance);
    }

    public override TaskStatus OnUpdate()
    {
      if (npcState == null || Target.Value == null || npcState.CurrentNpcState != NpcStateEnum.MoveFollow)
        return TaskStatus.Failure;
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
      DefaultDataWriteUtility.Write(writer, "FollowDistance", FollowDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      FollowDistance = DefaultDataReadUtility.Read(reader, "FollowDistance", FollowDistance);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
    }
  }
}
