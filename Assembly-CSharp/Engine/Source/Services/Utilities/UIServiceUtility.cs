using System;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using FlowCanvas;

namespace Engine.Source.Services.Utilities
{
  public static class UIServiceUtility
  {
    public static void PushWindow<T>(FlowOutput output, Action<T> initialise = null) where T : class, IWindow
    {
      UIService service = ServiceLocator.GetService<UIService>();
      T target = service.Get<T>();
      if (initialise != null)
        initialise(target);
      Action<IWindow> handler = null;
      handler = window =>
      {
        target.DisableWindowEvent -= handler;
        output.Call();
      };
      target.DisableWindowEvent += handler;
      service.Push<T>();
    }
  }
}
