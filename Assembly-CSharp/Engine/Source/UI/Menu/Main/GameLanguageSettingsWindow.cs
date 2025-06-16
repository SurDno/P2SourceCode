using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameLanguageSettingsWindow : 
    CancelableSimpleWindow,
    IGameLanguageSettingsWindow,
    IWindow,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer((IGameLanguageSettingsWindow) this);
    }
  }
}
