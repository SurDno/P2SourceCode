using System;
using System.IO;
using System.Linq;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Profiles;
using Engine.Source.Services.Saves;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public class SaveLoadConsoleCommands {
	[ConsoleCommand("save_game")]
	[ConsoleCommand("load_game")]
	private static string SaveLoadGame(string command, ConsoleParameter[] parameters) {
		if (parameters.Length > 1 || (parameters.Length == 1 && parameters[0].Value == "?"))
			return command + " [file name]";
		var profiles = ServiceLocator.GetService<ProfilesService>();
		var fileName = "";
		if (parameters.Length == 0) {
			if (command == "save_game")
				profiles.GenerateSaveName();
			fileName = profiles.Current.LastSave;
		} else {
			if (parameters.Length != 1)
				return "Error parameters count";
			fileName = parameters[0].Value;
		}

		if (!fileName.EndsWith(".xml"))
			fileName += ".xml";
		SRDebug.Instance.HideDebugPanel();
		CoroutineService.Instance.WaitFrame((Action)(() => {
			switch (command) {
				case "save_game":
					ServiceLocator.GetService<SavesService>()
						.Save(ProfilesUtility.SavePath(profiles.Current.Name, fileName));
					break;
				case "load_game":
					var service1 = ServiceLocator.GetService<VirtualMachineController>();
					var service2 = ServiceLocator.GetService<GameLauncher>();
					var str = ProfilesUtility.SavePath(profiles.Current.Name, fileName);
					if (File.Exists(str)) {
						if (service1.IsLoaded)
							service2.RestartGameWithSave(str);
						else
							service2.StartGameWithSave(str);
					} else
						Debug.LogError("Save file name not found : " + str);

					break;
			}
		}));
		return command + " " + fileName;
	}

	[ConsoleCommand("start_game")]
	private static string StartGame(string command, ConsoleParameter[] parameters) {
		if (parameters.Length > 1 || (parameters.Length == 1 && parameters[0].Value == "?"))
			return command + " [project name]";
		var projectName = "";
		if (parameters.Length == 0)
			projectName = "";
		else {
			if (parameters.Length != 1)
				return "Error parameters count";
			projectName = parameters[0].Value;
		}

		SRDebug.Instance.HideDebugPanel();
		CoroutineService.Instance.WaitFrame((Action)(() => {
			if (!(command == "start_game"))
				return;
			InstanceByRequest<GameDataService>.Instance.SetCurrentGameData(projectName);
			if (ServiceLocator.GetService<VirtualMachineController>().IsLoaded)
				ServiceLocator.GetService<GameLauncher>().RestartGame();
			else {
				var service = ServiceLocator.GetService<ProfilesService>();
				if (service.Profiles.Count() == 0)
					service.GenerateNewProfile(
						InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().GameName);
				ServiceLocator.GetService<GameLauncher>().StartNewGame();
			}
		}));
		return command + " " + projectName;
	}
}