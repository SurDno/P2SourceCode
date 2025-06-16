using System;

namespace Engine.Source.UI
{
  public interface IWindow
  {
    event Action<IWindow> DisableWindowEvent;

    bool IsEnabled { get; }
  }
}
