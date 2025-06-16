using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartLanguageSettingsWindow : 
    CancelableSimpleWindow,
    IStartLanguageSettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer((IStartLanguageSettingsWindow) this);
    }
  }
}
