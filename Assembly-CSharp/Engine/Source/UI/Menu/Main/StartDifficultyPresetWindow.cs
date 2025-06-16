using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartDifficultyPresetWindow : 
    SimpleWindow,
    IStartDifficultyPresetWindow,
    IWindow,
    IMainMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer<IStartDifficultyPresetWindow>(this);
    }
  }
}
