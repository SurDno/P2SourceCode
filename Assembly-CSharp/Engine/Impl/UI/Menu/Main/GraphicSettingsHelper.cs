using Engine.Source.Commons;
using Engine.Source.Settings;

namespace Engine.Impl.UI.Menu.Main
{
  public static class GraphicSettingsHelper
  {
    public static void OnAutoValueChange<T>(SettingsValueView<T> view)
    {
      GraphicsGameSettings instance = InstanceByRequest<GraphicsGameSettings>.Instance;
      view.ApplyVisibleValue();
      instance.Apply();
    }
  }
}
