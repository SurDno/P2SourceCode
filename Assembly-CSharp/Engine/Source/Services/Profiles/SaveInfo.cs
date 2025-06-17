using System;
using System.Globalization;
using System.IO;
using Cofe.Serializations.Converters;
using Engine.Common.Components.Regions;
using UnityEngine;

namespace Engine.Source.Services.Profiles
{
  public class SaveInfo
  {
    private const char separator = '-';
    private static readonly char[] separators = [
      '-'
    ];
    private static readonly char[] fileSeparators = [
      '/',
      '\\'
    ];
    private const string timeMask = "yyyy_MM_dd_HH_mm_ss";
    public int Days;
    public int Hours;
    public int Minutes;
    public BuildingEnum Building;
    public string Id;
    public DateTime SaveDateTime;

    public static SaveInfo GetSaveInfo(string saveName)
    {
      saveName = Path.GetFileNameWithoutExtension(saveName);
      string[] strArray = saveName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 5 && strArray.Length != 6)
        return null;
      SaveInfo saveInfo = new SaveInfo();
      saveInfo.Days = DefaultConverter.ParseInt(strArray[0]);
      saveInfo.Hours = DefaultConverter.ParseInt(strArray[1]);
      saveInfo.Minutes = DefaultConverter.ParseInt(strArray[2]);
      DefaultConverter.TryParseEnum(strArray[3], out saveInfo.Building);
      saveInfo.Id = strArray[4];
      saveInfo.SaveDateTime = DateTime.MinValue;
      if (strArray.Length == 6)
      {
        string s = strArray[5];
        if (!DateTime.TryParseExact(s, "yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out saveInfo.SaveDateTime))
          Debug.LogError("Error parse date : " + s);
      }
      return saveInfo;
    }

    public static string GetFileName(int days, int hours, int minutes, BuildingEnum building)
    {
      return days.ToString().PadLeft(2, '0') + "-" + hours.ToString().PadLeft(2, '0') + "-" + minutes.ToString().PadLeft(2, '0') + "-" + building + "-" + Guid.NewGuid().ToString().Replace("-", "") + "-" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
    }
  }
}
