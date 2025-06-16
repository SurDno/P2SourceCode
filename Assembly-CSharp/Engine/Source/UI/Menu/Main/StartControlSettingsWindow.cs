using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartControlSettingsWindow : 
    CancelableSimpleWindow,
    IStartControlSettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartControlSettingsWindow>((IStartControlSettingsWindow) this);
    }
  }
}
