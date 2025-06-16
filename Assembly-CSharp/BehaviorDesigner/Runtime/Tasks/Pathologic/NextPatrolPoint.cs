// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.NextPatrolPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Choose next patrol point")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (NextPatrolPoint))]
  public class NextPatrolPoint : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform PatrolTransform;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedInt CurrentIndex;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Result;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool EndPatrol;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool InversePath = (SharedBool) false;
    private PatrolTypeEnum PatollingType;
    private bool inited;
    private PatrolPath patrolPath;
    private List<Transform> patrolPoints;

    public override void OnStart()
    {
      this.inited = false;
      if ((UnityEngine.Object) this.PatrolTransform.Value != (UnityEngine.Object) null)
      {
        this.patrolPath = this.PatrolTransform.Value.GetComponent<PatrolPath>();
      }
      else
      {
        IEntity owner = this.Owner.GetComponent<EngineGameObject>().Owner;
        if (owner == null)
        {
          Debug.LogWarningFormat("{0} has no entity", (object) this.gameObject.name);
          return;
        }
        IEntity setupPoint = owner.GetComponent<NavigationComponent>().SetupPoint;
        if (setupPoint != null)
          this.patrolPath = ((IEntityView) setupPoint).GameObject?.GetComponent<PatrolPath>();
        if ((UnityEngine.Object) this.patrolPath == (UnityEngine.Object) null)
        {
          CrowdItemComponent component = owner.GetComponent<CrowdItemComponent>();
          if (component != null)
            this.patrolPath = component.Point?.GameObject?.GetComponent<PatrolPath>();
        }
      }
      if ((UnityEngine.Object) this.patrolPath == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0} has null patrol points", (object) this.gameObject.name);
      }
      else
      {
        this.PatollingType = this.patrolPath.PatrolType;
        if (this.patrolPath.transform.childCount < 2)
        {
          Debug.LogWarningFormat("{0} contains less than two patrol points", (object) this.patrolPath.gameObject.name);
        }
        else
        {
          this.patrolPoints = this.patrolPath.PointsList;
          this.inited = true;
        }
      }
    }

    public override TaskStatus OnUpdate()
    {
      if (!this.inited || this.patrolPoints == null || this.patrolPoints.Count == 0)
        return TaskStatus.Failure;
      if (this.PatollingType == PatrolTypeEnum.Looped)
        this.CurrentIndex.Value = (this.CurrentIndex.Value + 1) % this.patrolPoints.Count;
      else if (this.PatollingType == PatrolTypeEnum.ToTheEndAndBack)
      {
        if (!this.InversePath.Value)
        {
          ++this.CurrentIndex.Value;
          if (this.CurrentIndex.Value >= this.patrolPoints.Count)
          {
            this.CurrentIndex.Value = this.patrolPoints.Count - 1;
            this.InversePath.Value = true;
          }
        }
        else
        {
          --this.CurrentIndex.Value;
          if (this.CurrentIndex.Value < 0)
          {
            this.CurrentIndex.Value = 0;
            this.InversePath.Value = false;
          }
        }
      }
      else if (this.PatollingType == PatrolTypeEnum.ToTheEndAndStop)
      {
        ++this.CurrentIndex.Value;
        if (this.EndPatrol != null)
          this.EndPatrol.Value = this.CurrentIndex.Value >= this.patrolPoints.Count;
      }
      if (this.CurrentIndex.Value < this.patrolPoints.Count)
        this.Result.Value = this.patrolPoints[this.CurrentIndex.Value];
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "PatrolTransform", this.PatrolTransform);
      BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "CurrentIndex", this.CurrentIndex);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Result", this.Result);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "EndPatrol", this.EndPatrol);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "InversePath", this.InversePath);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.PatrolTransform = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "PatrolTransform", this.PatrolTransform);
      this.CurrentIndex = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "CurrentIndex", this.CurrentIndex);
      this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Result", this.Result);
      this.EndPatrol = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "EndPatrol", this.EndPatrol);
      this.InversePath = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "InversePath", this.InversePath);
    }
  }
}
