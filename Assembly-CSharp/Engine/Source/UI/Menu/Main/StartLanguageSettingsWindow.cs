// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.StartLanguageSettingsWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Menu.Main;

#nullable disable
namespace Engine.Source.UI.Menu.Main
{
  public class StartLanguageSettingsWindow : 
    CancelableSimpleWindow,
    IStartLanguageSettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      this.RegisterLayer<IStartLanguageSettingsWindow>((IStartLanguageSettingsWindow) this);
    }
  }
}
