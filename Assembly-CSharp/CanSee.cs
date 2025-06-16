using System;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

[TaskDescription("Can see NPC (with Behaviour and Info engine components)")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (CanSee))]
public class CanSee : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy]
  [SerializeField]
  public SharedTransform Result;
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy]
  [SerializeField]
  public SharedTransformList ResultList;
  [UnityEngine.Tooltip("Use None for any detector type")]
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public DetectType DetectType = DetectType.None;
  protected DetectorComponent detector;
  protected IEntity entity;

  protected virtual bool Filter(DetectableComponent detectable)
  {
    return DetectType == DetectType.None || DetectType == detectable.VisibleDetectType;
  }

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
    if (entity == null || detector == null)
      return TaskStatus.Failure;
    if (ResultList.Value != null)
      ResultList.Value.Clear();
    foreach (IDetectableComponent detectableComponent in detector.Visible)
    {
      if (detectableComponent != null && !detectableComponent.IsDisposed)
      {
        DetectableComponent detectable = (DetectableComponent) detectableComponent;
        if (Filter(detectable))
        {
          GameObject gameObject = ((IEntityView) detectable.Owner).GameObject;
          if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
          {
            if (ResultList.Value == null)
            {
              Result.Value = gameObject.transform;
              return TaskStatus.Success;
            }
            ResultList.Value.Add(gameObject.transform);
          }
        }
      }
    }
    if (ResultList.Value == null || ResultList.Value.Count == 0)
      return TaskStatus.Failure;
    Result.Value = ResultList.Value[0];
    return TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "ResultList", ResultList);
    DefaultDataWriteUtility.WriteEnum(writer, "DetectType", DetectType);
  }

  public void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
    ResultList = BehaviorTreeDataReadUtility.ReadShared(reader, "ResultList", ResultList);
    DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
  }
}
