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
  [TaskDescription("Retreat from")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Retreat))]
  public class Retreat : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Retreat distance (task will be finished as soon as it will be reached)")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat RetreatDistance = 25f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Target;
    private bool inited;
    private NpcState npcState;

    public override void OnStart()
    {
      if (!inited)
      {
        npcState = gameObject.GetComponent<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
          return;
        }
        inited = true;
      }
      if (Target.Value == null)
        Debug.LogWarningFormat("{0}: has null Target", gameObject.name);
      else
        npcState.MoveRetreat(Target.Value, RetreatDistance.Value);
    }

    public override TaskStatus OnUpdate()
    {
      if (!inited || Target.Value == null || npcState.CurrentNpcState != NpcStateEnum.MoveRetreat)
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
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RetreatDistance", RetreatDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      RetreatDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "RetreatDistance", RetreatDistance);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
    }
  }
}
