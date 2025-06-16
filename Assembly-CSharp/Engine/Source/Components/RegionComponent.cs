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
using System;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IRegionComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RegionComponent : EngineComponent, IRegionComponent, IComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected RegionEnum region;
    [DataReadProxy(MemberEnum.None, Name = "RegionBehavior")]
    [DataWriteProxy(MemberEnum.None, Name = "RegionBehavior")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected RegionBehaviourEnum regionBehaviour;
    [FromThis]
    private ParametersComponent parameters;
    [FromLocator]
    private SpreadingService spreadingService;
    private RegionMesh regionMesh;

    [Inspected]
    public IParameterValue<int> DiseaseLevel { get; } = (IParameterValue<int>) new ParameterValue<int>();

    [Inspected]
    public IParameterValue<float> Reputation { get; } = (IParameterValue<float>) new ParameterValue<float>();

    public RegionMesh RegionMesh => this.regionMesh;

    public RegionEnum Region => this.region;

    public RegionBehaviourEnum RegionBehaviour => this.regionBehaviour;

    private void UpdateReputationValue(float newValue)
    {
      switch (this.RegionBehaviour)
      {
        case RegionBehaviourEnum.AlwaysMaxReputation:
          this.Reputation.Value = 1f;
          break;
        case RegionBehaviourEnum.AlwaysMinReputation:
          this.Reputation.Value = 0.0f;
          break;
      }
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.spreadingService.AddRegion((IRegionComponent) this);
      RegionUtility.AddRegion(this.region, this);
      this.DiseaseLevel.Set<int>(this.parameters.GetByName<int>(ParameterNameEnum.DiseaseLevel));
      this.Reputation.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Reputation));
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      this.OnGameObjectChangedEvent();
      this.UpdateReputationValue(0.0f);
      this.Reputation.ChangeValueEvent += new Action<float>(this.UpdateReputationValue);
    }

    public override void OnRemoved()
    {
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
      this.regionMesh = (RegionMesh) null;
      this.DiseaseLevel.Set<int>((IParameter<int>) null);
      this.Reputation.Set<float>((IParameter<float>) null);
      RegionUtility.RemoveRegion(this.region, this);
      this.spreadingService.RemoveRegion((IRegionComponent) this);
      this.Reputation.ChangeValueEvent -= new Action<float>(this.UpdateReputationValue);
      base.OnRemoved();
    }

    private void OnGameObjectChangedEvent()
    {
      if (((IEntityView) this.Owner).IsAttached)
        this.regionMesh = RegionLocator.GetRegionMesh(this.region);
      else
        this.regionMesh = (RegionMesh) null;
    }
  }
}
