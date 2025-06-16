using Engine.Common.Commons;
using System;

namespace Engine.Common.Components
{
  public interface IBlueprintComponent : IComponent
  {
    event Action<IBlueprintComponent> CompleteEvent;

    event Action<IBlueprintComponent> AttachEvent;

    IBlueprintObject Blueprint { get; set; }

    bool IsStarted { get; }

    bool IsAttached { get; }

    void Start();

    void Start(IEntity owner);

    void Stop();

    void SendEvent(string name);
  }
}
