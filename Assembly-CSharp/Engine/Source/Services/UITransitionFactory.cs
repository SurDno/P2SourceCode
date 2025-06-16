// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.UITransitionFactory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Menu;
using Engine.Impl.UI.Menu.Main;
using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  public static class UITransitionFactory
  {
    public static Func<UIWindow, UIWindow, IEnumerator> GetTransition(
      UIWindow closeWindow,
      UIWindow openWindow)
    {
      return new Func<UIWindow, UIWindow, IEnumerator>(UITransitionFactory.DefaultTransition);
    }

    private static IEnumerator DefaultTransition(UIWindow closeWindow, UIWindow openWindow)
    {
      if ((UnityEngine.Object) closeWindow != (UnityEngine.Object) null)
      {
        IEnumerator closed = closeWindow.OnClosed();
        while (closed.MoveNext())
          yield return closed.Current;
        closed = (IEnumerator) null;
      }
      if ((UnityEngine.Object) openWindow != (UnityEngine.Object) null)
      {
        IEnumerator opened = openWindow.OnOpened();
        while (opened.MoveNext())
          yield return opened.Current;
        opened = (IEnumerator) null;
      }
    }

    private static IEnumerator OpenGameWindow(UIWindow closeWindow, UIWindow openWindow)
    {
      if ((UnityEngine.Object) closeWindow != (UnityEngine.Object) null)
      {
        IEnumerator closed = closeWindow.OnClosed();
        while (closed.MoveNext())
          yield return closed.Current;
        closed = (IEnumerator) null;
      }
      GameWindow gameWindow = openWindow as GameWindow;
      if (!((UnityEngine.Object) gameWindow == (UnityEngine.Object) null))
      {
        gameWindow.IsEnabled = true;
        gameWindow.Menu.transform.localScale = Vector3.zero;
        float scale = 0.0f;
        float speed = 3f;
        while (true)
        {
          scale += Time.deltaTime * speed;
          if ((double) scale < 1.0)
          {
            gameWindow.Menu.transform.localScale = Vector3.one * EasingFunction.EaseOutElastic(0.0f, 1f, scale);
            yield return (object) null;
          }
          else
            break;
        }
        gameWindow.Menu.transform.localScale = Vector3.one;
      }
    }
  }
}
