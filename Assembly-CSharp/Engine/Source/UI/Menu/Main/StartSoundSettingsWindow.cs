using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartSoundSettingsWindow : 
    CancelableSimpleWindow,
    IStartSoundSettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartSoundSettingsWindow>((IStartSoundSettingsWindow) this);
    }
  }
}
