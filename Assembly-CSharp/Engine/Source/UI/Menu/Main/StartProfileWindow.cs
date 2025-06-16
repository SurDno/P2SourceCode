using Engine.Impl.UI.Menu.Main;
using System;

namespace Engine.Source.UI.Menu.Main
{
  public class StartProfileWindow : 
    CancelableSimpleWindow,
    IStartProfileWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartProfileWindow>((IStartProfileWindow) this);
    }

    public override Type GetWindowType() => typeof (IStartProfileWindow);
  }
}
