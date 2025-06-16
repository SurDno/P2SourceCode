using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameControlSettingsWindow : 
    CancelableSimpleWindow,
    IGameControlSettingsWindow,
    IWindow,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer<IGameControlSettingsWindow>(this);
    }
  }
}
