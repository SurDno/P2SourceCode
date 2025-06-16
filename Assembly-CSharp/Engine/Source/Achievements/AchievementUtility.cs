using System.IO;

namespace Engine.Source.Achievements
{
  public static class AchievementUtility
  {
    public static uint GetSteamAppId()
    {
      string path = "steam_appid.txt";
      uint result;
      return File.Exists(path) && uint.TryParse(File.ReadAllText(path), out result) ? result : 0U;
    }

    public static bool IsSteamAvailable => AchievementUtility.GetSteamAppId() > 0U;
  }
}
