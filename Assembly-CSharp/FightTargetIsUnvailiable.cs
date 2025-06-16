using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

[TaskDescription("Fight target is unvailiable")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (FightTargetIsUnvailiable))]
public class FightTargetIsUnvailiable : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public SharedTransform Target;
  protected DetectorComponent detector;
  protected IEntity entity;

  public override void OnAwake()
  {
    entity = EntityUtility.GetEntity(gameObject);
    if (entity == null)
    {
      Debug.LogWarning((object) (gameObject.name + " : entity not found, method : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) gameObject);
    }
    else
    {
      detector = entity.GetComponent<DetectorComponent>();
      if (detector != null)
        return;
      Debug.LogWarningFormat("{0}: doesn't contain {1} engine component", (object) gameObject.name, (object) typeof (IDetectorComponent).Name);
    }
  }

  public override TaskStatus OnUpdate()
  {
    if ((UnityEngine.Object) Target.Value == (UnityEngine.Object) null)
      return TaskStatus.Success;
    if (entity == null || detector == null)
      return TaskStatus.Failure;
    if ((UnityEngine.Object) Target.Value == (UnityEngine.Object) gameObject.transform || !Target.Value.gameObject.activeSelf)
      return TaskStatus.Success;
    if (gameObject.GetComponent<EnemyBase>().IsFaint)
      return TaskStatus.Failure;
    List<IEntity> entityList = new List<IEntity>();
    foreach (IDetectableComponent detectable in detector.Visible)
    {
      if (DetectableIsTarget(detectable))
        return TaskStatus.Failure;
    }
    foreach (IDetectableComponent detectable in detector.Hearing)
    {
      if (DetectableIsTarget(detectable))
        return TaskStatus.Failure;
    }
    return TaskStatus.Success;
  }

  private bool DetectableIsTarget(IDetectableComponent detectable)
  {
    if (detectable == null || detectable.IsDisposed)
      return false;
    IEntity owner = detectable.Owner;
    if (owner == null)
      return false;
    GameObject gameObject = ((IEntityView) owner).GameObject;
    return !((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && (UnityEngine.Object) gameObject.transform == (UnityEngine.Object) Target.Value;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
  }

  public void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
  }
}
