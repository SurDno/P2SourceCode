﻿using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (POISequenceCrowdForTestScene))]
public class POISequenceCrowdForTestScene : 
  POISequence,
  IStub,
  ISerializeDataWrite,
  ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public SharedTransform SearchGO;

  protected override void FillPOICache()
  {
    poiCache.Clear();
    FindPOIsInGameObject(SearchGO.Value.gameObject, poiCache);
    FilterIsChilds(poiCache);
  }

  public new void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteTaskList(writer, "Children", children);
    DefaultDataWriteUtility.WriteEnum(writer, "AbortType", abortType);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "SearchEntity", SearchEntity);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "InSearchRadius", InSearchRadius);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "FindClosest", FindClosest);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "AvoidLastUsedPoiNumber", AvoidLastUsedPoiNumber);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "OutDestination", OutDestination);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "OutRotation", OutRotation);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Interior", Interior);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "SameRegionOnly", SameRegionOnly);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "DebugModeOn", DebugModeOn);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "SearchingArea", SearchingArea);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "TotalUnlockedPois", TotalUnlockedPois);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "PlayablePois", PlayablePois);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "PlayablePoisInRadius", PlayablePoisInRadius);
    DefaultDataWriteUtility.Write(writer, "LongAnimationsOnly", LongAnimationsOnly);
    DefaultDataWriteUtility.WriteEnum(writer, "OutPOIAnimation", OutPOIAnimation);
    DefaultDataWriteUtility.Write(writer, "OutAnimationIndex", OutAnimationIndex);
    DefaultDataWriteUtility.Write(writer, "OutMiddleAnimationsCount", OutMiddleAnimationsCount);
    DefaultDataWriteUtility.Write(writer, "OutAngle", OutAngle);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "SearchGO", SearchGO);
  }

  public new void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    children = BehaviorTreeDataReadUtility.ReadTaskList(reader, "Children", children);
    abortType = DefaultDataReadUtility.ReadEnum<AbortType>(reader, "AbortType");
    SearchEntity = BehaviorTreeDataReadUtility.ReadShared(reader, "SearchEntity", SearchEntity);
    InSearchRadius = BehaviorTreeDataReadUtility.ReadShared(reader, "InSearchRadius", InSearchRadius);
    FindClosest = BehaviorTreeDataReadUtility.ReadShared(reader, "FindClosest", FindClosest);
    AvoidLastUsedPoiNumber = BehaviorTreeDataReadUtility.ReadShared(reader, "AvoidLastUsedPoiNumber", AvoidLastUsedPoiNumber);
    OutDestination = BehaviorTreeDataReadUtility.ReadShared(reader, "OutDestination", OutDestination);
    OutRotation = BehaviorTreeDataReadUtility.ReadShared(reader, "OutRotation", OutRotation);
    Interior = BehaviorTreeDataReadUtility.ReadShared(reader, "Interior", Interior);
    SameRegionOnly = BehaviorTreeDataReadUtility.ReadShared(reader, "SameRegionOnly", SameRegionOnly);
    DebugModeOn = BehaviorTreeDataReadUtility.ReadShared(reader, "DebugModeOn", DebugModeOn);
    SearchingArea = BehaviorTreeDataReadUtility.ReadShared(reader, "SearchingArea", SearchingArea);
    TotalUnlockedPois = BehaviorTreeDataReadUtility.ReadShared(reader, "TotalUnlockedPois", TotalUnlockedPois);
    PlayablePois = BehaviorTreeDataReadUtility.ReadShared(reader, "PlayablePois", PlayablePois);
    PlayablePoisInRadius = BehaviorTreeDataReadUtility.ReadShared(reader, "PlayablePoisInRadius", PlayablePoisInRadius);
    LongAnimationsOnly = DefaultDataReadUtility.Read(reader, "LongAnimationsOnly", LongAnimationsOnly);
    OutPOIAnimation = DefaultDataReadUtility.ReadEnum<POIAnimationEnum>(reader, "OutPOIAnimation");
    OutAnimationIndex = DefaultDataReadUtility.Read(reader, "OutAnimationIndex", OutAnimationIndex);
    OutMiddleAnimationsCount = DefaultDataReadUtility.Read(reader, "OutMiddleAnimationsCount", OutMiddleAnimationsCount);
    OutAngle = DefaultDataReadUtility.Read(reader, "OutAngle", OutAngle);
    SearchGO = BehaviorTreeDataReadUtility.ReadShared(reader, "SearchGO", SearchGO);
  }
}
