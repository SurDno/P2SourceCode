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
        if (settings == null)
          settings = !ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.UsePlayerPrefs ? new PlayerFileSettings() : new PlayerPrefsSettings();
        return settings;
      }
    }
  }
}
