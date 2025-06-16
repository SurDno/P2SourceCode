using System;

namespace UnityEditor;

public static class EditorGUILayout2 {
	private const string nameNothing = "Nothing";
	private const string nameEverything = "Everything";

	public static string GetEnumValueName(Enum enumValue) {
		var values = (int[])Enum.GetValues(enumValue.GetType());
		var names = Enum.GetNames(enumValue.GetType());
		var enumValueName = "";
		var num = Convert.ToInt32(enumValue);
		for (var index = 0; index < values.Length; ++index)
			if ((values[index] & num) != 0) {
				if (enumValueName != "")
					enumValueName += ", ";
				enumValueName += names[index];
			}

		if (enumValueName == "")
			enumValueName = "Nothing";
		return enumValueName;
	}
}