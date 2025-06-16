using System;
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
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Choose next random point point")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (NextRandomPoint))]
  public class NextRandomPoint : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Wander radius")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat SearchRadius = 50f;
    [Tooltip("Walk area type")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public WalkArea WalkAreaType = WalkArea.All;
    [Tooltip("Point will be written to this variable.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedVector3 Result;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool SameRegionOnly = true;
    private IRegionComponent region;

    public override void OnStart()
    {
      if (!SameRegionOnly.Value)
        return;
      IEntity entity = EntityUtility.GetEntity(gameObject);
      if (entity == null)
        return;
      NavigationComponent component = entity.GetComponent<NavigationComponent>();
      if (component == null)
        return;
      region = component.Region;
    }

    public override TaskStatus OnUpdate() => Next() ? TaskStatus.Success : TaskStatus.Failure;

    public bool Next()
    {
      int num = 10;
      float maxDistance1 = 5f;
      NavMeshHit hit = new NavMeshHit();
      int mask;
      if (WalkAreaType == WalkArea.Road)
        mask = AreaEnum.Road.ToMask();
      else if (WalkAreaType == WalkArea.RadAndFootpath)
        mask = AreaEnum.RoadFootPath.ToMask();
      else if (WalkAreaType == WalkArea.All)
      {
        mask = AreaEnum.All.ToMask();
      }
      else
      {
        Debug.LogWarningFormat("{0} not supported area type", gameObject.name);
        return false;
      }
      for (int index = 0; index < num; ++index)
      {
        float f = Random.Range(0, 360);
        Vector3 position = gameObject.transform.position;
        position.x += SearchRadius.Value * Mathf.Sin(f);
        position.z += SearchRadius.Value * Mathf.Cos(f);
        if (NavMesh.SamplePosition(position, out hit, maxDistance1, mask) && CheckRegion(hit.position))
        {
          Result.Value = hit.position;
          return true;
        }
      }
      float maxDistance2 = maxDistance1 * 2f;
      for (int index = 0; index < num; ++index)
      {
        float f = Random.Range(0, 360);
        Vector3 position = gameObject.transform.position;
        position.x += SearchRadius.Value * Mathf.Sin(f);
        position.z += SearchRadius.Value * Mathf.Cos(f);
        if (NavMesh.SamplePosition(position, out hit, maxDistance2, mask) && CheckRegion(hit.position))
        {
          Result.Value = hit.position;
          return true;
        }
      }
      return false;
    }

    private bool CheckRegion(Vector3 position)
    {
      if (!SameRegionOnly.Value || region == null)
        return true;
      RegionComponent regionByPosition = RegionUtility.GetRegionByPosition(position);
      return regionByPosition != null && region.Region == regionByPosition.Region;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SearchRadius", SearchRadius);
      DefaultDataWriteUtility.WriteEnum(writer, "WalkAreaType", WalkAreaType);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Result", Result);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SameRegionOnly", SameRegionOnly);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      SearchRadius = BehaviorTreeDataReadUtility.ReadShared(reader, "SearchRadius", SearchRadius);
      WalkAreaType = DefaultDataReadUtility.ReadEnum<WalkArea>(reader, "WalkAreaType");
      Result = BehaviorTreeDataReadUtility.ReadShared(reader, "Result", Result);
      SameRegionOnly = BehaviorTreeDataReadUtility.ReadShared(reader, "SameRegionOnly", SameRegionOnly);
    }

    public enum WalkArea
    {
      Road = 1,
      RadAndFootpath = 2,
      All = 100,
    }
  }
}
