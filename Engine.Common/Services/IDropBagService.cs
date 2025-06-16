using System;

namespace Engine.Common.Services
{
  public interface IDropBagService
  {
    void AddEntity(IEntity entity);

    void Reset();

    event Action<IEntity> OnCreateEntity;

    event Action<IEntity> OnDeleteEntity;
  }
}
