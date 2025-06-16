using System;
using System.Collections.Generic;

namespace Engine.Source.Services;

public static class ProfilerUtility {
	private static Dictionary<Type, string> types = new();

	public static string GetTypeName(Type type) {
		string name;
		if (!types.TryGetValue(type, out name)) {
			name = type.Name;
			types.Add(type, name);
		}

		return name;
	}
}