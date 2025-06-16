using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Regions;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IRegionComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RegionComponent : EngineComponent, IRegionComponent, IComponent
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected RegionEnum region;
    [DataReadProxy(Name = "RegionBehavior")]
    [DataWriteProxy(Name = "RegionBehavior")]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected RegionBehaviourEnum regionBehaviour;
    [FromThis]
    private ParametersComponent parameters;
    [FromLocator]
    private SpreadingService spreadingService;
    private RegionMesh regionMesh;

    [Inspected]
    public IParameterValue<int> DiseaseLevel { get; } = new ParameterValue<int>();

    [Inspected]
    public IParameterValue<float> Reputation { get; } = new ParameterValue<float>();

    public RegionMesh RegionMesh => regionMesh;

    public RegionEnum Region => region;

    public RegionBehaviourEnum RegionBehaviour => regionBehaviour;

    private void UpdateReputationValue(float newValue)
    {
      switch (RegionBehaviour)
      {
        case RegionBehaviourEnum.AlwaysMaxReputation:
          Reputation.Value = 1f;
          break;
        case RegionBehaviourEnum.AlwaysMinReputation:
          Reputation.Value = 0.0f;
          break;
      }
    }

    public override void OnAdded()
    {
      base.OnAdded();
      spreadingService.AddRegion(this);
      RegionUtility.AddRegion(region, this);
      DiseaseLevel.Set(parameters.GetByName<int>(ParameterNameEnum.DiseaseLevel));
      Reputation.Set(parameters.GetByName<float>(ParameterNameEnum.Reputation));
      ((IEntityView) Owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
      OnGameObjectChangedEvent();
      UpdateReputationValue(0.0f);
      Reputation.ChangeValueEvent += UpdateReputationValue;
    }

    public override void OnRemoved()
    {
      ((IEntityView) Owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
      regionMesh = null;
      DiseaseLevel.Set(null);
      Reputation.Set(null);
      RegionUtility.RemoveRegion(region, this);
      spreadingService.RemoveRegion(this);
      Reputation.ChangeValueEvent -= UpdateReputationValue;
      base.OnRemoved();
    }

    private void OnGameObjectChangedEvent()
    {
      if (((IEntityView) Owner).IsAttached)
        regionMesh = RegionLocator.GetRegionMesh(region);
      else
        regionMesh = null;
    }
  }
}
