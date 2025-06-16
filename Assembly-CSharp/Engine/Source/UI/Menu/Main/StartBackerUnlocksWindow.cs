using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartBackerUnlocksWindow : 
    CancelableSimpleWindow,
    IStartBackerUnlocksWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer<IStartBackerUnlocksWindow>(this);
    }
  }
}
