using System.IO;

namespace Engine.Source.Achievements
{
  public static class AchievementUtility
  {
    public static uint GetSteamAppId()
    {
      string path = "steam_appid.txt";
      return File.Exists(path) && uint.TryParse(File.ReadAllText(path), out uint result) ? result : 0U;
    }

    public static bool IsSteamAvailable => GetSteamAppId() > 0U;
  }
}
