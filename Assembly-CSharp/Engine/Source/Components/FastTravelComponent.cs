using System;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Inspectors;

namespace Engine.Source.Components
{
  [Factory(typeof (IFastTravelComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FastTravelComponent : EngineComponent, IFastTravelComponent, IComponent
  {
    [FromThis]
    private ParametersComponent parameters;

    [Inspected]
    public IParameterValue<bool> CanFastTravel { get; } = new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<FastTravelPointEnum> FastTravelPointIndex { get; } = new ParameterValue<FastTravelPointEnum>();

    [Inspected]
    public IParameterValue<int> FastTravelPrice { get; } = new ParameterValue<int>();

    public event Action<FastTravelPointEnum, TimeSpan> TravelToPoint;

    public override void OnAdded()
    {
      base.OnAdded();
      CanFastTravel.Set(parameters.GetByName<bool>(ParameterNameEnum.CanFastTravel));
      FastTravelPointIndex.Set(parameters.GetByName<FastTravelPointEnum>(ParameterNameEnum.FastTravelPointIndex));
      FastTravelPrice.Set(parameters.GetByName<int>(ParameterNameEnum.FastTravelPrice));
    }

    public override void OnRemoved()
    {
      CanFastTravel.Set(null);
      FastTravelPointIndex.Set(null);
      FastTravelPrice.Set(null);
      base.OnRemoved();
    }

    public void FireTravelToPoint(FastTravelPointEnum target, TimeSpan travelTime)
    {
      Action<FastTravelPointEnum, TimeSpan> travelToPoint = TravelToPoint;
      if (travelToPoint == null)
        return;
      travelToPoint(target, travelTime);
    }
  }
}
