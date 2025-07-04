﻿using System;
using Engine.Common.Components.Parameters;

namespace Engine.Common.Components.Regions
{
  public interface IFastTravelComponent : IComponent
  {
    event Action<FastTravelPointEnum, TimeSpan> TravelToPoint;

    IParameterValue<bool> CanFastTravel { get; }

    IParameterValue<FastTravelPointEnum> FastTravelPointIndex { get; }

    IParameterValue<int> FastTravelPrice { get; }
  }
}
