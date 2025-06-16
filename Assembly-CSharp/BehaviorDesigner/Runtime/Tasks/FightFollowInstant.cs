using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Temp/Movement")]
  [TaskDescription("Преследовать противника")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightFollowInstant))]
  public class FightFollowInstant : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Header("Передвижение")]
    [Tooltip("Если игрок удалился на эту дистанцию, но NPC переходит на бег.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat RunDistance = 5f;
    [Tooltip("Если игрок удалился на эту дистанцию, но NPC останавливается.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat StopDistance = 1.2f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool Aim;
    private NpcState npcState;

    public override void OnStart()
    {
      if ((UnityEngine.Object) npcState == (UnityEngine.Object) null)
      {
        npcState = gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) gameObject);
          return;
        }
      }
      npcState.FightFollow(StopDistance.Value, RunDistance.Value, Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) npcState == (UnityEngine.Object) null ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RunDistance", RunDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StopDistance", StopDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Aim", Aim);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      RunDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "RunDistance", RunDistance);
      StopDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "StopDistance", StopDistance);
      Aim = BehaviorTreeDataReadUtility.ReadShared(reader, "Aim", Aim);
    }
  }
}
