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
  [TaskDescription("Преследовать противника")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightFollow))]
  public class FightFollow : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat followTime = (SharedFloat) 0.0f;
    [Header("Передвижение")]
    [Tooltip("Если игрок удалился на эту дистанцию, но NPC переходит на бег.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat RunDistance = (SharedFloat) 5f;
    [Tooltip("Если игрок удалился на эту дистанцию, но NPC останавливается.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat StopDistance = (SharedFloat) 1.2f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Aim;
    private NpcState npcState;

    public override void OnStart()
    {
      base.OnStart();
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
      {
        this.npcState = this.gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return;
        }
      }
      this.npcState.FightFollow(this.StopDistance.Value, this.RunDistance.Value, this.Aim.Value);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((double) this.followTime.Value > 0.0 && (double) this.startTime + (double) this.waitDuration < (double) Time.time)
        return TaskStatus.Success;
      if (this.npcState.CurrentNpcState != NpcStateEnum.FightFollow)
        return TaskStatus.Failure;
      switch (this.npcState.Status)
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
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "FollowTime", this.followTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RunDistance", this.RunDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StopDistance", this.StopDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Aim", this.Aim);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.followTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "FollowTime", this.followTime);
      this.RunDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RunDistance", this.RunDistance);
      this.StopDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StopDistance", this.StopDistance);
      this.Aim = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Aim", this.Aim);
    }
  }
}
