using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameKeySettingsWindow : SimpleWindow, IGameKeySettingsWindow, IWindow, IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameKeySettingsWindow>((IGameKeySettingsWindow) this);
    }
  }
}
