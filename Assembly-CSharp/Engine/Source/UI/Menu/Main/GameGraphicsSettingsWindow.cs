using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameGraphicsSettingsWindow : 
    CancelableSimpleWindow,
    IGameGraphicsSettingsWindow,
    IWindow,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameGraphicsSettingsWindow>((IGameGraphicsSettingsWindow) this);
    }
  }
}
