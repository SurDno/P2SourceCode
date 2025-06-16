using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNavMeshAgent
{
  [FactoryProxy(typeof (SetSpeed))]
  [TaskCategory("Basic/NavMeshAgent")]
  [TaskDescription("Sets the maximum movement speed when following a path. Returns Success.")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SetSpeed : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedGameObject targetGameObject;
    [Tooltip("The NavMeshAgent speed")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat speed;
    private NavMeshAgent navMeshAgent;
    private GameObject prevGameObject;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetGameObject", targetGameObject);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Speed", speed);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      targetGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetGameObject", targetGameObject);
      speed = BehaviorTreeDataReadUtility.ReadShared(reader, "Speed", speed);
    }

    public override void OnStart()
    {
      GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
      if (!((UnityEngine.Object) defaultGameObject != (UnityEngine.Object) prevGameObject))
        return;
      navMeshAgent = defaultGameObject.GetComponent<NavMeshAgent>();
      prevGameObject = defaultGameObject;
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) navMeshAgent == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "NavMeshAgent is null");
        return TaskStatus.Failure;
      }
      navMeshAgent.speed = speed.Value;
      return TaskStatus.Success;
    }

    public override void OnReset()
    {
      targetGameObject = null;
      speed = 0.0f;
    }
  }
}
