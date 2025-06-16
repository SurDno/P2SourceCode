using System;
using System.Collections.Generic;
using Cofe.Utility;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services.Consoles;

[RuntimeService(typeof(ConsoleService))]
public class ConsoleService {
	public const string HelpToken = "?";
	public const char ParameterToken = '-';
	public const string ParameterTokenText = "-";
	public const char DelimiterToken = ':';
	public const string DelimiterTokenText = ":";
	public const char SpaceToken = ' ';
	public const string NewLineToken = "\n";
	public const char QuoteToken = '"';
	public const string CommandNotFoundText = "Command not found : ";

	public const string CommonHelpText =
		"Format :\ncommand [value | \"value with spaces\" | -param | -param: value | -param: \"value with spaces\"]\n\nCommands :\n";

	public const string FirstHelpText = "Print \"?\" for help";
	public const string PrefixExecuteCommandText = "Execute command : ";
	private Action<string> onAddedLine;
	private object sync = new();
	[Inspected] private static Dictionary<string, Func<string, ConsoleParameter[], string>> commands = new();

	public event Action<string> OnAddedLine {
		add {
			lock (sync) {
				if (onAddedLine == null && value != null)
					value("Print \"?\" for help");
				onAddedLine += value;
			}
		}
		remove {
			lock (sync) {
				onAddedLine -= value;
			}
		}
	}

	public static void RegisterCommand(string name, Func<string, ConsoleParameter[], string> func) {
		if (commands.ContainsKey(name))
			Debug.LogError("Command exist : " + name);
		else
			commands.Add(name, func);
	}

	public void ExecuteCommand(string value) {
		if (value == "?")
			AddLine(GetHelpText());
		else {
			var str = "Execute command : \"" + value + "\"\n" + ComputeCommand(value);
			var chArray = new char[1] { '\n' };
			foreach (var line in str.Split(chArray))
				AddLine(line);
		}
	}

	public void AddLine(string line) {
		var onAddedLine = this.onAddedLine;
		if (onAddedLine == null)
			return;
		onAddedLine(line);
	}

	private string GetHelpText() {
		var helpText =
			"Format :\ncommand [value | \"value with spaces\" | -param | -param: value | -param: \"value with spaces\"]\n\nCommands :\n";
		foreach (var key in commands.Keys)
			helpText = helpText + key + "\n";
		return helpText;
	}

	private string ComputeCommand(string value) {
		value = value.Trim();
		var num = value.IndexOf(' ');
		string key;
		if (num == -1) {
			key = value;
			value = "";
		} else {
			key = value.Substring(0, num);
			value = value.Substring(num);
		}

		Func<string, ConsoleParameter[], string> func = null;
		if (!commands.TryGetValue(key, out func))
			return "Command not found : " + key;
		var parameters = GetParameters(value);
		return func(key, parameters.ToArray());
	}

	private List<ConsoleParameter> GetParameters(string text) {
		text = text.Trim();
		var parameters = new List<ConsoleParameter>();
		var stringList = new List<string>();
		var startIndex = 0;
		var flag = false;
		for (var index = 0; index < text.Length; ++index) {
			var ch = text[index];
			if (ch == '"') {
				if (flag) {
					var str = text.Substring(startIndex, index - startIndex + 1).Trim();
					if (!str.IsNullOrEmpty())
						stringList.Add(str);
					startIndex = index + 1;
					flag = false;
				} else {
					startIndex = index;
					flag = true;
				}
			}

			if (!flag && (ch == ' ' || ch == ':')) {
				var str = text.Substring(startIndex, index - startIndex + 1).Trim();
				if (!str.IsNullOrEmpty()) {
					stringList.Add(str);
					startIndex = index + 1;
				}
			}
		}

		var str1 = text.Substring(startIndex).Trim();
		if (!str1.IsNullOrEmpty())
			stringList.Add(str1);
		for (var index = 0; index < stringList.Count; ++index) {
			var str2 = stringList[index];
			ConsoleParameter consoleParameter1;
			if (str2.StartsWith("-")) {
				if (str2.EndsWith(":")) {
					if (index + 1 < stringList.Count) {
						var consoleParameterList = parameters;
						consoleParameter1 = new ConsoleParameter();
						consoleParameter1.Parameter = str2.Trim(':');
						consoleParameter1.Value = stringList[index + 1].Trim(' ', '"');
						var consoleParameter2 = consoleParameter1;
						consoleParameterList.Add(consoleParameter2);
						++index;
					} else {
						var consoleParameterList = parameters;
						consoleParameter1 = new ConsoleParameter();
						consoleParameter1.Parameter = str2.Trim(':');
						consoleParameter1.Value = "";
						var consoleParameter3 = consoleParameter1;
						consoleParameterList.Add(consoleParameter3);
					}
				} else {
					var consoleParameterList = parameters;
					consoleParameter1 = new ConsoleParameter();
					consoleParameter1.Parameter = str2;
					consoleParameter1.Value = "";
					var consoleParameter4 = consoleParameter1;
					consoleParameterList.Add(consoleParameter4);
				}
			} else {
				var consoleParameterList = parameters;
				consoleParameter1 = new ConsoleParameter();
				consoleParameter1.Parameter = "";
				consoleParameter1.Value = str2.Trim(' ', '"');
				var consoleParameter5 = consoleParameter1;
				consoleParameterList.Add(consoleParameter5);
			}
		}

		return parameters;
	}
}