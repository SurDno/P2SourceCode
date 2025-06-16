using System;
using System.Collections.Generic;
using Cofe.Meta;
using Cofe.Serializations.Converters;

namespace Engine.Source.Services.Consoles.Binds;

[Initialisable]
public static class SetConsoleCommand {
	private static Dictionary<string, Holder> binds = new();

	public static void AddBind(
		Type type,
		Type valueType,
		string name,
		bool needTarget,
		Action<object, object> action) {
		var holder = new Holder {
			Type = type,
			Func = (target, value) => {
				object result1;
				if (valueType.IsEnum) {
					Enum result2;
					if (!DefaultConverter.TryParseEnum(value, valueType, out result2))
						return "Error parse value : \"" + value + "\" , name : " + name;
					result1 = result2;
				} else {
					if (!ConvertService.ContainsConverter(valueType))
						return "Error, parser type not found : " + valueType + " , name : " + name;
					if (!ConvertService.TryParse(valueType, value, out result1))
						return "Error parse value : \"" + value + "\" , name : " + name;
				}

				if (needTarget) {
					target = ConsoleTargetService.GetTarget(type, target);
					if (target == null)
						return "Error, target not found : " + type + " , name : " + name;
				}

				action(target, result1);
				return "Change " + name + " to : " + value;
			}
		};
		binds.Add(name, holder);
	}

	public static void AddBind<TObject, T>(string name, bool needTarget, Action<TObject, T> action)
		where TObject : class {
		AddBind(typeof(TObject), typeof(T), name, needTarget, (target, value) => action(target as TObject, (T)value));
	}

	[ConsoleCommand("set")]
	private static string Command(string command, ConsoleParameter[] parameters) {
		if (parameters.Length == 0 || (parameters.Length == 1 && parameters[0].Value == "?")) {
			var str = command + " [target] property value\n\nProperties :\n";
			foreach (var bind in binds)
				str = str + bind.Key + "\n";
			return str;
		}

		Holder holder;
		object target;
		string str1;
		if (parameters.Length == 2) {
			if (!binds.TryGetValue(parameters[0].Value, out holder))
				return "Parameter not found";
			target = ConsoleTargetService.GetTarget(holder.Type, new ConsoleParameter());
			str1 = parameters[1].Value;
		} else {
			if (parameters.Length != 3)
				return "Error parameter count";
			if (!binds.TryGetValue(parameters[1].Value, out holder))
				return "Parameter not found";
			target = ConsoleTargetService.GetTarget(holder.Type, parameters[0]);
			str1 = parameters[2].Value;
		}

		return holder.Func(target, str1);
	}

	private class Holder {
		public Type Type;
		public Func<object, string, string> Func;
	}
}