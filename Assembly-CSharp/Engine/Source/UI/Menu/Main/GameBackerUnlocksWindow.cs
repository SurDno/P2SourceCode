using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class GameBackerUnlocksWindow : 
    CancelableSimpleWindow,
    IGameBackerUnlocksWindow,
    IWindow,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IGameBackerUnlocksWindow>((IGameBackerUnlocksWindow) this);
    }
  }
}
