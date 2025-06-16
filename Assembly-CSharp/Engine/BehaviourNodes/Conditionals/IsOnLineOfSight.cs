using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Can raycast to NPC?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsOnLineOfSight))]
  public class IsOnLineOfSight : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public float RaycastDistance = 30f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public LayerMask physicsLayer = LayerMaskUtility.GetMask(0) | LayerMaskUtility.GetMask(9) | LayerMaskUtility.GetMask(14);
    private Pivot pivot;

    public override void OnAwake()
    {
      pivot = gameObject.GetComponent<Pivot>();
      if (pivot == null)
      {
        Debug.LogErrorFormat("{0} doesnt' contain {1} unity component", gameObject.name, typeof (Pivot).Name);
      }
      else
      {
        if (!(pivot.Head == null))
          return;
        Debug.LogWarningFormat("{0} Pivot doesnt' contain Head GameObject", gameObject.name);
      }
    }

    public override void OnStart()
    {
      if (!(Target.Value == null))
        return;
      Debug.LogWarningFormat("{0} Target is null", gameObject.name);
    }

    public override TaskStatus OnUpdate()
    {
      if (pivot == null || Target.Value == null || (Target.Value.transform.position - gameObject.transform.position).magnitude > (double) RaycastDistance)
        return TaskStatus.Failure;
      Vector3 origin = !(pivot.Head != null) ? gameObject.transform.position : pivot.Head.transform.position;
      Pivot component = gameObject.GetComponent<Pivot>();
      if (component == null)
      {
        Debug.LogErrorFormat("{0} doesnt' contain Pivot unity component");
        return TaskStatus.Failure;
      }
      Vector3 vector3 = !(component.Head != null) ? gameObject.transform.position : component.Head.transform.position;
      return Physics.Raycast(origin, vector3 - origin, RaycastDistance, physicsLayer, QueryTriggerInteraction.Ignore) ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      DefaultDataWriteUtility.Write(writer, "RaycastDistance", RaycastDistance);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "PhysicsLayer", physicsLayer);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      RaycastDistance = DefaultDataReadUtility.Read(reader, "RaycastDistance", RaycastDistance);
      physicsLayer = BehaviorTreeDataReadUtility.ReadUnity(reader, "PhysicsLayer", physicsLayer);
    }
  }
}
