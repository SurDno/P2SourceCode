using Engine.Impl.UI.Menu.Main;
using System;

namespace Engine.Source.UI.Menu.Main
{
  public class StartGammaSettingsWindow : SimpleWindow, IStartGammaSettingsWindow, IWindow, IMainMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartGammaSettingsWindow>((IStartGammaSettingsWindow) this);
    }

    public override Type GetWindowType() => typeof (IStartGammaSettingsWindow);
  }
}
