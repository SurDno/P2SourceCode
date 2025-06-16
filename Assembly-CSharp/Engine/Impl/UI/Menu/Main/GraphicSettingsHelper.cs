// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Main.GraphicSettingsHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings;

#nullable disable
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
