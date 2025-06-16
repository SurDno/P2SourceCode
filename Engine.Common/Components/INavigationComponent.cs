using System;
using Engine.Common.Components.Movable;

namespace Engine.Common.Components
{
  public interface INavigationComponent : IComponent
  {
    IRegionComponent Region { get; }

    IBuildingComponent Building { get; }

    AreaEnum Area { get; }

    bool HasPrevTeleport { get; }

    event RegionHandler EnterRegionEvent;

    event RegionHandler ExitRegionEvent;

    event BuildingHandler EnterBuildingEvent;

    event BuildingHandler ExitBuildingEvent;

    event AreaHandler EnterAreaEvent;

    event AreaHandler ExitAreaEvent;

    event Action<INavigationComponent, IEntity> OnTeleport;

    void TeleportTo(IEntity target);
  }
}
