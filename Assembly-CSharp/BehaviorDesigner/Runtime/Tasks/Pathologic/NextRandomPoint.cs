using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Movable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Choose next random point point")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (NextRandomPoint))]
  public class NextRandomPoint : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Wander radius")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat SearchRadius = (SharedFloat) 50f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Walk area type")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public NextRandomPoint.WalkArea WalkAreaType = NextRandomPoint.WalkArea.All;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Point will be written to this variable.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedVector3 Result;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool SameRegionOnly = (SharedBool) true;
    private IRegionComponent region;

    public override void OnStart()
    {
      if (!this.SameRegionOnly.Value)
        return;
      IEntity entity = EntityUtility.GetEntity(this.gameObject);
      if (entity == null)
        return;
      NavigationComponent component = entity.GetComponent<NavigationComponent>();
      if (component == null)
        return;
      this.region = component.Region;
    }

    public override TaskStatus OnUpdate() => this.Next() ? TaskStatus.Success : TaskStatus.Failure;

    public bool Next()
    {
      int num = 10;
      float maxDistance1 = 5f;
      NavMeshHit hit = new NavMeshHit();
      int mask;
      if (this.WalkAreaType == NextRandomPoint.WalkArea.Road)
        mask = AreaEnum.Road.ToMask();
      else if (this.WalkAreaType == NextRandomPoint.WalkArea.RadAndFootpath)
        mask = AreaEnum.RoadFootPath.ToMask();
      else if (this.WalkAreaType == NextRandomPoint.WalkArea.All)
      {
        mask = AreaEnum.All.ToMask();
      }
      else
      {
        Debug.LogWarningFormat("{0} not supported area type", (object) this.gameObject.name);
        return false;
      }
      for (int index = 0; index < num; ++index)
      {
        float f = (float) UnityEngine.Random.Range(0, 360);
        Vector3 position = this.gameObject.transform.position;
        position.x += this.SearchRadius.Value * Mathf.Sin(f);
        position.z += this.SearchRadius.Value * Mathf.Cos(f);
        if (NavMesh.SamplePosition(position, out hit, maxDistance1, mask) && this.CheckRegion(hit.position))
        {
          this.Result.Value = hit.position;
          return true;
        }
      }
      float maxDistance2 = maxDistance1 * 2f;
      for (int index = 0; index < num; ++index)
      {
        float f = (float) UnityEngine.Random.Range(0, 360);
        Vector3 position = this.gameObject.transform.position;
        position.x += this.SearchRadius.Value * Mathf.Sin(f);
        position.z += this.SearchRadius.Value * Mathf.Cos(f);
        if (NavMesh.SamplePosition(position, out hit, maxDistance2, mask) && this.CheckRegion(hit.position))
        {
          this.Result.Value = hit.position;
          return true;
        }
      }
      return false;
    }

    private bool CheckRegion(Vector3 position)
    {
      if (!this.SameRegionOnly.Value || this.region == null)
        return true;
      RegionComponent regionByPosition = RegionUtility.GetRegionByPosition(position);
      return regionByPosition != null && this.region.Region == regionByPosition.Region;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "SearchRadius", this.SearchRadius);
      DefaultDataWriteUtility.WriteEnum<NextRandomPoint.WalkArea>(writer, "WalkAreaType", this.WalkAreaType);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "Result", this.Result);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "SameRegionOnly", this.SameRegionOnly);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.SearchRadius = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "SearchRadius", this.SearchRadius);
      this.WalkAreaType = DefaultDataReadUtility.ReadEnum<NextRandomPoint.WalkArea>(reader, "WalkAreaType");
      this.Result = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "Result", this.Result);
      this.SameRegionOnly = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "SameRegionOnly", this.SameRegionOnly);
    }

    public enum WalkArea
    {
      Road = 1,
      RadAndFootpath = 2,
      All = 100, // 0x00000064
    }
  }
}
