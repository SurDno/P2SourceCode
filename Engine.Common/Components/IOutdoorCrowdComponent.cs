using System;
using Engine.Common.Components.Crowds;

namespace Engine.Common.Components
{
  public interface IOutdoorCrowdComponent : IComponent
  {
    OutdoorCrowdLayoutEnum Layout { get; set; }

    void AddEntity(IEntity entity);

    void Reset();

    event Action<IEntity> OnCreateEntity;

    event Action<IEntity> OnDeleteEntity;
  }
}
