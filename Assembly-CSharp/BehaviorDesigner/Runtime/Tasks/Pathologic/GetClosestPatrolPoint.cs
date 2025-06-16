// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.GetClosestPatrolPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components.Utilities;
using Scripts.Tools.Serializations.Converters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Get closest patrol point index")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (GetClosestPatrolPoint))]
  public class GetClosestPatrolPoint : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
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
    private bool inited;
    private PatrolPath patrolPath;
    private List<Transform> patrolPoints;

    public override void OnStart()
    {
      this.inited = false;
      this.patrolPath = this.PatrolTransform.Value.GetComponent<PatrolPath>();
      if ((UnityEngine.Object) this.patrolPath == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} has no patrol path", (object) this.gameObject.name);
      }
      else
      {
        this.patrolPoints = this.patrolPath.PointsList;
        if (!this.patrolPath.RestartFromClosestPoint)
        {
          this.inited = true;
        }
        else
        {
          float num1 = float.PositiveInfinity;
          int num2 = 0;
          for (int index = 0; index < this.patrolPoints.Count; ++index)
          {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(this.patrolPoints[index].position, out hit, 1f, -1))
            {
              NavMeshPath path = new NavMeshPath();
              if (NavMesh.CalculatePath(hit.position, this.Owner.transform.position, -1, path))
              {
                float pathLength = NavMeshUtility.GetPathLength(path);
                if ((double) pathLength < (double) num1)
                {
                  num1 = pathLength;
                  num2 = index;
                }
              }
            }
          }
          this.CurrentIndex.Value = num2;
          this.inited = true;
        }
      }
    }

    public override TaskStatus OnUpdate()
    {
      if (!this.inited || this.patrolPoints == null)
        return TaskStatus.Failure;
      if (this.patrolPoints.Count == 0)
      {
        Debug.LogErrorFormat("{0} has wrong patrol path in {1}", (object) this.gameObject.name, (object) this.PatrolTransform.Value.name);
        return TaskStatus.Failure;
      }
      if (this.CurrentIndex.Value >= this.patrolPoints.Count)
        this.CurrentIndex.Value = this.patrolPoints.Count - 1;
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
    }
  }
}
