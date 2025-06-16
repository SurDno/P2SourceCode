using System;
using System.Globalization;
using Cofe.Meta;
using Cofe.Serializations.Converters;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class BindScreenCaptureConsoleCommands {
	[ConsoleCommand("screen_capture")]
	private static string ScreenCaptureCommand(string command, ConsoleParameter[] parameters) {
		if (parameters.Length == 1 && parameters[0].Value == "?")
			return command + " superSize";
		var superSize = 0;
		if (parameters.Length == 0)
			superSize = 1;
		else {
			if (parameters.Length != 1)
				return "Error parameter count";
			superSize = DefaultConverter.ParseInt(parameters[0].Parameter);
		}

		if (superSize <= 0)
			return "Error superSize";
		SRDebug.Instance.HideDebugPanel();
		var fileName = DateTime.Now.ToString(CultureInfo.InvariantCulture) + ".png";
		fileName = fileName.Replace("/", "_").Replace(":", "_");
		CoroutineService.Instance.WaitFrame((Action)(() => ScreenCapture.CaptureScreenshot(fileName, superSize)));
		return command + " " + fileName;
	}
}