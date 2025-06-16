using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scripts.Data;

public static class BuildSettingsUtility {
	public static IEnumerable<GameDataInfo> GetAllGameData() {
		var dictionary = new Dictionary<string, GameDataInfo>();
		foreach (var gameDataInfo in ScriptableObjectInstance<BuildSettings>.Instance.Data)
			dictionary[gameDataInfo.Name] = gameDataInfo;
		return dictionary.Values;
	}

	public static GameDataInfo GetGameData(string projectName) {
		return GetAllGameData().FirstOrDefault(o => o.Name == projectName) ??
		       throw new Exception("Game data name : " + projectName + " not found");
	}

	public static GameDataInfo GetDefaultGameData() {
		return GetGameData(ScriptableObjectInstance<BuildSettings>.Instance.FirstDataName);
	}

	public static bool IsDataExist(string projectName) {
		return File.Exists(PlatformUtility.GetPath(
			"Data/VirtualMachine/{ProjectName}".Replace("{ProjectName}", projectName) + "/Version.xml"));
	}
}