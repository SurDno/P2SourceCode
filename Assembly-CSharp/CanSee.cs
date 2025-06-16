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
using System.Reflection;
using UnityEngine;

[TaskDescription("Can see NPC (with Behaviour and Info engine components)")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (CanSee))]
public class CanSee : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedTransform Result;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedTransformList ResultList;
  [UnityEngine.Tooltip("Use None for any detector type")]
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public DetectType DetectType = DetectType.None;
  protected DetectorComponent detector;
  protected IEntity entity;

  protected virtual bool Filter(DetectableComponent detectable)
  {
    return this.DetectType == DetectType.None || this.DetectType == detectable.VisibleDetectType;
  }

  public override void OnAwake()
  {
    this.entity = EntityUtility.GetEntity(this.gameObject);
    if (this.entity == null)
    {
      Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.detector = this.entity.GetComponent<DetectorComponent>();
      if (this.detector != null)
        return;
      Debug.LogWarningFormat("{0}: doesn't contain {1} engine component", (object) this.gameObject.name, (object) typeof (IDetectorComponent).Name);
    }
  }

  public override TaskStatus OnUpdate()
  {
    if (this.entity == null || this.detector == null)
      return TaskStatus.Failure;
    if (this.ResultList.Value != null)
      this.ResultList.Value.Clear();
    foreach (IDetectableComponent detectableComponent in this.detector.Visible)
    {
      if (detectableComponent != null && !detectableComponent.IsDisposed)
      {
        DetectableComponent detectable = (DetectableComponent) detectableComponent;
        if (this.Filter(detectable))
        {
          GameObject gameObject = ((IEntityView) detectable.Owner).GameObject;
          if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
          {
            if (this.ResultList.Value == null)
            {
              this.Result.Value = gameObject.transform;
              return TaskStatus.Success;
            }
            this.ResultList.Value.Add(gameObject.transform);
          }
        }
      }
    }
    if (this.ResultList.Value == null || this.ResultList.Value.Count == 0)
      return TaskStatus.Failure;
    this.Result.Value = this.ResultList.Value[0];
    return TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Result", this.Result);
    BehaviorTreeDataWriteUtility.WriteShared<SharedTransformList>(writer, "ResultList", this.ResultList);
    DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "DetectType", this.DetectType);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Result", this.Result);
    this.ResultList = BehaviorTreeDataReadUtility.ReadShared<SharedTransformList>(reader, "ResultList", this.ResultList);
    this.DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
  }
}
