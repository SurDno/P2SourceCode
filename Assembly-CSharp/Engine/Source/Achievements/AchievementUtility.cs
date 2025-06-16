// Decompiled with JetBrains decompiler
// Type: Engine.Source.Achievements.AchievementUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.IO;

#nullable disable
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
