using System;
using System.Collections;
using Engine.Impl.UI.Menu;
using Engine.Impl.UI.Menu.Main;
using UnityEngine;

namespace Engine.Source.Services
{
  public static class UITransitionFactory
  {
    public static Func<UIWindow, UIWindow, IEnumerator> GetTransition(
      UIWindow closeWindow,
      UIWindow openWindow)
    {
      return DefaultTransition;
    }

    private static IEnumerator DefaultTransition(UIWindow closeWindow, UIWindow openWindow)
    {
      if (closeWindow != null)
      {
        IEnumerator closed = closeWindow.OnClosed();
        while (closed.MoveNext())
          yield return closed.Current;
        closed = null;
      }
      if (openWindow != null)
      {
        IEnumerator opened = openWindow.OnOpened();
        while (opened.MoveNext())
          yield return opened.Current;
        opened = null;
      }
    }

    private static IEnumerator OpenGameWindow(UIWindow closeWindow, UIWindow openWindow)
    {
      if (closeWindow != null)
      {
        IEnumerator closed = closeWindow.OnClosed();
        while (closed.MoveNext())
          yield return closed.Current;
        closed = null;
      }
      GameWindow gameWindow = openWindow as GameWindow;
      if (!(gameWindow == null))
      {
        gameWindow.IsEnabled = true;
        gameWindow.Menu.transform.localScale = Vector3.zero;
        float scale = 0.0f;
        float speed = 3f;
        while (true)
        {
          scale += Time.deltaTime * speed;
          if (scale < 1.0)
          {
            gameWindow.Menu.transform.localScale = Vector3.one * EasingFunction.EaseOutElastic(0.0f, 1f, scale);
            yield return null;
          }
          else
            break;
        }
        gameWindow.Menu.transform.localScale = Vector3.one;
      }
    }
  }
}
