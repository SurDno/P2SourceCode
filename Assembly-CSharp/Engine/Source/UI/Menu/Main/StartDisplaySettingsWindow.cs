using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartDisplaySettingsWindow : 
    CancelableSimpleWindow,
    IStartDisplaySettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer((IStartDisplaySettingsWindow) this);
    }
  }
}
