using System;

namespace Engine.Common.Components
{
  public interface ILocationItemComponent : IComponent
  {
    bool IsHibernation { get; }

    ILocationComponent LogicLocation { get; }

    bool IsIndoor { get; }

    event Action<ILocationItemComponent> OnHibernationChanged;

    event Action<ILocationItemComponent, ILocationComponent> OnChangeLocation;
  }
}
