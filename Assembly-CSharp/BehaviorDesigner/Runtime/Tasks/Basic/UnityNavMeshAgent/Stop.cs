using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNavMeshAgent
{
  [FactoryProxy(typeof (Stop))]
  [TaskCategory("Basic/NavMeshAgent")]
  [TaskDescription("Stop movement of this agent along its current path. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Stop : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedGameObject targetGameObject;
    private NavMeshAgent navMeshAgent;
    private GameObject prevGameObject;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedGameObject>(writer, "TargetGameObject", this.targetGameObject);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.targetGameObject = BehaviorTreeDataReadUtility.ReadShared<SharedGameObject>(reader, "TargetGameObject", this.targetGameObject);
    }

    public override void OnStart()
    {
      GameObject defaultGameObject = this.GetDefaultGameObject(this.targetGameObject.Value);
      if (!((UnityEngine.Object) defaultGameObject != (UnityEngine.Object) this.prevGameObject))
        return;
      this.navMeshAgent = defaultGameObject.GetComponent<NavMeshAgent>();
      this.prevGameObject = defaultGameObject;
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.navMeshAgent == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "NavMeshAgent is null");
        return TaskStatus.Failure;
      }
      this.navMeshAgent.isStopped = true;
      return TaskStatus.Success;
    }

    public override void OnReset() => this.targetGameObject = (SharedGameObject) null;
  }
}
