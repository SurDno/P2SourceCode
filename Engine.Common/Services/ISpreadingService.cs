using System;
using Engine.Common.Commons;
using Engine.Common.Components.Regions;

namespace Engine.Common.Services
{
  public interface ISpreadingService
  {
    event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoadedOnce;

    event Action<IEntity, IEntity, BuildingEnum, DiseasedStateEnum> OnFurnitureLoaded;

    event Action<IEntity> OnRegionLoadedOnce;

    event Action<IEntity> OnRegionLoaded;

    void Reset();
  }
}
