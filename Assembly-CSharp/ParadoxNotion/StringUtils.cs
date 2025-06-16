using System;
using System.Text.RegularExpressions;
using UnityEditor;

namespace ParadoxNotion
{
  public static class StringUtils
  {
    public static string SplitCamelCase(this string s)
    {
      if (string.IsNullOrEmpty(s))
        return s;
      s = s.Replace("_", " ");
      s = char.ToUpper(s[0]).ToString() + s.Substring(1);
      return Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
    }

    public static string GetCapitals(this string s)
    {
      if (string.IsNullOrEmpty(s))
        return string.Empty;
      string str = "";
      foreach (char c in s)
      {
        if (char.IsUpper(c))
          str += c.ToString();
      }
      return str.Trim();
    }

    public static string GetAlphabetLetter(int index)
    {
      if (index < 0)
        return (string) null;
      string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      return index >= str.Length ? index.ToString() : str[index].ToString();
    }

    public static string ToStringAdvanced(this object o)
    {
      if (o == null || o.Equals((object) null))
        return "NULL";
      switch (o)
      {
        case string _:
          return string.Format("\"{0}\"", (object) (string) o);
        case UnityEngine.Object _:
          return (o as UnityEngine.Object).name;
        default:
          System.Type type = o.GetType();
          return type.RTIsSubclassOf(typeof (Enum)) && ReflectionTools.RTGetAttribute<FlagsAttribute>(type, true) != null ? EditorGUILayout2.GetEnumValueName((Enum) o) : o.ToString();
      }
    }
  }
}
