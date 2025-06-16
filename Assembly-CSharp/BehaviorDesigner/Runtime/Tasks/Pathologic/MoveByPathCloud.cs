using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components.Utilities;
using Scripts.Tools.Serializations.Converters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Move To by path cloud")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MoveByPathCloud))]
  public class MoveByPathCloud : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransformList Path;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool InversePath = (SharedBool) false;
    protected EngineBehavior behavior;
    protected NpcState npcState;
    protected NavMeshAgent agent;
    protected bool inited = false;
    private bool canInvert = false;

    public override void OnStart()
    {
      if (!this.inited)
      {
        this.agent = this.gameObject.GetComponent<NavMeshAgent>();
        if ((UnityEngine.Object) this.agent == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain NavMeshAgent unity component"), (UnityEngine.Object) this.gameObject);
          return;
        }
        this.npcState = this.gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return;
        }
        this.inited = true;
      }
      List<Vector3> path = new List<Vector3>();
      foreach (Transform transform in this.Path.Value)
        path.Add(transform.position);
      this.npcState.MoveByPathCloud(path);
    }

    public override TaskStatus OnUpdate()
    {
      if (!this.inited || this.npcState.Status == NpcStateStatusEnum.Failed)
        return TaskStatus.Failure;
      if (this.npcState.Status != NpcStateStatusEnum.Success)
        return TaskStatus.Running;
      if (this.canInvert)
        this.InversePath.Value = !this.InversePath.Value;
      return TaskStatus.Success;
    }

    public override void OnDrawGizmos() => NavMeshUtility.DrawPath(this.agent);

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransformList>(writer, "Path", this.Path);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "InversePath", this.InversePath);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Path = BehaviorTreeDataReadUtility.ReadShared<SharedTransformList>(reader, "Path", this.Path);
      this.InversePath = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "InversePath", this.InversePath);
    }
  }
}
