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
    public LayerMask physicsLayer = (LayerMask) (LayerMaskUtility.GetMask((LayerMask) 0) | LayerMaskUtility.GetMask((LayerMask) 9) | LayerMaskUtility.GetMask((LayerMask) 14));
    private Pivot pivot;

    public override void OnAwake()
    {
      pivot = gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} doesnt' contain {1} unity component", (object) gameObject.name, (object) typeof (Pivot).Name);
      }
      else
      {
        if (!((UnityEngine.Object) pivot.Head == (UnityEngine.Object) null))
          return;
        Debug.LogWarningFormat("{0} Pivot doesnt' contain Head GameObject", (object) gameObject.name);
      }
    }

    public override void OnStart()
    {
      if (!((UnityEngine.Object) Target.Value == (UnityEngine.Object) null))
        return;
      Debug.LogWarningFormat("{0} Target is null", (object) gameObject.name);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null || (UnityEngine.Object) Target.Value == (UnityEngine.Object) null || (double) (Target.Value.transform.position - gameObject.transform.position).magnitude > RaycastDistance)
        return TaskStatus.Failure;
      Vector3 origin = !((UnityEngine.Object) pivot.Head != (UnityEngine.Object) null) ? gameObject.transform.position : pivot.Head.transform.position;
      Pivot component = gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} doesnt' contain Pivot unity component");
        return TaskStatus.Failure;
      }
      Vector3 vector3 = !((UnityEngine.Object) component.Head != (UnityEngine.Object) null) ? gameObject.transform.position : component.Head.transform.position;
      return Physics.Raycast(origin, vector3 - origin, RaycastDistance, (int) physicsLayer, QueryTriggerInteraction.Ignore) ? TaskStatus.Failure : TaskStatus.Success;
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
