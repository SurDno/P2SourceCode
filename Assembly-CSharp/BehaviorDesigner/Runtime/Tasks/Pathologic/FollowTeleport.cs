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
  [FactoryProxy(typeof (FollowTeleport))]
  public class FollowTeleport : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Follow distance")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat FollowDistance = (SharedFloat) 2f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Через это время будет предпринята очередная попытка телепортации, если почтальон не дошел до цели.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat TeleportRepeatTime = (SharedFloat) 30f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Нужно ли ждать пока игрок посмотрит на посыльного?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool WaitForPlayerFacesYou = (SharedBool) true;
    private NpcState npcState;

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
      {
        this.npcState = this.gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return;
        }
      }
      this.npcState.MoveFollowTeleport(this.FollowDistance.Value, this.TeleportRepeatTime.Value, this.WaitForPlayerFacesYou.Value);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null || this.npcState.CurrentNpcState != NpcStateEnum.MoveFollowTeleport)
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

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "FollowDistance", this.FollowDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "TeleportRepeatTime", this.TeleportRepeatTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "WaitForPlayerFacesYou", this.WaitForPlayerFacesYou);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.FollowDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "FollowDistance", this.FollowDistance);
      this.TeleportRepeatTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "TeleportRepeatTime", this.TeleportRepeatTime);
      this.WaitForPlayerFacesYou = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "WaitForPlayerFacesYou", this.WaitForPlayerFacesYou);
    }
  }
}
