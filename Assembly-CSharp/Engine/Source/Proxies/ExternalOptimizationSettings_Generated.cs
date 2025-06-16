using System;
using System.Runtime;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ExternalOptimizationSettings_Generated settingsGenerated = (ExternalOptimizationSettings_Generated) target2;
      settingsGenerated.Version = Version;
      settingsGenerated.CreateDisabledPrefabs = CreateDisabledPrefabs;
      settingsGenerated.DestroyDisabledPrefabs = DestroyDisabledPrefabs;
      settingsGenerated.ThreadCount = ThreadCount;
      settingsGenerated.MinAffinityCount = MinAffinityCount;
      settingsGenerated.Affinity = Affinity;
      settingsGenerated.LazyFsm = LazyFsm;
      settingsGenerated.SmoothHierarchyMapping = SmoothHierarchyMapping;
      settingsGenerated.DisableAudio = DisableAudio;
      settingsGenerated.DisableLipSync = DisableLipSync;
      settingsGenerated.DisableGrass = DisableGrass;
      settingsGenerated.PreloadGrass = PreloadGrass;
      settingsGenerated.DisableBlueprints = DisableBlueprints;
      settingsGenerated.BlueprintsInSeparateThread = BlueprintsInSeparateThread;
      settingsGenerated.InteractableAsyncBlueprintStart = InteractableAsyncBlueprintStart;
      settingsGenerated.DisableBehaviourTree = DisableBehaviourTree;
      settingsGenerated.PauseBehaviourTree = PauseBehaviourTree;
      settingsGenerated.DisableLeaf = DisableLeaf;
      settingsGenerated.DisableDetectors = DisableDetectors;
      settingsGenerated.ReduceUpdateFarObjects = ReduceUpdateFarObjects;
      settingsGenerated.SaveCompress = SaveCompress;
      settingsGenerated.PreloadBehaviors = PreloadBehaviors;
      settingsGenerated.PreloadBlueprints = PreloadBlueprints;
      settingsGenerated.UseCompressedTemplates = UseCompressedTemplates;
      settingsGenerated.TargetFrameRate = TargetFrameRate;
      settingsGenerated.LatencyMode = LatencyMode;
      settingsGenerated.PathfindingIterationsPerFrame = PathfindingIterationsPerFrame;
      settingsGenerated.ReduceUpdateFarObjectsDistance = ReduceUpdateFarObjectsDistance;
      settingsGenerated.UsePlayerPrefs = UsePlayerPrefs;
      settingsGenerated.DetectorUpdateDelay = DetectorUpdateDelay;
      settingsGenerated.NavigationUpdateDelay = NavigationUpdateDelay;
      settingsGenerated.IndoorCrowdUpdateDelay = IndoorCrowdUpdateDelay;
      settingsGenerated.OutdoorCrowdUpdateDelay = OutdoorCrowdUpdateDelay;
      settingsGenerated.EffectUpdateDelay = EffectUpdateDelay;
      settingsGenerated.DiseaseUpdateDelay = DiseaseUpdateDelay;
      settingsGenerated.LeafUpdateDelay = LeafUpdateDelay;
      settingsGenerated.LeafSpawnDelay = LeafSpawnDelay;
      settingsGenerated.SoundUpdateDelay = SoundUpdateDelay;
      settingsGenerated.BehaviorTreeUpdateDelay = BehaviorTreeUpdateDelay;
      settingsGenerated.BlueprintUpdateDelay = BlueprintUpdateDelay;
      settingsGenerated.WeatherUpdateDelay = WeatherUpdateDelay;
      settingsGenerated.PlayerUpdateDelay = PlayerUpdateDelay;
      settingsGenerated.EnvironmentUpdateDelay = EnvironmentUpdateDelay;
      settingsGenerated.FlockSpawnDelay = FlockSpawnDelay;
      settingsGenerated.FlockMoveDelay = FlockMoveDelay;
      settingsGenerated.FlockCastDelay = FlockCastDelay;
      settingsGenerated.PlagueZoneDelay = PlagueZoneDelay;
      settingsGenerated.BlueprintSoundsDelay = BlueprintSoundsDelay;
      settingsGenerated.BlueprintEffectsDelay = BlueprintEffectsDelay;
      settingsGenerated.LightShaftsDelay = LightShaftsDelay;
      settingsGenerated.CrowdSpawnMinDistance = CrowdSpawnMinDistance;
      settingsGenerated.CrowdSpawnMaxDistance = CrowdSpawnMaxDistance;
      settingsGenerated.CrowdDestroyDistance = CrowdDestroyDistance;
      settingsGenerated.DestroyOutdoorCrowdInIndoor = DestroyOutdoorCrowdInIndoor;
      settingsGenerated.RegionDistanceDelay = RegionDistanceDelay;
      settingsGenerated.MaxOutdoorCrowdEntityCount = MaxOutdoorCrowdEntityCount;
      CloneableObjectUtility.FillListTo(settingsGenerated.LimitOutdoorAreaCrowdEntity, LimitOutdoorAreaCrowdEntity);
      settingsGenerated.MultipleSceneLoader = MultipleSceneLoader;
      settingsGenerated.VirtualMachineDebug = VirtualMachineDebug;
      settingsGenerated.VMEventsQueuePerFrameTimeMax = VMEventsQueuePerFrameTimeMax;
      settingsGenerated.ChangeLocationLoadingWindow = ChangeLocationLoadingWindow;
      settingsGenerated.ReleaseStrategyIndex = ReleaseStrategyIndex;
      settingsGenerated.DevelopmentStrategyIndex = DevelopmentStrategyIndex;
      settingsGenerated.EditorStrategyIndex = EditorStrategyIndex;
      settingsGenerated.ConsoleStrategyIndex = ConsoleStrategyIndex;
      CloneableObjectUtility.CopyListTo(settingsGenerated.MemoryStrategies, MemoryStrategies);
      settingsGenerated.MemoryStrategyTimeEventPeriod = MemoryStrategyTimeEventPeriod;
      settingsGenerated.useAiLods = useAiLods;
      settingsGenerated.AiLodsDistance = AiLodsDistance;
      settingsGenerated.AiLodsMinDistance = AiLodsMinDistance;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Version", Version);
      DefaultDataWriteUtility.Write(writer, "CreateDisabledPrefabs", CreateDisabledPrefabs);
      DefaultDataWriteUtility.Write(writer, "DestroyDisabledPrefabs", DestroyDisabledPrefabs);
      DefaultDataWriteUtility.Write(writer, "ThreadCount", ThreadCount);
      DefaultDataWriteUtility.Write(writer, "MinAffinityCount", MinAffinityCount);
      DefaultDataWriteUtility.Write(writer, "Affinity", Affinity);
      DefaultDataWriteUtility.Write(writer, "LazyFsm", LazyFsm);
      DefaultDataWriteUtility.Write(writer, "SmoothHierarchyMapping", SmoothHierarchyMapping);
      DefaultDataWriteUtility.Write(writer, "DisableAudio", DisableAudio);
      DefaultDataWriteUtility.Write(writer, "DisableLipSync", DisableLipSync);
      DefaultDataWriteUtility.Write(writer, "DisableGrass", DisableGrass);
      DefaultDataWriteUtility.Write(writer, "PreloadGrass", PreloadGrass);
      DefaultDataWriteUtility.Write(writer, "DisableBlueprints", DisableBlueprints);
      DefaultDataWriteUtility.Write(writer, "BlueprintsInSeparateThread", BlueprintsInSeparateThread);
      DefaultDataWriteUtility.Write(writer, "InteractableAsyncBlueprintStart", InteractableAsyncBlueprintStart);
      DefaultDataWriteUtility.Write(writer, "DisableBehaviourTree", DisableBehaviourTree);
      DefaultDataWriteUtility.Write(writer, "PauseBehaviourTree", PauseBehaviourTree);
      DefaultDataWriteUtility.Write(writer, "DisableLeaf", DisableLeaf);
      DefaultDataWriteUtility.Write(writer, "DisableDetectors", DisableDetectors);
      DefaultDataWriteUtility.Write(writer, "ReduceUpdateFarObjects", ReduceUpdateFarObjects);
      DefaultDataWriteUtility.Write(writer, "SaveCompress", SaveCompress);
      DefaultDataWriteUtility.Write(writer, "PreloadBehaviors", PreloadBehaviors);
      DefaultDataWriteUtility.Write(writer, "PreloadBlueprints", PreloadBlueprints);
      DefaultDataWriteUtility.Write(writer, "UseCompressedTemplates", UseCompressedTemplates);
      DefaultDataWriteUtility.Write(writer, "TargetFrameRate", TargetFrameRate);
      DefaultDataWriteUtility.WriteEnum(writer, "LatencyMode", LatencyMode);
      DefaultDataWriteUtility.Write(writer, "PathfindingIterationsPerFrame", PathfindingIterationsPerFrame);
      DefaultDataWriteUtility.Write(writer, "ReduceUpdateFarObjectsDistance", ReduceUpdateFarObjectsDistance);
      DefaultDataWriteUtility.Write(writer, "UsePlayerPrefs", UsePlayerPrefs);
      DefaultDataWriteUtility.Write(writer, "DetectorUpdateDelay", DetectorUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "NavigationUpdateDelay", NavigationUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "IndoorCrowdUpdateDelay", IndoorCrowdUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "OutdoorCrowdUpdateDelay", OutdoorCrowdUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "EffectUpdateDelay", EffectUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "DiseaseUpdateDelay", DiseaseUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "LeafUpdateDelay", LeafUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "LeafSpawnDelay", LeafSpawnDelay);
      DefaultDataWriteUtility.Write(writer, "SoundUpdateDelay", SoundUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "BehaviorTreeUpdateDelay", BehaviorTreeUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "BlueprintUpdateDelay", BlueprintUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "WeatherUpdateDelay", WeatherUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "PlayerUpdateDelay", PlayerUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "EnvironmentUpdateDelay", EnvironmentUpdateDelay);
      DefaultDataWriteUtility.Write(writer, "FlockSpawnDelay", FlockSpawnDelay);
      DefaultDataWriteUtility.Write(writer, "FlockMoveDelay", FlockMoveDelay);
      DefaultDataWriteUtility.Write(writer, "FlockCastDelay", FlockCastDelay);
      DefaultDataWriteUtility.Write(writer, "PlagueZoneDelay", PlagueZoneDelay);
      DefaultDataWriteUtility.Write(writer, "BlueprintSoundsDelay", BlueprintSoundsDelay);
      DefaultDataWriteUtility.Write(writer, "BlueprintEffectsDelay", BlueprintEffectsDelay);
      DefaultDataWriteUtility.Write(writer, "LightShaftsDelay", LightShaftsDelay);
      DefaultDataWriteUtility.Write(writer, "CrowdSpawnMinDistance", CrowdSpawnMinDistance);
      DefaultDataWriteUtility.Write(writer, "CrowdSpawnMaxDistance", CrowdSpawnMaxDistance);
      DefaultDataWriteUtility.Write(writer, "CrowdDestroyDistance", CrowdDestroyDistance);
      DefaultDataWriteUtility.Write(writer, "DestroyOutdoorCrowdInIndoor", DestroyOutdoorCrowdInIndoor);
      DefaultDataWriteUtility.Write(writer, "RegionDistanceDelay", RegionDistanceDelay);
      DefaultDataWriteUtility.Write(writer, "MaxOutdoorCrowdEntityCount", MaxOutdoorCrowdEntityCount);
      DefaultDataWriteUtility.WriteListEnum(writer, "LimitOutdoorAreaCrowdEntity", LimitOutdoorAreaCrowdEntity);
      DefaultDataWriteUtility.Write(writer, "MultipleSceneLoader", MultipleSceneLoader);
      DefaultDataWriteUtility.Write(writer, "VirtualMachineDebug", VirtualMachineDebug);
      DefaultDataWriteUtility.Write(writer, "VMEventsQueuePerFrameTimeMax", VMEventsQueuePerFrameTimeMax);
      DefaultDataWriteUtility.Write(writer, "ChangeLocationLoadingWindow", ChangeLocationLoadingWindow);
      DefaultDataWriteUtility.Write(writer, "ReleaseStrategyIndex", ReleaseStrategyIndex);
      DefaultDataWriteUtility.Write(writer, "DevelopmentStrategyIndex", DevelopmentStrategyIndex);
      DefaultDataWriteUtility.Write(writer, "EditorStrategyIndex", EditorStrategyIndex);
      DefaultDataWriteUtility.Write(writer, "ConsoleStrategyIndex", ConsoleStrategyIndex);
      DefaultDataWriteUtility.WriteListSerialize(writer, "MemoryStrategies", MemoryStrategies);
      DefaultDataWriteUtility.Write(writer, "MemoryStrategyTimeEventPeriod", MemoryStrategyTimeEventPeriod);
      DefaultDataWriteUtility.Write(writer, "UseAiLods", useAiLods);
      DefaultDataWriteUtility.Write(writer, "AiLodsDistance", AiLodsDistance);
      DefaultDataWriteUtility.Write(writer, "AiLodsMinDistance", AiLodsMinDistance);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Version = DefaultDataReadUtility.Read(reader, "Version", Version);
      CreateDisabledPrefabs = DefaultDataReadUtility.Read(reader, "CreateDisabledPrefabs", CreateDisabledPrefabs);
      DestroyDisabledPrefabs = DefaultDataReadUtility.Read(reader, "DestroyDisabledPrefabs", DestroyDisabledPrefabs);
      ThreadCount = DefaultDataReadUtility.Read(reader, "ThreadCount", ThreadCount);
      MinAffinityCount = DefaultDataReadUtility.Read(reader, "MinAffinityCount", MinAffinityCount);
      Affinity = DefaultDataReadUtility.Read(reader, "Affinity", Affinity);
      LazyFsm = DefaultDataReadUtility.Read(reader, "LazyFsm", LazyFsm);
      SmoothHierarchyMapping = DefaultDataReadUtility.Read(reader, "SmoothHierarchyMapping", SmoothHierarchyMapping);
      DisableAudio = DefaultDataReadUtility.Read(reader, "DisableAudio", DisableAudio);
      DisableLipSync = DefaultDataReadUtility.Read(reader, "DisableLipSync", DisableLipSync);
      DisableGrass = DefaultDataReadUtility.Read(reader, "DisableGrass", DisableGrass);
      PreloadGrass = DefaultDataReadUtility.Read(reader, "PreloadGrass", PreloadGrass);
      DisableBlueprints = DefaultDataReadUtility.Read(reader, "DisableBlueprints", DisableBlueprints);
      BlueprintsInSeparateThread = DefaultDataReadUtility.Read(reader, "BlueprintsInSeparateThread", BlueprintsInSeparateThread);
      InteractableAsyncBlueprintStart = DefaultDataReadUtility.Read(reader, "InteractableAsyncBlueprintStart", InteractableAsyncBlueprintStart);
      DisableBehaviourTree = DefaultDataReadUtility.Read(reader, "DisableBehaviourTree", DisableBehaviourTree);
      PauseBehaviourTree = DefaultDataReadUtility.Read(reader, "PauseBehaviourTree", PauseBehaviourTree);
      DisableLeaf = DefaultDataReadUtility.Read(reader, "DisableLeaf", DisableLeaf);
      DisableDetectors = DefaultDataReadUtility.Read(reader, "DisableDetectors", DisableDetectors);
      ReduceUpdateFarObjects = DefaultDataReadUtility.Read(reader, "ReduceUpdateFarObjects", ReduceUpdateFarObjects);
      SaveCompress = DefaultDataReadUtility.Read(reader, "SaveCompress", SaveCompress);
      PreloadBehaviors = DefaultDataReadUtility.Read(reader, "PreloadBehaviors", PreloadBehaviors);
      PreloadBlueprints = DefaultDataReadUtility.Read(reader, "PreloadBlueprints", PreloadBlueprints);
      UseCompressedTemplates = DefaultDataReadUtility.Read(reader, "UseCompressedTemplates", UseCompressedTemplates);
      TargetFrameRate = DefaultDataReadUtility.Read(reader, "TargetFrameRate", TargetFrameRate);
      LatencyMode = DefaultDataReadUtility.ReadEnum<GCLatencyMode>(reader, "LatencyMode");
      PathfindingIterationsPerFrame = DefaultDataReadUtility.Read(reader, "PathfindingIterationsPerFrame", PathfindingIterationsPerFrame);
      ReduceUpdateFarObjectsDistance = DefaultDataReadUtility.Read(reader, "ReduceUpdateFarObjectsDistance", ReduceUpdateFarObjectsDistance);
      UsePlayerPrefs = DefaultDataReadUtility.Read(reader, "UsePlayerPrefs", UsePlayerPrefs);
      DetectorUpdateDelay = DefaultDataReadUtility.Read(reader, "DetectorUpdateDelay", DetectorUpdateDelay);
      NavigationUpdateDelay = DefaultDataReadUtility.Read(reader, "NavigationUpdateDelay", NavigationUpdateDelay);
      IndoorCrowdUpdateDelay = DefaultDataReadUtility.Read(reader, "IndoorCrowdUpdateDelay", IndoorCrowdUpdateDelay);
      OutdoorCrowdUpdateDelay = DefaultDataReadUtility.Read(reader, "OutdoorCrowdUpdateDelay", OutdoorCrowdUpdateDelay);
      EffectUpdateDelay = DefaultDataReadUtility.Read(reader, "EffectUpdateDelay", EffectUpdateDelay);
      DiseaseUpdateDelay = DefaultDataReadUtility.Read(reader, "DiseaseUpdateDelay", DiseaseUpdateDelay);
      LeafUpdateDelay = DefaultDataReadUtility.Read(reader, "LeafUpdateDelay", LeafUpdateDelay);
      LeafSpawnDelay = DefaultDataReadUtility.Read(reader, "LeafSpawnDelay", LeafSpawnDelay);
      SoundUpdateDelay = DefaultDataReadUtility.Read(reader, "SoundUpdateDelay", SoundUpdateDelay);
      BehaviorTreeUpdateDelay = DefaultDataReadUtility.Read(reader, "BehaviorTreeUpdateDelay", BehaviorTreeUpdateDelay);
      BlueprintUpdateDelay = DefaultDataReadUtility.Read(reader, "BlueprintUpdateDelay", BlueprintUpdateDelay);
      WeatherUpdateDelay = DefaultDataReadUtility.Read(reader, "WeatherUpdateDelay", WeatherUpdateDelay);
      PlayerUpdateDelay = DefaultDataReadUtility.Read(reader, "PlayerUpdateDelay", PlayerUpdateDelay);
      EnvironmentUpdateDelay = DefaultDataReadUtility.Read(reader, "EnvironmentUpdateDelay", EnvironmentUpdateDelay);
      FlockSpawnDelay = DefaultDataReadUtility.Read(reader, "FlockSpawnDelay", FlockSpawnDelay);
      FlockMoveDelay = DefaultDataReadUtility.Read(reader, "FlockMoveDelay", FlockMoveDelay);
      FlockCastDelay = DefaultDataReadUtility.Read(reader, "FlockCastDelay", FlockCastDelay);
      PlagueZoneDelay = DefaultDataReadUtility.Read(reader, "PlagueZoneDelay", PlagueZoneDelay);
      BlueprintSoundsDelay = DefaultDataReadUtility.Read(reader, "BlueprintSoundsDelay", BlueprintSoundsDelay);
      BlueprintEffectsDelay = DefaultDataReadUtility.Read(reader, "BlueprintEffectsDelay", BlueprintEffectsDelay);
      LightShaftsDelay = DefaultDataReadUtility.Read(reader, "LightShaftsDelay", LightShaftsDelay);
      CrowdSpawnMinDistance = DefaultDataReadUtility.Read(reader, "CrowdSpawnMinDistance", CrowdSpawnMinDistance);
      CrowdSpawnMaxDistance = DefaultDataReadUtility.Read(reader, "CrowdSpawnMaxDistance", CrowdSpawnMaxDistance);
      CrowdDestroyDistance = DefaultDataReadUtility.Read(reader, "CrowdDestroyDistance", CrowdDestroyDistance);
      DestroyOutdoorCrowdInIndoor = DefaultDataReadUtility.Read(reader, "DestroyOutdoorCrowdInIndoor", DestroyOutdoorCrowdInIndoor);
      RegionDistanceDelay = DefaultDataReadUtility.Read(reader, "RegionDistanceDelay", RegionDistanceDelay);
      MaxOutdoorCrowdEntityCount = DefaultDataReadUtility.Read(reader, "MaxOutdoorCrowdEntityCount", MaxOutdoorCrowdEntityCount);
      LimitOutdoorAreaCrowdEntity = DefaultDataReadUtility.ReadListEnum(reader, "LimitOutdoorAreaCrowdEntity", LimitOutdoorAreaCrowdEntity);
      MultipleSceneLoader = DefaultDataReadUtility.Read(reader, "MultipleSceneLoader", MultipleSceneLoader);
      VirtualMachineDebug = DefaultDataReadUtility.Read(reader, "VirtualMachineDebug", VirtualMachineDebug);
      VMEventsQueuePerFrameTimeMax = DefaultDataReadUtility.Read(reader, "VMEventsQueuePerFrameTimeMax", VMEventsQueuePerFrameTimeMax);
      ChangeLocationLoadingWindow = DefaultDataReadUtility.Read(reader, "ChangeLocationLoadingWindow", ChangeLocationLoadingWindow);
      ReleaseStrategyIndex = DefaultDataReadUtility.Read(reader, "ReleaseStrategyIndex", ReleaseStrategyIndex);
      DevelopmentStrategyIndex = DefaultDataReadUtility.Read(reader, "DevelopmentStrategyIndex", DevelopmentStrategyIndex);
      EditorStrategyIndex = DefaultDataReadUtility.Read(reader, "EditorStrategyIndex", EditorStrategyIndex);
      ConsoleStrategyIndex = DefaultDataReadUtility.Read(reader, "ConsoleStrategyIndex", ConsoleStrategyIndex);
      MemoryStrategies = DefaultDataReadUtility.ReadListSerialize(reader, "MemoryStrategies", MemoryStrategies);
      MemoryStrategyTimeEventPeriod = DefaultDataReadUtility.Read(reader, "MemoryStrategyTimeEventPeriod", MemoryStrategyTimeEventPeriod);
      useAiLods = DefaultDataReadUtility.Read(reader, "UseAiLods", useAiLods);
      AiLodsDistance = DefaultDataReadUtility.Read(reader, "AiLodsDistance", AiLodsDistance);
      AiLodsMinDistance = DefaultDataReadUtility.Read(reader, "AiLodsMinDistance", AiLodsMinDistance);
    }
  }
}
