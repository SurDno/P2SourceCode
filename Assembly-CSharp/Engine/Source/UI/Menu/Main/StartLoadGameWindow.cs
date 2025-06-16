using Engine.Impl.UI.Menu.Main;
using System;

namespace Engine.Source.UI.Menu.Main
{
  public class StartLoadGameWindow : 
    CancelableSimpleWindow,
    IStartLoadGameWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartLoadGameWindow>((IStartLoadGameWindow) this);
    }

    public override Type GetWindowType() => typeof (IStartLoadGameWindow);
  }
}
