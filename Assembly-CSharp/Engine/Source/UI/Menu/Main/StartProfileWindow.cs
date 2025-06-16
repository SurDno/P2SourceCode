using System;
using Engine.Impl.UI.Menu.Main;

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
      RegisterLayer<IStartProfileWindow>(this);
    }

    public override Type GetWindowType() => typeof (IStartProfileWindow);
  }
}
