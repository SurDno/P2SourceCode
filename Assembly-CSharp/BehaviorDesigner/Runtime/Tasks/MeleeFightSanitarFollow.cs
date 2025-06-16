// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.MeleeFightSanitarFollow
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
using UnityEngine.AI;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Преследовать противника держа дистанцию")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightSanitarFollow))]
  public class MeleeFightSanitarFollow : 
    MeleeFightBase,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Aim;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat followTime = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SanitarFollowDescription description;
    private float desiredWalkSpeed;
    private Vector3 lastPlayerPosition;

    public override void OnStart()
    {
      base.OnStart();
      this.waitDuration = this.followTime.Value;
      this.startTime = Time.time;
      this.agent.enabled = true;
      this.npcState.FightIdle(this.Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      if ((double) this.followTime.Value > 0.0 && (double) this.startTime + (double) this.waitDuration < (double) Time.time)
        return TaskStatus.Success;
      if ((UnityEngine.Object) this.description == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (typeof (MeleeFightSanitarFollow).Name + " has no " + typeof (SanitarFollowDescription).Name + " attached"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      this.owner.RotationTarget = (Transform) null;
      this.owner.RotateByPath = false;
      this.owner.RetreatAngle = new float?();
      if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
        return TaskStatus.Success;
      this.UpdatePath();
      Vector3 vector3 = this.Target.Value.position - this.owner.transform.position;
      float magnitude = vector3.magnitude;
      vector3.Normalize();
      if (NavMesh.Raycast(this.owner.transform.position, this.Target.Value.position, out NavMeshHit _, -1))
      {
        if (!this.agent.hasPath)
          return TaskStatus.Running;
        if ((double) this.agent.remainingDistance > (double) this.description.RunDistance)
        {
          this.owner.RotationTarget = this.Target.Value;
          this.owner.RotateByPath = true;
          this.owner.RetreatAngle = new float?();
        }
      }
      this.desiredWalkSpeed = (double) this.agent.remainingDistance <= (double) this.description.KeepDistance ? ((double) this.agent.remainingDistance <= (double) this.description.RetreatDistance ? -1f : 0.0f) : ((double) this.agent.remainingDistance > (double) this.description.RunDistance ? 2f : 1f);
      this.owner.DesiredWalkSpeed = this.desiredWalkSpeed;
      this.owner.RotationTarget = this.Target.Value;
      this.agent.nextPosition = this.animator.rootPosition;
      return TaskStatus.Running;
    }

    private void UpdatePath()
    {
      if ((double) (this.lastPlayerPosition - this.Target.Value.position).magnitude <= (double) this.description.RetreatDistance)
        return;
      if (!this.agent.isOnNavMesh)
        this.agent.Warp(this.transform.position);
      if (this.agent.isOnNavMesh)
        this.agent.destination = this.Target.Value.position;
      this.lastPlayerPosition = this.Target.Value.position;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Aim", this.Aim);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "FollowTime", this.followTime);
      BehaviorTreeDataWriteUtility.WriteUnity<SanitarFollowDescription>(writer, "Description", this.description);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.Aim = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Aim", this.Aim);
      this.followTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "FollowTime", this.followTime);
      this.description = BehaviorTreeDataReadUtility.ReadUnity<SanitarFollowDescription>(reader, "Description", this.description);
    }
  }
}
