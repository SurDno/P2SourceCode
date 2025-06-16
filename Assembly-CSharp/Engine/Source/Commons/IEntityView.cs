using System;

namespace Engine.Source.Commons
{
  public interface IEntityView
  {
    GameObject GameObject { get; }

    bool IsAttached { get; }

    Vector3 Position { get; set; }

    Quaternion Rotation { get; set; }

    event Action OnGameObjectChangedEvent;
  }
}
