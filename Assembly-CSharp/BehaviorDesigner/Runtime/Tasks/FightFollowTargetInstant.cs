// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.FightFollowTargetInstant
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
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Temp/Movement")]
  [TaskDescription("идти к предмету")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightFollowTargetInstant))]
  public class FightFollowTargetInstant : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Если объект удалился на эту дистанцию, но NPC переходит на бег.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [Header("Передвижение")]
    [Tooltip("Если объект удалился на эту дистанцию, но NPC переходит на бег.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat RunDistance = (SharedFloat) 7f;
    [Tooltip("Если объект удалился на эту дистанцию, но NPC останавливается.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat StopDistance = (SharedFloat) 3f;
    [Tooltip("Если объект удалился на эту дистанцию, но NPC останавливается.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat RetreatDistance = (SharedFloat) 2f;
    [Tooltip("Надо ли прицеливаться из оружия.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Aim = (SharedBool) false;
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
      if (this.Target == null || (UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
        return;
      this.npcState.FightFollowTarget(this.StopDistance.Value, this.RunDistance.Value, this.RetreatDistance.Value, this.Target.Value, this.Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) this.npcState == (UnityEngine.Object) null ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RunDistance", this.RunDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "StopDistance", this.StopDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RetreatDistance", this.RetreatDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Aim", this.Aim);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.RunDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RunDistance", this.RunDistance);
      this.StopDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "StopDistance", this.StopDistance);
      this.RetreatDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RetreatDistance", this.RetreatDistance);
      this.Aim = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Aim", this.Aim);
    }
  }
}
