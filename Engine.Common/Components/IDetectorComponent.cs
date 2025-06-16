using System;
using System.Collections.Generic;

namespace Engine.Common.Components
{
  public interface IDetectorComponent : IComponent
  {
    bool IsEnabled { get; set; }

    HashSet<IDetectableComponent> Visible { get; }

    HashSet<IDetectableComponent> Hearing { get; }

    event Action<IDetectableComponent> OnSee;

    event Action<IDetectableComponent> OnStopSee;

    event Action<IDetectableComponent> OnHear;
  }
}
