using System;

namespace Engine.Common.Components
{
  public interface IHerbRootsComponent : IComponent
  {
    void Reset();

    event Action OnTriggerEnterEvent;

    event Action OnTriggerLeaveEvent;

    event Action OnActivateStartEvent;

    event Action OnActivateEndEvent;

    event Action OnHerbSpawnEvent;

    event Action OnLastHerbSpawnEvent;
  }
}
