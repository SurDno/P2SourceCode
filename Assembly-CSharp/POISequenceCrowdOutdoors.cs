using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.BehaviorNodes;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Crowds;
using Scripts.Tools.Serializations.Converters;

[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (POISequenceCrowdOutdoors))]
public class POISequenceCrowdOutdoors : POISequence, IStub, ISerializeDataWrite, ISerializeDataRead
{
  protected override void FillPOICache()
  {
    this.poiCache.Clear();
    IEntity owner = this.Owner.GetComponent<EngineGameObject>().Owner;
    if (owner == null)
    {
      this.executionStatus = TaskStatus.Failure;
    }
    else
    {
      CrowdItemComponent component = owner.GetComponent<CrowdItemComponent>();
      if (component != null && component.Crowd != null)
      {
        foreach (CrowdPointInfo point in component.Point.Region.GetComponent<CrowdPointsComponent>().Points)
        {
          if (point.Area == component.Area && (UnityEngine.Object) point.GameObject != (UnityEngine.Object) null && point.GameObject.activeInHierarchy)
            this.FindPOIsInGameObject(((IEntityView) point.EntityPoint).GameObject, this.poiCache);
        }
      }
      this.FilterIsChilds(this.poiCache);
    }
  }

  public new void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteTaskList<Task>(writer, "Children", this.children);
    DefaultDataWriteUtility.WriteEnum<AbortType>(writer, "AbortType", this.abortType);
    BehaviorTreeDataWriteUtility.WriteShared<SharedEntity>(writer, "SearchEntity", this.SearchEntity);
    BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "InSearchRadius", this.InSearchRadius);
    BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "FindClosest", this.FindClosest);
    BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "AvoidLastUsedPoiNumber", this.AvoidLastUsedPoiNumber);
    BehaviorTreeDataWriteUtility.WriteShared<SharedVector3>(writer, "OutDestination", this.OutDestination);
    BehaviorTreeDataWriteUtility.WriteShared<SharedQuaternion>(writer, "OutRotation", this.OutRotation);
    BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Interior", this.Interior);
    BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "SameRegionOnly", this.SameRegionOnly);
    BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "DebugModeOn", this.DebugModeOn);
    BehaviorTreeDataWriteUtility.WriteShared<SharedString>(writer, "SearchingArea", this.SearchingArea);
    BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "TotalUnlockedPois", this.TotalUnlockedPois);
    BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PlayablePois", this.PlayablePois);
    BehaviorTreeDataWriteUtility.WriteShared<SharedInt>(writer, "PlayablePoisInRadius", this.PlayablePoisInRadius);
    DefaultDataWriteUtility.Write(writer, "LongAnimationsOnly", this.LongAnimationsOnly);
    DefaultDataWriteUtility.WriteEnum<POIAnimationEnum>(writer, "OutPOIAnimation", this.OutPOIAnimation);
    DefaultDataWriteUtility.Write(writer, "OutAnimationIndex", this.OutAnimationIndex);
    DefaultDataWriteUtility.Write(writer, "OutMiddleAnimationsCount", this.OutMiddleAnimationsCount);
    DefaultDataWriteUtility.Write(writer, "OutAngle", this.OutAngle);
  }

  public new void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.children = BehaviorTreeDataReadUtility.ReadTaskList<Task>(reader, "Children", this.children);
    this.abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
    this.SearchEntity = BehaviorTreeDataReadUtility.ReadShared<SharedEntity>(reader, "SearchEntity", this.SearchEntity);
    this.InSearchRadius = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "InSearchRadius", this.InSearchRadius);
    this.FindClosest = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "FindClosest", this.FindClosest);
    this.AvoidLastUsedPoiNumber = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "AvoidLastUsedPoiNumber", this.AvoidLastUsedPoiNumber);
    this.OutDestination = BehaviorTreeDataReadUtility.ReadShared<SharedVector3>(reader, "OutDestination", this.OutDestination);
    this.OutRotation = BehaviorTreeDataReadUtility.ReadShared<SharedQuaternion>(reader, "OutRotation", this.OutRotation);
    this.Interior = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Interior", this.Interior);
    this.SameRegionOnly = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "SameRegionOnly", this.SameRegionOnly);
    this.DebugModeOn = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "DebugModeOn", this.DebugModeOn);
    this.SearchingArea = BehaviorTreeDataReadUtility.ReadShared<SharedString>(reader, "SearchingArea", this.SearchingArea);
    this.TotalUnlockedPois = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "TotalUnlockedPois", this.TotalUnlockedPois);
    this.PlayablePois = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PlayablePois", this.PlayablePois);
    this.PlayablePoisInRadius = BehaviorTreeDataReadUtility.ReadShared<SharedInt>(reader, "PlayablePoisInRadius", this.PlayablePoisInRadius);
    this.LongAnimationsOnly = DefaultDataReadUtility.Read(reader, "LongAnimationsOnly", this.LongAnimationsOnly);
    this.OutPOIAnimation = DefaultDataReadUtility.ReadEnum<POIAnimationEnum>(reader, "OutPOIAnimation");
    this.OutAnimationIndex = DefaultDataReadUtility.Read(reader, "OutAnimationIndex", this.OutAnimationIndex);
    this.OutMiddleAnimationsCount = DefaultDataReadUtility.Read(reader, "OutMiddleAnimationsCount", this.OutMiddleAnimationsCount);
    this.OutAngle = DefaultDataReadUtility.Read(reader, "OutAngle", this.OutAngle);
  }
}
