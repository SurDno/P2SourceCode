using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameSoundSettingsWindow : 
    CancelableSimpleWindow,
    IGameSoundSettingsWindow,
    IWindow,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameSoundSettingsWindow>((IGameSoundSettingsWindow) this);
    }
  }
}
