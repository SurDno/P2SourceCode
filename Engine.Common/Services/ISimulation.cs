using System;

namespace Engine.Common.Services
{
  public interface ISimulation
  {
    IEntity Hierarchy { get; }

    IEntity Objects { get; }

    IEntity Storables { get; }

    IEntity Others { get; }

    IEntity Player { get; }

    IEntity Get(Guid id);

    void Add(IEntity entity, IEntity parent);
  }
}
