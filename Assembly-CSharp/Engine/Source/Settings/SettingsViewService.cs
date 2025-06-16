using System.Collections.Generic;
using Cofe.Utility;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Settings;

[RuntimeService(typeof(SettingsViewService))]
public class SettingsViewService {
	[Inspected] private static List<object> settings = new();

	public static void AddSettings(object setting) {
		settings.Add(setting);
		settings.Sort((a, b) => TypeUtility.GetTypeName(a.GetType()).CompareTo(TypeUtility.GetTypeName(b.GetType())));
	}
}