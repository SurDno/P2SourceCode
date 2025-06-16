using Engine.Impl.UI.Menu.Main;
using System;
using System.Collections;

namespace Engine.Source.UI.Menu.Main
{
  public class GameLoadGameWindow : CancelableSimpleWindow, IGameLoadGameWindow, IWindow, IPauseMenu
  {
    public static bool IsActive = false;

    public override IEnumerator OnOpened()
    {
      GameLoadGameWindow.IsActive = true;
      return base.OnOpened();
    }

    public override IEnumerator OnClosed()
    {
      GameLoadGameWindow.IsActive = false;
      return base.OnClosed();
    }

    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameLoadGameWindow>((IGameLoadGameWindow) this);
    }

    public override Type GetWindowType() => typeof (IGameLoadGameWindow);
  }
}
