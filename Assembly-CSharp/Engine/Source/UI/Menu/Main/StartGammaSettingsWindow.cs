using System;
using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartGammaSettingsWindow : SimpleWindow, IStartGammaSettingsWindow, IWindow, IMainMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer<IStartGammaSettingsWindow>(this);
    }

    public override Type GetWindowType() => typeof (IStartGammaSettingsWindow);
  }
}
