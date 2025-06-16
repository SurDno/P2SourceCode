// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Profiles.ProfilesUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Profiles
{
  public static class ProfilesUtility
  {
    public static string ConvertCreationTime(DateTime dateTime, string formatTag)
    {
      return ServiceLocator.GetService<LocalizationService>().GetText(formatTag).Replace("<day>", dateTime.Day.ToString().PadLeft(2, '0')).Replace("<month>", dateTime.Month.ToString().PadLeft(2, '0')).Replace("<year>", dateTime.Year.ToString().PadLeft(4, '0')).Replace("<hour>", dateTime.Hour.ToString().PadLeft(2, '0')).Replace("<minute>", dateTime.Minute.ToString().PadLeft(2, '0')).Replace("<second>", dateTime.Second.ToString().PadLeft(2, '0'));
    }

    private static string ConvertProfileName(string profileName)
    {
      return profileName.Replace("Profile", "Haruspex");
    }

    public static string GetGameName(string profileName)
    {
      return ProfilesUtility.ConvertProfileName(profileName).Split(' ')[0];
    }

    public static string ConvertProfileName(string profile, string formatTag)
    {
      string str = ServiceLocator.GetService<LocalizationService>().GetText(formatTag);
      string[] strArray = ProfilesUtility.ConvertProfileName(profile).Split(' ');
      if (strArray.Length >= 2)
        str = str.Replace("<gamename>", strArray[0]).Replace("<index>", strArray[1]);
      int saveCount = ProfilesUtility.GetSaveCount(profile);
      return str.Replace("<saves>", saveCount.ToString());
    }

    public static string ConvertSaveName(string saveName)
    {
      string text = ServiceLocator.GetService<LocalizationService>().GetText("{SaveNameFormat}");
      return ProfilesUtility.ConvertSaveName(saveName, text);
    }

    public static string ConvertSaveName(string saveName, string format)
    {
      SaveInfo saveInfo = SaveInfo.GetSaveInfo(saveName);
      return saveInfo == null ? saveName : format.Replace("<day>", saveInfo.Days.ToString()).Replace("<hours>", saveInfo.Hours.ToString()).Replace("<minutes>", saveInfo.Minutes.ToString().PadLeft(2, '0')).Replace("<building>", "{Building." + (object) saveInfo.Building + "}");
    }

    public static string GenerateSaveName()
    {
      ITimeService service1 = ServiceLocator.GetService<ITimeService>();
      ServiceLocator.GetService<GameLauncher>();
      ISimulation service2 = ServiceLocator.GetService<ISimulation>();
      TimeSpan gameTime = service1.GameTime;
      IBuildingComponent building1 = service2?.Player?.GetComponent<INavigationComponent>()?.Building;
      BuildingEnum building2 = building1 != null ? building1.Building : BuildingEnum.None;
      return SaveInfo.GetFileName(gameTime.Days, gameTime.Hours, gameTime.Minutes, building2);
    }

    public static int GetGameDay(string path)
    {
      SaveInfo saveInfo = SaveInfo.GetSaveInfo(path);
      return saveInfo == null ? -1 : saveInfo.Days;
    }

    public static string GetLastSave(string profile)
    {
      foreach (ProfileData profile1 in ServiceLocator.GetService<ProfilesService>().Profiles)
      {
        if (profile1.Name == profile)
          return profile1.LastSave;
      }
      return "";
    }

    public static List<string> GetSaveNames(string profile)
    {
      List<string> saveNames = new List<string>();
      string path = ProfilesUtility.ProfilePath(profile);
      if (!Directory.Exists(path))
        return saveNames;
      foreach (string directory in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
      {
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);
        if (directoryInfo.GetFiles().Length != 0)
        {
          string name = directoryInfo.Name;
          saveNames.Add(name);
        }
      }
      saveNames.Sort((Comparison<string>) ((a, b) =>
      {
        DateTime saveCreationTime = ProfilesUtility.GetSaveCreationTime(profile, a);
        return ProfilesUtility.GetSaveCreationTime(profile, b).CompareTo(saveCreationTime);
      }));
      return saveNames;
    }

    public static int GetSaveCount(string name) => ProfilesUtility.GetSaveNames(name).Count;

    public static bool IsSaveExist(string path) => Directory.Exists(path);

    public static string ProfilePath(string profile)
    {
      return "{DataPath}/Saves/".Replace("{DataPath}", Application.persistentDataPath) + profile;
    }

    public static string SavePath(string profile, string save)
    {
      return ProfilesUtility.ProfilePath(profile) + "/" + save;
    }

    public static DateTime GetSaveCreationTime(string profile, string save)
    {
      SaveInfo saveInfo = SaveInfo.GetSaveInfo(save);
      if (saveInfo != null && saveInfo.SaveDateTime != DateTime.MinValue)
        return saveInfo.SaveDateTime;
      string path = ProfilesUtility.SavePath(profile, save);
      return ProfilesUtility.IsSaveExist(path) ? File.GetCreationTime(path) : DateTime.MinValue;
    }
  }
}
