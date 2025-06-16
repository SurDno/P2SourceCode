// Decompiled with JetBrains decompiler
// Type: FightTargetIsUnvailiable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
[TaskDescription("Fight target is unvailiable")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (FightTargetIsUnvailiable))]
public class FightTargetIsUnvailiable : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedTransform Target;
  protected DetectorComponent detector;
  protected IEntity entity;

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
    if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null)
      return TaskStatus.Success;
    if (this.entity == null || this.detector == null)
      return TaskStatus.Failure;
    if ((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) this.gameObject.transform || !this.Target.Value.gameObject.activeSelf)
      return TaskStatus.Success;
    if (this.gameObject.GetComponent<EnemyBase>().IsFaint)
      return TaskStatus.Failure;
    List<IEntity> entityList = new List<IEntity>();
    foreach (IDetectableComponent detectable in this.detector.Visible)
    {
      if (this.DetectableIsTarget(detectable))
        return TaskStatus.Failure;
    }
    foreach (IDetectableComponent detectable in this.detector.Hearing)
    {
      if (this.DetectableIsTarget(detectable))
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
    return !((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && (UnityEngine.Object) gameObject.transform == (UnityEngine.Object) this.Target.Value;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
  }
}
