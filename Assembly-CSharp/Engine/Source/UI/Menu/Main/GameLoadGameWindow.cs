using System;
using System.Collections;
using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameLoadGameWindow : CancelableSimpleWindow, IGameLoadGameWindow, IWindow, IPauseMenu
  {
    public static bool IsActive;

    public override IEnumerator OnOpened()
    {
      IsActive = true;
      return base.OnOpened();
    }

    public override IEnumerator OnClosed()
    {
      IsActive = false;
      return base.OnClosed();
    }

    protected override void RegisterLayer()
    {
      RegisterLayer<IGameLoadGameWindow>(this);
    }

    public override Type GetWindowType() => typeof (IGameLoadGameWindow);
  }
}
