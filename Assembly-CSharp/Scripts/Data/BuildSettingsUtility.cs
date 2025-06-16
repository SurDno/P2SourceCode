// Decompiled with JetBrains decompiler
// Type: Scripts.Data.BuildSettingsUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace Scripts.Data
{
  public static class BuildSettingsUtility
  {
    public static IEnumerable<GameDataInfo> GetAllGameData()
    {
      Dictionary<string, GameDataInfo> dictionary = new Dictionary<string, GameDataInfo>();
      foreach (GameDataInfo gameDataInfo in ScriptableObjectInstance<BuildSettings>.Instance.Data)
        dictionary[gameDataInfo.Name] = gameDataInfo;
      return (IEnumerable<GameDataInfo>) dictionary.Values;
    }

    public static GameDataInfo GetGameData(string projectName)
    {
      return BuildSettingsUtility.GetAllGameData().FirstOrDefault<GameDataInfo>((Func<GameDataInfo, bool>) (o => o.Name == projectName)) ?? throw new Exception("Game data name : " + projectName + " not found");
    }

    public static GameDataInfo GetDefaultGameData()
    {
      return BuildSettingsUtility.GetGameData(ScriptableObjectInstance<BuildSettings>.Instance.FirstDataName);
    }

    public static bool IsDataExist(string projectName)
    {
      return File.Exists(PlatformUtility.GetPath("Data/VirtualMachine/{ProjectName}".Replace("{ProjectName}", projectName) + "/Version.xml"));
    }
  }
}
