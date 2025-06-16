using System;
using System.Diagnostics;
using Engine.Source.Settings.External;
using Inspectors;
using SRDebugger.Services;
using SRF.Service;

namespace Engine.Source.Commons
{
  public class UpdateService : InstanceByRequest<UpdateService>
  {
    private IProfilerService service;
    private Stopwatch measure = new Stopwatch();

    [Inspected]
    public IUpdater Updater { get; private set; } = new Updater();

    [Inspected]
    public IUpdater CameraUpdater { get; private set; } = new Updater();

    [Inspected]
    public IUpdater DetectorUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DetectorUpdateDelay);

    [Inspected]
    public IUpdater NavigationUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.NavigationUpdateDelay);

    [Inspected]
    public IUpdater IndoorCrowdUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.IndoorCrowdUpdateDelay);

    [Inspected]
    public IUpdater OutdoorCrowdUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.OutdoorCrowdUpdateDelay);

    [Inspected]
    public IUpdater EffectUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.EffectUpdateDelay);

    [Inspected]
    public IUpdater DiseaseUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DiseaseUpdateDelay);

    [Inspected]
    public IUpdater LeafUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LeafUpdateDelay);

    [Inspected]
    public IUpdater LeafSpawner { get; private set; } = new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LeafSpawnDelay);

    [Inspected]
    public IUpdater WeatherUpdater { get; private set; } = new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.WeatherUpdateDelay);

    [Inspected]
    public IUpdater PlayerUpdater { get; private set; } = new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PlayerUpdateDelay);

    [Inspected]
    public IUpdater EnvironmentUpdater { get; private set; } = new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.EnvironmentUpdateDelay);

    [Inspected]
    public IUpdater FlockSpawnUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.FlockSpawnDelay);

    [Inspected]
    public IUpdater FlockMoveUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.FlockMoveDelay);

    [Inspected]
    public IUpdater FlockCastUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.FlockCastDelay);

    [Inspected]
    public IUpdater PlagueZoneUpdater { get; private set; } = new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PlagueZoneDelay);

    [Inspected]
    public IUpdater VirtualMachineUpdater { get; private set; } = new Updater();

    [Inspected]
    public IUpdater BlueprintUpdater { get; private set; } = new Updater();

    [Inspected]
    public IUpdater BlueprintSoundsUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintSoundsDelay);

    [Inspected]
    public IUpdater BlueprintEffectsUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintEffectsDelay);

    [Inspected]
    public IUpdater ParametersUpdater { get; private set; } = new Updater();

    [Inspected]
    public IUpdater LightShaftsUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LightShaftsDelay);

    [Inspected]
    public IUpdater RegionDistanceUpdater { get; private set; } = new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.RegionDistanceDelay);

    public void Start()
    {
      measure.Start();
      service = SRServiceManager.GetService<IProfilerService>();
    }

    public void Update()
    {
      TimeSpan elapsed = measure.Elapsed;
      Updater.ComputeUpdate();
      DetectorUpdater.ComputeUpdate();
      NavigationUpdater.ComputeUpdate();
      IndoorCrowdUpdater.ComputeUpdate();
      OutdoorCrowdUpdater.ComputeUpdate();
      EffectUpdater.ComputeUpdate();
      DiseaseUpdater.ComputeUpdate();
      LeafUpdater.ComputeUpdate();
      LeafSpawner.ComputeUpdate();
      WeatherUpdater.ComputeUpdate();
      PlayerUpdater.ComputeUpdate();
      EnvironmentUpdater.ComputeUpdate();
      FlockSpawnUpdater.ComputeUpdate();
      FlockMoveUpdater.ComputeUpdate();
      FlockCastUpdater.ComputeUpdate();
      PlagueZoneUpdater.ComputeUpdate();
      VirtualMachineUpdater.ComputeUpdate();
      BlueprintUpdater.ComputeUpdate();
      BlueprintSoundsUpdater.ComputeUpdate();
      BlueprintEffectsUpdater.ComputeUpdate();
      LightShaftsUpdater.ComputeUpdate();
      RegionDistanceUpdater.ComputeUpdate();
      TimeSpan timeSpan = measure.Elapsed - elapsed;
      if (service == null)
        return;
      service.SetCustom(timeSpan.TotalSeconds);
    }

    public void LateUpdate()
    {
      CameraUpdater.ComputeUpdate();
      ParametersUpdater.ComputeUpdate();
    }
  }
}
