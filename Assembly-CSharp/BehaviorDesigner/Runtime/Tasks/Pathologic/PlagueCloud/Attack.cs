// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud.Attack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud
{
  [TaskDescription("Attack")]
  [TaskCategory("Pathologic/PlagueCloud")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Attack))]
  public class Attack : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Meters/sec")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat MovementSpeed = (SharedFloat) 5f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Stop distance")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat StopDistance = (SharedFloat) 0.0f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Rocket Mode")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool RocketMode = (SharedBool) false;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Rocket overshoot")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat RocketOvershoot = (SharedFloat) 2f;
    private Vector3 targetPos;
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
      if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0} : null target", (object) this.gameObject.name);
      }
      else
      {
        this.targetPos = this.Target.Value.transform.position;
        if (this.RocketMode.Value)
          this.targetPos += (this.targetPos - this.transform.position).normalized * this.RocketOvershoot.Value;
        if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null || (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
          return;
        this.npcState.MoveFollow(this.Target.Value, 0.0f);
      }
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null || (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null || this.npcState.CurrentNpcState != NpcStateEnum.MoveFollow)
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
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "MovementSpeed", this.MovementSpeed);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StopDistance", this.StopDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RocketMode", this.RocketMode);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RocketOvershoot", this.RocketOvershoot);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.MovementSpeed = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "MovementSpeed", this.MovementSpeed);
      this.StopDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StopDistance", this.StopDistance);
      this.RocketMode = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RocketMode", this.RocketMode);
      this.RocketOvershoot = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RocketOvershoot", this.RocketOvershoot);
    }
  }
}
