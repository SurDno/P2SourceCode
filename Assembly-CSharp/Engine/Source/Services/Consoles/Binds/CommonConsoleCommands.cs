using System.Collections;
using System.IO;
using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class CommonConsoleCommands {
	[ConsoleCommand("test")]
	private static string TestCommand(string command, ConsoleParameter[] parameters) {
		var str = "Command : \"" + command + "\"\n\n";
		for (var index = 0; index < parameters.Length; ++index) {
			var parameter = parameters[index];
			str = str + "Parameter : \"" + parameter.Parameter + "\"\n" + "Value : \"" + parameter.Value + "\"\n\n";
		}

		return str;
	}

	[ConsoleCommand("print")]
	private static string PrintCommand(string command, ConsoleParameter[] parameters) {
		if (parameters.Length == 0 || parameters.Length > 1 || (parameters.Length == 1 && parameters[0].Value == "?"))
			return command + " filename";
		return !File.Exists(parameters[0].Value)
			? "file not found : " + parameters[0].Value
			: File.ReadAllText(parameters[0].Value);
	}

	[ConsoleCommand("exec")]
	private static string ExecCommand(string command, ConsoleParameter[] parameters) {
		if (parameters.Length == 0 || parameters.Length > 1 || (parameters.Length == 1 && parameters[0].Value == "?"))
			return command + " filename";
		var str1 = "Data/ConsoleCommands/";
		var str2 = ".txt";
		var path = parameters[0].Value;
		if (!File.Exists(path)) {
			if (File.Exists(parameters[0].Value + str2))
				path = parameters[0].Value + str2;
			else if (File.Exists(str1 + parameters[0].Value))
				path = str1 + parameters[0].Value;
			else {
				if (!File.Exists(str1 + parameters[0].Value + str2))
					return "file not found : " + parameters[0].Value;
				path = str1 + parameters[0].Value + str2;
			}
		}

		CoroutineService.Instance.Route(ExecuteCommands(File.ReadAllLines(path)));
		return "";
	}

	private static IEnumerator ExecuteCommands(string[] lines) {
		var console = ServiceLocator.GetService<ConsoleService>();
		var gotoTag = "#goto";
		var delayTag = "#delay";
		var labelTag = ":";
		for (var index = 0; index < lines.Length; ++index) {
			var line = lines[index];
			if (line == "")
				yield return null;
			else if (line.StartsWith("//"))
				yield return null;
			else if (line.StartsWith(":"))
				yield return null;
			else if (line.StartsWith(gotoTag)) {
				var valueText = line.Substring(gotoTag.Length).Trim();
				var num = lines.IndexOf(labelTag + valueText);
				if (num == -1) {
					console.AddLine("error: label '" + valueText + "' not found");
					break;
				}

				index = num - 1;
				console.AddLine(line);
				yield return null;
			} else if (line.StartsWith(delayTag)) {
				var valueText = line.Substring(delayTag.Length).Trim();
				var value = DefaultConverter.ParseFloat(valueText);
				console.AddLine(line);
				yield return new WaitForSecondsRealtime(value);
			} else {
				console.ExecuteCommand(line);
				yield return null;
				line = null;
			}
		}
	}
}