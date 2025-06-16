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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat EscapeDistance = (SharedFloat) 40f;
    [Tooltip("Время убегания (0 - бесконечно)")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat EscapeTime = (SharedFloat) 0.0f;
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
      this.npcState.FightEscape(this.EscapeDistance.Value);
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((double) this.EscapeTime.Value > 0.0 && (double) this.startTime + (double) this.waitDuration < (double) Time.time)
        return TaskStatus.Success;
      if (this.npcState.CurrentNpcState != NpcStateEnum.FightEscape)
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
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "EscapeDistance", this.EscapeDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "EscapeTime", this.EscapeTime);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.EscapeDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "EscapeDistance", this.EscapeDistance);
      this.EscapeTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "EscapeTime", this.EscapeTime);
    }
  }
}
