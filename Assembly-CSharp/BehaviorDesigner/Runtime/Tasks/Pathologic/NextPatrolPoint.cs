using System;
using System.Collections.Generic;
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

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Choose next patrol point")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (NextPatrolPoint))]
  public class NextPatrolPoint : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform PatrolTransform;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedInt CurrentIndex;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Result;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool EndPatrol;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool InversePath = false;
    private PatrolTypeEnum PatollingType;
    private bool inited;
    private PatrolPath patrolPath;
    private List<Transform> patrolPoints;

    public override void OnStart()
    {
      inited = false;
      if ((UnityEngine.Object) PatrolTransform.Value != (UnityEngine.Object) null)
      {
        patrolPath = PatrolTransform.Value.GetComponent<PatrolPath>();
      }
      else
      {
        IEntity owner = Owner.GetComponent<EngineGameObject>().Owner;
        if (owner == null)
        {
          Debug.LogWarningFormat("{0} has no entity", (object) gameObject.name);
          return;
        }
        IEntity setupPoint = owner.GetComponent<NavigationComponent>().SetupPoint;
        if (setupPoint != null)
          patrolPath = ((IEntityView) setupPoint).GameObject?.GetComponent<PatrolPath>();
        if ((UnityEngine.Object) patrolPath == (UnityEngine.Object) null)
        {
          CrowdItemComponent component = owner.GetComponent<CrowdItemComponent>();
          if (component != null)
            patrolPath = component.Point?.GameObject?.GetComponent<PatrolPath>();
        }
      }
      if ((UnityEngine.Object) patrolPath == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("{0} has null patrol points", (object) gameObject.name);
      }
      else
      {
        PatollingType = patrolPath.PatrolType;
        if (patrolPath.transform.childCount < 2)
        {
          Debug.LogWarningFormat("{0} contains less than two patrol points", (object) patrolPath.gameObject.name);
        }
        else
        {
          patrolPoints = patrolPath.PointsList;
          inited = true;
        }
      }
    }

    public override TaskStatus OnUpdate()
    {
      if (!inited || patrolPoints == null || patrolPoints.Count == 0)
        return TaskStatus.Failure;
      if (PatollingType == PatrolTypeEnum.Looped)
        CurrentIndex.Value = (CurrentIndex.Value + 1) % patrolPoints.Count;
      else if (PatollingType == PatrolTypeEnum.ToTheEndAndBack)
      {
        if (!InversePath.Value)
        {
          ++CurrentIndex.Value;
          if (CurrentIndex.Value >= patrolPoints.Count)
          {
            CurrentIndex.Value = patrolPoints.Count - 1;
            InversePath.Value = true;
          }
        }
        else
        {
          --CurrentIndex.Value;
          if (CurrentIndex.Value < 0)
          {
            CurrentIndex.Value = 0;
            InversePath.Value = false;
          }
        }
      }
      else if (PatollingType == PatrolTypeEnum.ToTheEndAndStop)
      {
        ++CurrentIndex.Value;
        if (EndPatrol != null)
          EndPatrol.Value = CurrentIndex.Value >= patrolPoints.Count;
      }
      if (CurrentIndex.Value < patrolPoints.Count)
        Result.Value = patrolPoints[CurrentIndex.Value];
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "PatrolTransform", PatrolTransform);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "CurrentIndex", CurrentIndex);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "EndPatrol", EndPatrol);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "InversePath", InversePath);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      PatrolTransform = BehaviorTreeDataReadUtility.ReadShared(reader, "PatrolTransform", PatrolTransform);
      CurrentIndex = BehaviorTreeDataReadUtility.ReadShared(reader, "CurrentIndex", CurrentIndex);
      Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
      EndPatrol = BehaviorTreeDataReadUtility.ReadShared(reader, "EndPatrol", EndPatrol);
      InversePath = BehaviorTreeDataReadUtility.ReadShared(reader, "InversePath", InversePath);
    }
  }
}
