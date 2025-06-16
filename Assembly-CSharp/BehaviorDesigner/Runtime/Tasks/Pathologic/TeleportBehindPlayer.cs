// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.TeleportBehindPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Settings;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Find Point Behind Player")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (TeleportBehindPlayer))]
  public class TeleportBehindPlayer : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat MinSearchDistance = (SharedFloat) 3f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat MaxSearchDistance = (SharedFloat) 18f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool UsePlayerVisionAngle = (SharedBool) true;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt NumberOfTries = (SharedInt) 5;
    private IEntity entity;
    private IEntity player;
    private NpcState npcState;
    private float lastTryTime;
    private float timeBetweenTries = 0.3f;
    private int triesMade = 0;
    private bool wasRestartBehaviourAfterTeleport;

    public override void OnStart()
    {
      this.entity = EntityUtility.GetEntity(this.gameObject);
      this.player = ServiceLocator.GetService<ISimulation>().Player;
      this.triesMade = 0;
      this.lastTryTime = Time.time;
      this.npcState = this.gameObject.GetComponent<NpcState>();
      if (!(bool) (UnityEngine.Object) this.npcState)
        return;
      this.wasRestartBehaviourAfterTeleport = this.npcState.RestartBehaviourAfterTeleport;
      this.npcState.RestartBehaviourAfterTeleport = false;
    }

    public override void OnEnd()
    {
      if ((bool) (UnityEngine.Object) this.npcState)
        this.npcState.RestartBehaviourAfterTeleport = this.wasRestartBehaviourAfterTeleport;
      this.triesMade = 0;
    }

    public override TaskStatus OnUpdate()
    {
      if (this.entity == null || this.player == null || (UnityEngine.Object) ((IEntityView) this.player).GameObject == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if ((double) Time.time >= (double) this.lastTryTime + (double) this.timeBetweenTries)
      {
        Vector3 vector3 = this.CountRandomPoint(((IEntityView) this.player).GameObject);
        if (this.TryPoint(vector3, ((IEntityView) this.player).GameObject.transform.position))
        {
          ILocationComponent location = this.player.GetComponent<LocationItemComponent>()?.Location;
          this.entity.GetComponent<NavigationComponent>().TeleportTo(location, vector3, Quaternion.LookRotation(((IEntityView) this.player).GameObject.transform.position - vector3));
          return TaskStatus.Success;
        }
        ++this.triesMade;
        this.lastTryTime = Time.time;
        if (this.triesMade > this.NumberOfTries.Value)
          return TaskStatus.Failure;
      }
      return TaskStatus.Running;
    }

    private Vector3 CountRandomPoint(GameObject player)
    {
      float num1 = this.UsePlayerVisionAngle.Value ? InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value : 0.0f;
      float num2 = UnityEngine.Random.Range(num1 - 180f, 180f - num1);
      float y = player.transform.rotation.eulerAngles.y + num2;
      float num3 = UnityEngine.Random.Range(this.MinSearchDistance.Value, this.MaxSearchDistance.Value);
      return player.transform.position + Quaternion.Euler(0.0f, y, 0.0f) * Vector3.back * num3;
    }

    private bool TryPoint(Vector3 point, Vector3 playerPosition)
    {
      NavMeshHit hit;
      if (!NavMesh.SamplePosition(point, out hit, 1f, -1))
        return false;
      point = hit.position;
      NavMeshPath path = new NavMeshPath();
      NavMesh.CalculatePath(point, playerPosition, -1, path);
      return path.status == NavMeshPathStatus.PathComplete && (double) NavMeshUtility.GetPathLength(path) <= (double) this.MaxSearchDistance.Value * 2.0;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "MinSearchDistance", this.MinSearchDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "MaxSearchDistance", this.MaxSearchDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "UsePlayerVisionAngle", this.UsePlayerVisionAngle);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "NumberOfTries", this.NumberOfTries);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.MinSearchDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "MinSearchDistance", this.MinSearchDistance);
      this.MaxSearchDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "MaxSearchDistance", this.MaxSearchDistance);
      this.UsePlayerVisionAngle = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "UsePlayerVisionAngle", this.UsePlayerVisionAngle);
      this.NumberOfTries = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "NumberOfTries", this.NumberOfTries);
    }
  }
}
