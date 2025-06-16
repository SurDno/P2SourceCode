using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Temp/Movement")]
  [TaskDescription("Убегать от противника")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightEscape))]
  public class FightEscape : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("На этом расстояние убегание завершится и вернет успех")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat EscapeDistance = 40f;
    [Tooltip("Время убегания (0 - бесконечно)")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat EscapeTime = 0.0f;
    private NpcState npcState;

    public override void OnStart()
    {
      base.OnStart();
      if (npcState == null)
      {
        npcState = gameObject.GetComponent<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
          return;
        }
      }
      npcState.FightEscape(EscapeDistance.Value);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if (EscapeTime.Value > 0.0 && startTime + (double) waitDuration < Time.time)
        return TaskStatus.Success;
      if (npcState.CurrentNpcState != NpcStateEnum.FightEscape)
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

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "EscapeDistance", EscapeDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "EscapeTime", EscapeTime);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      EscapeDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "EscapeDistance", EscapeDistance);
      EscapeTime = BehaviorTreeDataReadUtility.ReadShared(reader, "EscapeTime", EscapeTime);
    }
  }
}
