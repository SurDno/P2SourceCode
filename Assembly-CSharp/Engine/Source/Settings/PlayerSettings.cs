// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.PlayerSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings.External;

#nullable disable
namespace Engine.Source.Settings
{
  public static class PlayerSettings
  {
    private static IPlayerSettings settings;

    public static IPlayerSettings Instance
    {
      get
      {
        if (PlayerSettings.settings == null)
          PlayerSettings.settings = !ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.UsePlayerPrefs ? (IPlayerSettings) new PlayerFileSettings() : (IPlayerSettings) new PlayerPrefsSettings();
        return PlayerSettings.settings;
      }
    }
  }
}
