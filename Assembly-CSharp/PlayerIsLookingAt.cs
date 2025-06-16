using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

[TaskDescription("Player is looking at")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (PlayerIsLookingAt))]
public class PlayerIsLookingAt : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public bool UseDetector = true;
  private DetectableComponent detectable;
  private float visionRange = 30f;
  private float eyeAngle = 135f;

  public override void OnAwake()
  {
    IEntity entity = EntityUtility.GetEntity(gameObject);
    if (entity == null)
      return;
    detectable = entity.GetComponent<DetectableComponent>();
  }

  public override TaskStatus OnUpdate()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return TaskStatus.Failure;
    if (UseDetector)
    {
      if (detectable == null)
        return TaskStatus.Failure;
      DetectorComponent component = player.GetComponent<DetectorComponent>();
      return component == null || !component.Visible.Contains(detectable) ? TaskStatus.Failure : TaskStatus.Success;
    }
    if ((UnityEngine.Object) ((IEntityView) player).GameObject == (UnityEngine.Object) null)
      return TaskStatus.Failure;
    GameObject gameObject1 = ((IEntityView) player).GameObject;
    Pivot component1 = gameObject1.GetComponent<Pivot>();
    Pivot component2 = gameObject.GetComponent<Pivot>();
    if ((UnityEngine.Object) component1 == (UnityEngine.Object) null || (UnityEngine.Object) component2 == (UnityEngine.Object) null || (double) (gameObject1.transform.position - gameObject.transform.position).magnitude > visionRange)
      return TaskStatus.Failure;
    Vector3 forward = gameObject.transform.position - gameObject1.transform.position;
    Quaternion rotation = gameObject1.transform.rotation;
    float magnitude = forward.magnitude;
    Quaternion quaternion = Quaternion.identity;
    if (!Mathf.Approximately(magnitude, 0.0f))
      quaternion = rotation * Quaternion.Inverse(Quaternion.LookRotation(forward));
    if ((double) Mathf.Abs(Mathf.DeltaAngle(quaternion.eulerAngles.y, 0.0f)) >= eyeAngle * 0.5)
      return TaskStatus.Failure;
    Vector3 position1 = component1.Head.transform.position;
    Vector3 position2 = gameObject.transform.position;
    if ((UnityEngine.Object) component2 == (UnityEngine.Object) null)
      Debug.LogError((object) (gameObject.ToString() + " has no pivot!"));
    else if ((UnityEngine.Object) component2.Head == (UnityEngine.Object) null)
      Debug.LogError((object) (gameObject.ToString() + "pivot has no head!"));
    else
      position2 = component2.Head.transform.position;
    LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
    LayerMask ragdollLayer = ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer;
    List<RaycastHit> result = new List<RaycastHit>();
    PhysicsUtility.Raycast(result, position2, position1 - position2, visionRange, -1 ^ (int) triggerInteractLayer ^ (int) ragdollLayer, QueryTriggerInteraction.Ignore);
    for (int index = 0; index < result.Count; ++index)
    {
      GameObject gameObject2 = result[index].collider.gameObject;
      if (!((UnityEngine.Object) gameObject2 == (UnityEngine.Object) gameObject))
        return (UnityEngine.Object) gameObject2 == (UnityEngine.Object) gameObject1 ? TaskStatus.Success : TaskStatus.Failure;
    }
    return TaskStatus.Failure;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    DefaultDataWriteUtility.Write(writer, "UseDetector", UseDetector);
  }

  public void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    UseDetector = DefaultDataReadUtility.Read(reader, "UseDetector", UseDetector);
  }
}
