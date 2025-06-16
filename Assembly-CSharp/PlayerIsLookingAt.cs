using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using System.Collections.Generic;
using UnityEngine;

[TaskDescription("Player is looking at")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (PlayerIsLookingAt))]
public class PlayerIsLookingAt : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public bool UseDetector = true;
  private DetectableComponent detectable;
  private float visionRange = 30f;
  private float eyeAngle = 135f;

  public override void OnAwake()
  {
    IEntity entity = EntityUtility.GetEntity(this.gameObject);
    if (entity == null)
      return;
    this.detectable = entity.GetComponent<DetectableComponent>();
  }

  public override TaskStatus OnUpdate()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return TaskStatus.Failure;
    if (this.UseDetector)
    {
      if (this.detectable == null)
        return TaskStatus.Failure;
      DetectorComponent component = player.GetComponent<DetectorComponent>();
      return component == null || !component.Visible.Contains((IDetectableComponent) this.detectable) ? TaskStatus.Failure : TaskStatus.Success;
    }
    if ((UnityEngine.Object) ((IEntityView) player).GameObject == (UnityEngine.Object) null)
      return TaskStatus.Failure;
    GameObject gameObject1 = ((IEntityView) player).GameObject;
    Pivot component1 = gameObject1.GetComponent<Pivot>();
    Pivot component2 = this.gameObject.GetComponent<Pivot>();
    if ((UnityEngine.Object) component1 == (UnityEngine.Object) null || (UnityEngine.Object) component2 == (UnityEngine.Object) null || (double) (gameObject1.transform.position - this.gameObject.transform.position).magnitude > (double) this.visionRange)
      return TaskStatus.Failure;
    Vector3 forward = this.gameObject.transform.position - gameObject1.transform.position;
    Quaternion rotation = gameObject1.transform.rotation;
    float magnitude = forward.magnitude;
    Quaternion quaternion = Quaternion.identity;
    if (!Mathf.Approximately(magnitude, 0.0f))
      quaternion = rotation * Quaternion.Inverse(Quaternion.LookRotation(forward));
    if ((double) Mathf.Abs(Mathf.DeltaAngle(quaternion.eulerAngles.y, 0.0f)) >= (double) this.eyeAngle * 0.5)
      return TaskStatus.Failure;
    Vector3 position1 = component1.Head.transform.position;
    Vector3 position2 = this.gameObject.transform.position;
    if ((UnityEngine.Object) component2 == (UnityEngine.Object) null)
      Debug.LogError((object) (this.gameObject.ToString() + " has no pivot!"));
    else if ((UnityEngine.Object) component2.Head == (UnityEngine.Object) null)
      Debug.LogError((object) (this.gameObject.ToString() + "pivot has no head!"));
    else
      position2 = component2.Head.transform.position;
    LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
    LayerMask ragdollLayer = ScriptableObjectInstance<GameSettingsData>.Instance.RagdollLayer;
    List<RaycastHit> result = new List<RaycastHit>();
    PhysicsUtility.Raycast(result, position2, position1 - position2, this.visionRange, -1 ^ (int) triggerInteractLayer ^ (int) ragdollLayer, QueryTriggerInteraction.Ignore);
    for (int index = 0; index < result.Count; ++index)
    {
      GameObject gameObject2 = result[index].collider.gameObject;
      if (!((UnityEngine.Object) gameObject2 == (UnityEngine.Object) this.gameObject))
        return (UnityEngine.Object) gameObject2 == (UnityEngine.Object) gameObject1 ? TaskStatus.Success : TaskStatus.Failure;
    }
    return TaskStatus.Failure;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    DefaultDataWriteUtility.Write(writer, "UseDetector", this.UseDetector);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.UseDetector = DefaultDataReadUtility.Read(reader, "UseDetector", this.UseDetector);
  }
}
