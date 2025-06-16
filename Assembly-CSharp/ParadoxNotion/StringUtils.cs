using System;
using System.Text.RegularExpressions;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ParadoxNotion;

public static class StringUtils {
	public static string SplitCamelCase(this string s) {
		if (string.IsNullOrEmpty(s))
			return s;
		s = s.Replace("_", " ");
		s = char.ToUpper(s[0]) + s.Substring(1);
		return Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
	}

	public static string GetCapitals(this string s) {
		if (string.IsNullOrEmpty(s))
			return string.Empty;
		var str = "";
		foreach (var c in s)
			if (char.IsUpper(c))
				str += c.ToString();
		return str.Trim();
	}

	public static string GetAlphabetLetter(int index) {
		if (index < 0)
			return null;
		var str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		return index >= str.Length ? index.ToString() : str[index].ToString();
	}

	public static string ToStringAdvanced(this object o) {
		if (o == null || o.Equals(null))
			return "NULL";
		switch (o) {
			case string _:
				return string.Format("\"{0}\"", (string)o);
			case Object _:
				return (o as Object).name;
			default:
				var type = o.GetType();
				return type.RTIsSubclassOf(typeof(Enum)) && type.RTGetAttribute<FlagsAttribute>(true) != null
					? EditorGUILayout2.GetEnumValueName((Enum)o)
					: o.ToString();
		}
	}
}