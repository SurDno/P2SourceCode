// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Regions.IFastTravelComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Parameters;
using System;

#nullable disable
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
