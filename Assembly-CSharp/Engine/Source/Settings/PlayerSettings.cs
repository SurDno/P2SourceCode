using Engine.Source.Settings.External;

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
