using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace Engine.Common.Binders;

[AttributeUsage(AttributeTargets.Enum)]
public class EnumTypeAttribute : TypeAttribute {
	public readonly string Name;
	private static Dictionary<string, Type> types = new();
	private static Dictionary<Type, string> names = new();

	public EnumTypeAttribute(string name) {
		Name = name;
	}

	public override void ComputeType(Type type) {
		types.Add(Name, type);
		names.Add(type, Name);
	}

	public static bool TryGetValue(Type type, out string result) {
		return names.TryGetValue(type, out result);
	}

	public static bool TryGetValue(string name, out Type result) {
		return types.TryGetValue(name, out result);
	}
}