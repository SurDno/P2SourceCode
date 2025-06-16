using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameDifficultySettingsWindow : 
    CancelableSimpleWindow,
    IGameDifficultySettingsWindow,
    IWindow,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameDifficultySettingsWindow>((IGameDifficultySettingsWindow) this);
    }
  }
}
