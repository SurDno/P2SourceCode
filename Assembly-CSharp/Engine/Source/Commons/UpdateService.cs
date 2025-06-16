using Engine.Source.Settings.External;
using Inspectors;
using SRDebugger.Services;
using SRF.Service;
using System;
using System.Diagnostics;

namespace Engine.Source.Commons
{
  public class UpdateService : InstanceByRequest<UpdateService>
  {
    private IProfilerService service;
    private Stopwatch measure = new Stopwatch();

    [Inspected]
    public IUpdater Updater { get; private set; } = (IUpdater) new Engine.Source.Commons.Updater();

    [Inspected]
    public IUpdater CameraUpdater { get; private set; } = (IUpdater) new Engine.Source.Commons.Updater();

    [Inspected]
    public IUpdater DetectorUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DetectorUpdateDelay);

    [Inspected]
    public IUpdater NavigationUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.NavigationUpdateDelay);

    [Inspected]
    public IUpdater IndoorCrowdUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.IndoorCrowdUpdateDelay);

    [Inspected]
    public IUpdater OutdoorCrowdUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.OutdoorCrowdUpdateDelay);

    [Inspected]
    public IUpdater EffectUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.EffectUpdateDelay);

    [Inspected]
    public IUpdater DiseaseUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DiseaseUpdateDelay);

    [Inspected]
    public IUpdater LeafUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LeafUpdateDelay);

    [Inspected]
    public IUpdater LeafSpawner { get; private set; } = (IUpdater) new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LeafSpawnDelay);

    [Inspected]
    public IUpdater WeatherUpdater { get; private set; } = (IUpdater) new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.WeatherUpdateDelay);

    [Inspected]
    public IUpdater PlayerUpdater { get; private set; } = (IUpdater) new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PlayerUpdateDelay);

    [Inspected]
    public IUpdater EnvironmentUpdater { get; private set; } = (IUpdater) new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.EnvironmentUpdateDelay);

    [Inspected]
    public IUpdater FlockSpawnUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.FlockSpawnDelay);

    [Inspected]
    public IUpdater FlockMoveUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.FlockMoveDelay);

    [Inspected]
    public IUpdater FlockCastUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.FlockCastDelay);

    [Inspected]
    public IUpdater PlagueZoneUpdater { get; private set; } = (IUpdater) new DelayUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PlagueZoneDelay);

    [Inspected]
    public IUpdater VirtualMachineUpdater { get; private set; } = (IUpdater) new Engine.Source.Commons.Updater();

    [Inspected]
    public IUpdater BlueprintUpdater { get; private set; } = (IUpdater) new Engine.Source.Commons.Updater();

    [Inspected]
    public IUpdater BlueprintSoundsUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintSoundsDelay);

    [Inspected]
    public IUpdater BlueprintEffectsUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintEffectsDelay);

    [Inspected]
    public IUpdater ParametersUpdater { get; private set; } = (IUpdater) new Engine.Source.Commons.Updater();

    [Inspected]
    public IUpdater LightShaftsUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LightShaftsDelay);

    [Inspected]
    public IUpdater RegionDistanceUpdater { get; private set; } = (IUpdater) new SmoothUpdater(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.RegionDistanceDelay);

    public void Start()
    {
      this.measure.Start();
      this.service = SRServiceManager.GetService<IProfilerService>();
    }

    public void Update()
    {
      TimeSpan elapsed = this.measure.Elapsed;
      this.Updater.ComputeUpdate();
      this.DetectorUpdater.ComputeUpdate();
      this.NavigationUpdater.ComputeUpdate();
      this.IndoorCrowdUpdater.ComputeUpdate();
      this.OutdoorCrowdUpdater.ComputeUpdate();
      this.EffectUpdater.ComputeUpdate();
      this.DiseaseUpdater.ComputeUpdate();
      this.LeafUpdater.ComputeUpdate();
      this.LeafSpawner.ComputeUpdate();
      this.WeatherUpdater.ComputeUpdate();
      this.PlayerUpdater.ComputeUpdate();
      this.EnvironmentUpdater.ComputeUpdate();
      this.FlockSpawnUpdater.ComputeUpdate();
      this.FlockMoveUpdater.ComputeUpdate();
      this.FlockCastUpdater.ComputeUpdate();
      this.PlagueZoneUpdater.ComputeUpdate();
      this.VirtualMachineUpdater.ComputeUpdate();
      this.BlueprintUpdater.ComputeUpdate();
      this.BlueprintSoundsUpdater.ComputeUpdate();
      this.BlueprintEffectsUpdater.ComputeUpdate();
      this.LightShaftsUpdater.ComputeUpdate();
      this.RegionDistanceUpdater.ComputeUpdate();
      TimeSpan timeSpan = this.measure.Elapsed - elapsed;
      if (this.service == null)
        return;
      this.service.SetCustom(timeSpan.TotalSeconds);
    }

    public void LateUpdate()
    {
      this.CameraUpdater.ComputeUpdate();
      this.ParametersUpdater.ComputeUpdate();
    }
  }
}
