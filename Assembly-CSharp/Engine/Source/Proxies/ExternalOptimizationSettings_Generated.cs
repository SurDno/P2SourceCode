using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Movable;
using Engine.Source.Otimizations;
using Engine.Source.Settings.External;
using System;
using System.Runtime;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExternalOptimizationSettings))]
  public class ExternalOptimizationSettings_Generated : 
    ExternalOptimizationSettings,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExternalOptimizationSettings_Generated instance = Activator.CreateInstance<ExternalOptimizationSettings_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExternalOptimizationSettings_Generated settingsGenerated = (ExternalOptimizationSettings_Generated) target2;
      settingsGenerated.Version = this.Version;
      settingsGenerated.CreateDisabledPrefabs = this.CreateDisabledPrefabs;
      settingsGenerated.DestroyDisabledPrefabs = this.DestroyDisabledPrefabs;
      settingsGenerated.ThreadCount = this.ThreadCount;
      settingsGenerated.MinAffinityCount = this.MinAffinityCount;
      settingsGenerated.Affinity = this.Affinity;
      settingsGenerated.LazyFsm = this.LazyFsm;
      settingsGenerated.SmoothHierarchyMapping = this.SmoothHierarchyMapping;
      settingsGenerated.DisableAudio = this.DisableAudio;
      settingsGenerated.DisableLipSync = this.DisableLipSync;
      settingsGenerated.DisableGrass = this.DisableGrass;
      settingsGenerated.PreloadGrass = this.PreloadGrass;
      settingsGenerated.DisableBlueprints = this.DisableBlueprints;
      settingsGenerated.BlueprintsInSeparateThread = this.BlueprintsInSeparateThread;
      settingsGenerated.InteractableAsyncBlueprintStart = this.InteractableAsyncBlueprintStart;
      settingsGenerated.DisableBehaviourTree = this.DisableBehaviourTree;
      settingsGenerated.PauseBehaviourTree = this.PauseBehaviourTree;
      settingsGenerated.DisableLeaf = this.DisableLeaf;
      settingsGenerated.DisableDetectors = this.DisableDetectors;
      settingsGenerated.ReduceUpdateFarObjects = this.ReduceUpdateFarObjects;
      settingsGenerated.SaveCompress = this.SaveCompress;
      settingsGenerated.PreloadBehaviors = this.PreloadBehaviors;
      settingsGenerated.PreloadBlueprints = this.PreloadBlueprints;
      settingsGenerated.UseCompressedTemplates = this.UseCompressedTemplates;
      settingsGenerated.TargetFrameRate = this.TargetFrameRate;
      settingsGenerated.LatencyMode = this.LatencyMode;
      settingsGenerated.PathfindingIterationsPerFrame = this.PathfindingIterationsPerFrame;
      settingsGenerated.ReduceUpdateFarObjectsDistance = this.ReduceUpdateFarObjectsDistance;
      settingsGenerated.UsePlayerPrefs = this.UsePlayerPrefs;
      settingsGenerated.DetectorUpdateDelay = this.DetectorUpdateDelay;
      settingsGenerated.NavigationUpdateDelay = this.NavigationUpdateDelay;
      settingsGenerated.IndoorCrowdUpdateDelay = this.IndoorCrowdUpdateDelay;
      settingsGenerated.OutdoorCrowdUpdateDelay = this.OutdoorCrowdUpdateDelay;
      settingsGenerated.EffectUpdateDelay = this.EffectUpdateDelay;
      settingsGenerated.DiseaseUpdateDelay = this.DiseaseUpdateDelay;
      settingsGenerated.LeafUpdateDelay = this.LeafUpdateDelay;
      settingsGenerated.LeafSpawnDelay = this.LeafSpawnDelay;
      settingsGenerated.SoundUpdateDelay = this.SoundUpdateDelay;
      settingsGenerated.BehaviorTreeUpdateDelay = this.BehaviorTreeUpdateDelay;
      settingsGenerated.BlueprintUpdateDelay = this.BlueprintUpdateDelay;
      settingsGenerated.WeatherUpdateDelay = this.WeatherUpdateDelay;
      settingsGenerated.PlayerUpdateDelay = this.PlayerUpdateDelay;
      settingsGenerated.EnvironmentUpdateDelay = this.EnvironmentUpdateDelay;
      settingsGenerated.FlockSpawnDelay = this.FlockSpawnDelay;
      settingsGenerated.FlockMoveDelay = this.FlockMoveDelay;
      settingsGenerated.FlockCastDelay = this.FlockCastDelay;
      settingsGenerated.PlagueZoneDelay = this.PlagueZoneDelay;
      settingsGenerated.BlueprintSoundsDelay = this.BlueprintSoundsDelay;
      settingsGenerated.BlueprintEffectsDelay = this.BlueprintEffectsDelay;
      settingsGenerated.LightShaftsDelay = this.LightShaftsDelay;
      settingsGenerated.CrowdSpawnMinDistance = this.CrowdSpawnMinDistance;
      settingsGenerated.CrowdSpawnMaxDistance = this.CrowdSpawnMaxDistance;
      settingsGenerated.CrowdDestroyDistance = this.CrowdDestroyDistance;
      settingsGenerated.DestroyOutdoorCrowdInIndoor = this.DestroyOutdoorCrowdInIndoor;
      settingsGenerated.RegionDistanceDelay = this.RegionDistanceDelay;
      settingsGenerated.MaxOutdoorCrowdEntityCount = this.MaxOutdoorCrowdEntityCount;
      CloneableObjectUtility.FillListTo<AreaEnum>(settingsGenerated.LimitOutdoorAreaCrowdEntity, this.LimitOutdoorAreaCrowdEntity);
      settingsGenerated.MultipleSceneLoader = this.MultipleSceneLoader;
      settingsGenerated.VirtualMachineDebug = this.VirtualMachineDebug;
      settingsGenerated.VMEventsQueuePerFrameTimeMax = this.VMEventsQueuePerFrameTimeMax;
      settingsGenerated.ChangeLocationLoadingWindow = this.ChangeLocationLoadingWindow;
      settingsGenerated.ReleaseStrategyIndex = this.ReleaseStrategyIndex;
      settingsGenerated.DevelopmentStrategyIndex = this.DevelopmentStrategyIndex;
      settingsGenerated.EditorStrategyIndex = this.EditorStrategyIndex;
      settingsGenerated.ConsoleStrategyIndex = this.ConsoleStrategyIndex;
      CloneableObjectUtility.CopyListTo<IMemoryStrategy>(settingsGenerated.MemoryStrategies, this.MemoryStrategies);
      settingsGenerated.MemoryStrategyTimeEventPeriod = this.MemoryStrategyTimeEventPeriod;
      settingsGenerated.useAiLods = this.useAiLods;
      settingsGenerated.AiLodsDistance = this.AiLodsDistance;
      settingsGenerated.AiLodsMinDistance = this.AiLodsMinDistance;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", this.Version);
      DefaultDataWriteUtility.Write(writer, "CreateDisabledPrefabs", this.CreateDisabledPrefabs);
      DefaultDataWriteUtility.Write(writer, "DestroyDisabledPrefabs", this.DestroyDisabledPrefabs);
      DefaultDataWriteUtility.Write(writer, "ThreadCount", this.ThreadCount);
      DefaultDataWriteUtility.Write(writer, "MinAffinityCount", this.MinAffinityCount);
      DefaultDataWriteUtility.Write(writer, "Affinity", this.Affinity);
      DefaultDataWriteUtility.Write(writer, "LazyFsm", this.LazyFsm);
      DefaultDataWriteUtility.Write(writer, "SmoothHierarchyMapping", this.SmoothHierarchyMapping);
      DefaultDataWriteUtility.Write(writer, "DisableAudio", this.DisableAudio);
      DefaultDataWriteUtility.Write(writer, "DisableLipSync", this.DisableLipSync);
      DefaultDataWriteUtility.Write(writer, "DisableGrass", this.DisableGrass);
      DefaultDataWriteUtility.Write(writer, "PreloadGrass", this.PreloadGrass);
      DefaultDataWriteUtility.Write(writer, "DisableBlueprints", this.DisableBlueprints);
      DefaultDataWriteUtility.Write(writer, "BlueprintsInSeparateThread", this.BlueprintsInSeparateThread);
      DefaultDataWriteUtility.Write(writer, "InteractableAsyncBlueprintStart", this.InteractableAsyncBlueprintStart);
      DefaultDataWriteUtility.Write(writer, "DisableBehaviourTree", this.DisableBehaviourTree);
      DefaultDataWriteUtility.Write(writer, "PauseBehaviourTree", this.PauseBehaviourTree);
      DefaultDataWriteUtility.Write(writer, "DisableLeaf", this.DisableLeaf);
      DefaultDataWriteUtility.Write(writer, "DisableDetectors", this.DisableDetectors);
      DefaultDataWriteUtility.Write(writer, "ReduceUpdateFarObjects", this.ReduceUpdateFarObjects);
      DefaultDataWriteUtility.Write(writer, "SaveCompress", this.SaveCompress);
      DefaultDataWriteUtility.Write(writer, "PreloadBehaviors", this.PreloadBehaviors);
      DefaultDataWriteUtility.Write(writer, "PreloadBlueprints", this.PreloadBlueprints);
      DefaultDataWriteUtility.Write(writer, "UseCompressedTemplates", this.UseCompressedTemplates);
      DefaultDataWriteUtility.Write(writer, "TargetFrameRate", this.TargetFrameRate);
      DefaultDataWriteUtility.WriteEnum<GCLatencyMode>(writer, "LatencyMode", this.LatencyMode);
      DefaultDataWriteUtility.Write(writer, "PathfindingIterationsPerFrame", this.PathfindingIterationsPerFrame);
      DefaultDataWriteUtility.Write(writer, "ReduceUpdateFarObjectsDistance", this.ReduceUpdateFarObjectsDistance);
      DefaultDataWriteUtility.Write(writer, "UsePlayerPrefs", this.UsePlayerPrefs);
      DefaultDataWriteUtility.Write(writer, "DetectorUpdateDelay", this.DetectorUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "NavigationUpdateDelay", this.NavigationUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "IndoorCrowdUpdateDelay", this.IndoorCrowdUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "OutdoorCrowdUpdateDelay", this.OutdoorCrowdUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "EffectUpdateDelay", this.EffectUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "DiseaseUpdateDelay", this.DiseaseUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "LeafUpdateDelay", this.LeafUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "LeafSpawnDelay", this.LeafSpawnDelay);
      DefaultDataWriteUtility.Write(writer, "SoundUpdateDelay", this.SoundUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "BehaviorTreeUpdateDelay", this.BehaviorTreeUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "BlueprintUpdateDelay", this.BlueprintUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "WeatherUpdateDelay", this.WeatherUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "PlayerUpdateDelay", this.PlayerUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "EnvironmentUpdateDelay", this.EnvironmentUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "FlockSpawnDelay", this.FlockSpawnDelay);
      DefaultDataWriteUtility.Write(writer, "FlockMoveDelay", this.FlockMoveDelay);
      DefaultDataWriteUtility.Write(writer, "FlockCastDelay", this.FlockCastDelay);
      DefaultDataWriteUtility.Write(writer, "PlagueZoneDelay", this.PlagueZoneDelay);
      DefaultDataWriteUtility.Write(writer, "BlueprintSoundsDelay", this.BlueprintSoundsDelay);
      DefaultDataWriteUtility.Write(writer, "BlueprintEffectsDelay", this.BlueprintEffectsDelay);
      DefaultDataWriteUtility.Write(writer, "LightShaftsDelay", this.LightShaftsDelay);
      DefaultDataWriteUtility.Write(writer, "CrowdSpawnMinDistance", this.CrowdSpawnMinDistance);
      DefaultDataWriteUtility.Write(writer, "CrowdSpawnMaxDistance", this.CrowdSpawnMaxDistance);
      DefaultDataWriteUtility.Write(writer, "CrowdDestroyDistance", this.CrowdDestroyDistance);
      DefaultDataWriteUtility.Write(writer, "DestroyOutdoorCrowdInIndoor", this.DestroyOutdoorCrowdInIndoor);
      DefaultDataWriteUtility.Write(writer, "RegionDistanceDelay", this.RegionDistanceDelay);
      DefaultDataWriteUtility.Write(writer, "MaxOutdoorCrowdEntityCount", this.MaxOutdoorCrowdEntityCount);
      DefaultDataWriteUtility.WriteListEnum<AreaEnum>(writer, "LimitOutdoorAreaCrowdEntity", this.LimitOutdoorAreaCrowdEntity);
      DefaultDataWriteUtility.Write(writer, "MultipleSceneLoader", this.MultipleSceneLoader);
      DefaultDataWriteUtility.Write(writer, "VirtualMachineDebug", this.VirtualMachineDebug);
      DefaultDataWriteUtility.Write(writer, "VMEventsQueuePerFrameTimeMax", this.VMEventsQueuePerFrameTimeMax);
      DefaultDataWriteUtility.Write(writer, "ChangeLocationLoadingWindow", this.ChangeLocationLoadingWindow);
      DefaultDataWriteUtility.Write(writer, "ReleaseStrategyIndex", this.ReleaseStrategyIndex);
      DefaultDataWriteUtility.Write(writer, "DevelopmentStrategyIndex", this.DevelopmentStrategyIndex);
      DefaultDataWriteUtility.Write(writer, "EditorStrategyIndex", this.EditorStrategyIndex);
      DefaultDataWriteUtility.Write(writer, "ConsoleStrategyIndex", this.ConsoleStrategyIndex);
      DefaultDataWriteUtility.WriteListSerialize<IMemoryStrategy>(writer, "MemoryStrategies", this.MemoryStrategies);
      DefaultDataWriteUtility.Write(writer, "MemoryStrategyTimeEventPeriod", this.MemoryStrategyTimeEventPeriod);
      DefaultDataWriteUtility.Write(writer, "UseAiLods", this.useAiLods);
      DefaultDataWriteUtility.Write(writer, "AiLodsDistance", this.AiLodsDistance);
      DefaultDataWriteUtility.Write(writer, "AiLodsMinDistance", this.AiLodsMinDistance);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Version = DefaultDataReadUtility.Read(reader, "Version", this.Version);
      this.CreateDisabledPrefabs = DefaultDataReadUtility.Read(reader, "CreateDisabledPrefabs", this.CreateDisabledPrefabs);
      this.DestroyDisabledPrefabs = DefaultDataReadUtility.Read(reader, "DestroyDisabledPrefabs", this.DestroyDisabledPrefabs);
      this.ThreadCount = DefaultDataReadUtility.Read(reader, "ThreadCount", this.ThreadCount);
      this.MinAffinityCount = DefaultDataReadUtility.Read(reader, "MinAffinityCount", this.MinAffinityCount);
      this.Affinity = DefaultDataReadUtility.Read(reader, "Affinity", this.Affinity);
      this.LazyFsm = DefaultDataReadUtility.Read(reader, "LazyFsm", this.LazyFsm);
      this.SmoothHierarchyMapping = DefaultDataReadUtility.Read(reader, "SmoothHierarchyMapping", this.SmoothHierarchyMapping);
      this.DisableAudio = DefaultDataReadUtility.Read(reader, "DisableAudio", this.DisableAudio);
      this.DisableLipSync = DefaultDataReadUtility.Read(reader, "DisableLipSync", this.DisableLipSync);
      this.DisableGrass = DefaultDataReadUtility.Read(reader, "DisableGrass", this.DisableGrass);
      this.PreloadGrass = DefaultDataReadUtility.Read(reader, "PreloadGrass", this.PreloadGrass);
      this.DisableBlueprints = DefaultDataReadUtility.Read(reader, "DisableBlueprints", this.DisableBlueprints);
      this.BlueprintsInSeparateThread = DefaultDataReadUtility.Read(reader, "BlueprintsInSeparateThread", this.BlueprintsInSeparateThread);
      this.InteractableAsyncBlueprintStart = DefaultDataReadUtility.Read(reader, "InteractableAsyncBlueprintStart", this.InteractableAsyncBlueprintStart);
      this.DisableBehaviourTree = DefaultDataReadUtility.Read(reader, "DisableBehaviourTree", this.DisableBehaviourTree);
      this.PauseBehaviourTree = DefaultDataReadUtility.Read(reader, "PauseBehaviourTree", this.PauseBehaviourTree);
      this.DisableLeaf = DefaultDataReadUtility.Read(reader, "DisableLeaf", this.DisableLeaf);
      this.DisableDetectors = DefaultDataReadUtility.Read(reader, "DisableDetectors", this.DisableDetectors);
      this.ReduceUpdateFarObjects = DefaultDataReadUtility.Read(reader, "ReduceUpdateFarObjects", this.ReduceUpdateFarObjects);
      this.SaveCompress = DefaultDataReadUtility.Read(reader, "SaveCompress", this.SaveCompress);
      this.PreloadBehaviors = DefaultDataReadUtility.Read(reader, "PreloadBehaviors", this.PreloadBehaviors);
      this.PreloadBlueprints = DefaultDataReadUtility.Read(reader, "PreloadBlueprints", this.PreloadBlueprints);
      this.UseCompressedTemplates = DefaultDataReadUtility.Read(reader, "UseCompressedTemplates", this.UseCompressedTemplates);
      this.TargetFrameRate = DefaultDataReadUtility.Read(reader, "TargetFrameRate", this.TargetFrameRate);
      this.LatencyMode = DefaultDataReadUtility.ReadEnum<GCLatencyMode>(reader, "LatencyMode");
      this.PathfindingIterationsPerFrame = DefaultDataReadUtility.Read(reader, "PathfindingIterationsPerFrame", this.PathfindingIterationsPerFrame);
      this.ReduceUpdateFarObjectsDistance = DefaultDataReadUtility.Read(reader, "ReduceUpdateFarObjectsDistance", this.ReduceUpdateFarObjectsDistance);
      this.UsePlayerPrefs = DefaultDataReadUtility.Read(reader, "UsePlayerPrefs", this.UsePlayerPrefs);
      this.DetectorUpdateDelay = DefaultDataReadUtility.Read(reader, "DetectorUpdateDelay", this.DetectorUpdateDelay);
      this.NavigationUpdateDelay = DefaultDataReadUtility.Read(reader, "NavigationUpdateDelay", this.NavigationUpdateDelay);
      this.IndoorCrowdUpdateDelay = DefaultDataReadUtility.Read(reader, "IndoorCrowdUpdateDelay", this.IndoorCrowdUpdateDelay);
      this.OutdoorCrowdUpdateDelay = DefaultDataReadUtility.Read(reader, "OutdoorCrowdUpdateDelay", this.OutdoorCrowdUpdateDelay);
      this.EffectUpdateDelay = DefaultDataReadUtility.Read(reader, "EffectUpdateDelay", this.EffectUpdateDelay);
      this.DiseaseUpdateDelay = DefaultDataReadUtility.Read(reader, "DiseaseUpdateDelay", this.DiseaseUpdateDelay);
      this.LeafUpdateDelay = DefaultDataReadUtility.Read(reader, "LeafUpdateDelay", this.LeafUpdateDelay);
      this.LeafSpawnDelay = DefaultDataReadUtility.Read(reader, "LeafSpawnDelay", this.LeafSpawnDelay);
      this.SoundUpdateDelay = DefaultDataReadUtility.Read(reader, "SoundUpdateDelay", this.SoundUpdateDelay);
      this.BehaviorTreeUpdateDelay = DefaultDataReadUtility.Read(reader, "BehaviorTreeUpdateDelay", this.BehaviorTreeUpdateDelay);
      this.BlueprintUpdateDelay = DefaultDataReadUtility.Read(reader, "BlueprintUpdateDelay", this.BlueprintUpdateDelay);
      this.WeatherUpdateDelay = DefaultDataReadUtility.Read(reader, "WeatherUpdateDelay", this.WeatherUpdateDelay);
      this.PlayerUpdateDelay = DefaultDataReadUtility.Read(reader, "PlayerUpdateDelay", this.PlayerUpdateDelay);
      this.EnvironmentUpdateDelay = DefaultDataReadUtility.Read(reader, "EnvironmentUpdateDelay", this.EnvironmentUpdateDelay);
      this.FlockSpawnDelay = DefaultDataReadUtility.Read(reader, "FlockSpawnDelay", this.FlockSpawnDelay);
      this.FlockMoveDelay = DefaultDataReadUtility.Read(reader, "FlockMoveDelay", this.FlockMoveDelay);
      this.FlockCastDelay = DefaultDataReadUtility.Read(reader, "FlockCastDelay", this.FlockCastDelay);
      this.PlagueZoneDelay = DefaultDataReadUtility.Read(reader, "PlagueZoneDelay", this.PlagueZoneDelay);
      this.BlueprintSoundsDelay = DefaultDataReadUtility.Read(reader, "BlueprintSoundsDelay", this.BlueprintSoundsDelay);
      this.BlueprintEffectsDelay = DefaultDataReadUtility.Read(reader, "BlueprintEffectsDelay", this.BlueprintEffectsDelay);
      this.LightShaftsDelay = DefaultDataReadUtility.Read(reader, "LightShaftsDelay", this.LightShaftsDelay);
      this.CrowdSpawnMinDistance = DefaultDataReadUtility.Read(reader, "CrowdSpawnMinDistance", this.CrowdSpawnMinDistance);
      this.CrowdSpawnMaxDistance = DefaultDataReadUtility.Read(reader, "CrowdSpawnMaxDistance", this.CrowdSpawnMaxDistance);
      this.CrowdDestroyDistance = DefaultDataReadUtility.Read(reader, "CrowdDestroyDistance", this.CrowdDestroyDistance);
      this.DestroyOutdoorCrowdInIndoor = DefaultDataReadUtility.Read(reader, "DestroyOutdoorCrowdInIndoor", this.DestroyOutdoorCrowdInIndoor);
      this.RegionDistanceDelay = DefaultDataReadUtility.Read(reader, "RegionDistanceDelay", this.RegionDistanceDelay);
      this.MaxOutdoorCrowdEntityCount = DefaultDataReadUtility.Read(reader, "MaxOutdoorCrowdEntityCount", this.MaxOutdoorCrowdEntityCount);
      this.LimitOutdoorAreaCrowdEntity = DefaultDataReadUtility.ReadListEnum<AreaEnum>(reader, "LimitOutdoorAreaCrowdEntity", this.LimitOutdoorAreaCrowdEntity);
      this.MultipleSceneLoader = DefaultDataReadUtility.Read(reader, "MultipleSceneLoader", this.MultipleSceneLoader);
      this.VirtualMachineDebug = DefaultDataReadUtility.Read(reader, "VirtualMachineDebug", this.VirtualMachineDebug);
      this.VMEventsQueuePerFrameTimeMax = DefaultDataReadUtility.Read(reader, "VMEventsQueuePerFrameTimeMax", this.VMEventsQueuePerFrameTimeMax);
      this.ChangeLocationLoadingWindow = DefaultDataReadUtility.Read(reader, "ChangeLocationLoadingWindow", this.ChangeLocationLoadingWindow);
      this.ReleaseStrategyIndex = DefaultDataReadUtility.Read(reader, "ReleaseStrategyIndex", this.ReleaseStrategyIndex);
      this.DevelopmentStrategyIndex = DefaultDataReadUtility.Read(reader, "DevelopmentStrategyIndex", this.DevelopmentStrategyIndex);
      this.EditorStrategyIndex = DefaultDataReadUtility.Read(reader, "EditorStrategyIndex", this.EditorStrategyIndex);
      this.ConsoleStrategyIndex = DefaultDataReadUtility.Read(reader, "ConsoleStrategyIndex", this.ConsoleStrategyIndex);
      this.MemoryStrategies = DefaultDataReadUtility.ReadListSerialize<IMemoryStrategy>(reader, "MemoryStrategies", this.MemoryStrategies);
      this.MemoryStrategyTimeEventPeriod = DefaultDataReadUtility.Read(reader, "MemoryStrategyTimeEventPeriod", this.MemoryStrategyTimeEventPeriod);
      this.useAiLods = DefaultDataReadUtility.Read(reader, "UseAiLods", this.useAiLods);
      this.AiLodsDistance = DefaultDataReadUtility.Read(reader, "AiLodsDistance", this.AiLodsDistance);
      this.AiLodsMinDistance = DefaultDataReadUtility.Read(reader, "AiLodsMinDistance", this.AiLodsMinDistance);
    }
  }
}
