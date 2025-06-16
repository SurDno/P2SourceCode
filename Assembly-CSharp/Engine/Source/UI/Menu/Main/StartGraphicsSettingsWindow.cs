using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartGraphicsSettingsWindow : 
    CancelableSimpleWindow,
    IStartGraphicsSettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartGraphicsSettingsWindow>((IStartGraphicsSettingsWindow) this);
    }
  }
}
