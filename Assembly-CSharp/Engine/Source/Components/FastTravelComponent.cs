using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Inspectors;
using System;

namespace Engine.Source.Components
{
  [Factory(typeof (IFastTravelComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FastTravelComponent : EngineComponent, IFastTravelComponent, IComponent
  {
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public IParameterValue<bool> CanFastTravel { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<FastTravelPointEnum> FastTravelPointIndex { get; } = (IParameterValue<FastTravelPointEnum>) new ParameterValue<FastTravelPointEnum>();

    [Inspected]
    public IParameterValue<int> FastTravelPrice { get; } = (IParameterValue<int>) new ParameterValue<int>();

    public event Action<FastTravelPointEnum, TimeSpan> TravelToPoint;

    public override void OnAdded()
    {
      base.OnAdded();
      this.CanFastTravel.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanFastTravel));
      this.FastTravelPointIndex.Set<FastTravelPointEnum>(this.parameters.GetByName<FastTravelPointEnum>(ParameterNameEnum.FastTravelPointIndex));
      this.FastTravelPrice.Set<int>(this.parameters.GetByName<int>(ParameterNameEnum.FastTravelPrice));
    }

    public override void OnRemoved()
    {
      this.CanFastTravel.Set<bool>((IParameter<bool>) null);
      this.FastTravelPointIndex.Set<FastTravelPointEnum>((IParameter<FastTravelPointEnum>) null);
      this.FastTravelPrice.Set<int>((IParameter<int>) null);
      base.OnRemoved();
    }

    public void FireTravelToPoint(FastTravelPointEnum target, TimeSpan travelTime)
    {
      Action<FastTravelPointEnum, TimeSpan> travelToPoint = this.TravelToPoint;
      if (travelToPoint == null)
        return;
      travelToPoint(target, travelTime);
    }
  }
}
