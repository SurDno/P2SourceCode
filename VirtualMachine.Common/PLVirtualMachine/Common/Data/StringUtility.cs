using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cofe.Serializations.Converters;

namespace PLVirtualMachine.Common.Data
{
  public static class StringUtility
  {
    public static object To(string text, Type type)
    {
      if (type == typeof (Guid))
        return ToGuid(text);
      if (type == typeof (ulong))
        return ToUInt64(text);
      if (type == typeof (HierarchyGuid))
        return new HierarchyGuid(text);
      if (type == typeof (long))
        return ToInt64(text);
      if (type == typeof (uint))
        return ToUInt32(text);
      if (type == typeof (int))
        return ToInt32(text);
      if (type == typeof (ushort))
        return ToUInt16(text);
      if (type == typeof (short))
        return ToInt16(text);
      if (type == typeof (byte))
        return ToByte(text);
      if (type == typeof (sbyte))
        return ToSByte(text);
      if (type == typeof (double))
        return ToDouble(text);
      if (type == typeof (float))
        return ToSingle(text);
      if (type == typeof (bool))
        return ToBoolean(text);
      if (type.IsEnum)
        return ToEnum(text, type);
      if (type == typeof (Type))
        return ToType(text);
      if (type == typeof (DateTime))
        return ToDateTime(text);
      if (type == typeof (byte[]))
        return Encoding.UTF8.GetBytes(text);
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (List<>) ? (IList) Activator.CreateInstance(type) : (object) text;
    }

    public static Guid ToGuid(string text) => "" == text ? Guid.Empty : new Guid(text);

    public static ulong ToUInt64(string text)
    {
      ulong result;
      return text == "" || !ulong.TryParse(text, out result) ? 0UL : result;
    }

    public static long ToInt64(string text)
    {
      long result;
      return text == "" || !long.TryParse(text, out result) ? 0L : result;
    }

    public static uint ToUInt32(string text)
    {
      uint result;
      return text == "" || !uint.TryParse(text, out result) ? 0U : result;
    }

    public static int ToInt32(string text)
    {
      int result;
      return text == "" || !int.TryParse(text, out result) ? 0 : result;
    }

    public static ushort ToUInt16(string text)
    {
      if (text == "")
        return 0;
      ushort result;
      if (!ushort.TryParse(text, out result))
        throw new Exception("Cannot convert " + text + " " + MethodBase.GetCurrentMethod().Name);
      return result;
    }

    public static short ToInt16(string text)
    {
      if (text == "")
        return 0;
      short result;
      if (!short.TryParse(text, out result))
        throw new Exception("Cannot convert " + text + " " + MethodBase.GetCurrentMethod().Name);
      return result;
    }

    public static byte ToByte(string text)
    {
      if (text == "")
        return 0;
      byte result;
      if (!byte.TryParse(text, out result))
        throw new Exception("Cannot convert " + text + " " + MethodBase.GetCurrentMethod().Name);
      return result;
    }

    public static sbyte ToSByte(string text)
    {
      if (text == "")
        return 0;
      sbyte result;
      if (!sbyte.TryParse(text, out result))
        throw new Exception("Cannot convert " + text + " " + MethodBase.GetCurrentMethod().Name);
      return result;
    }

    public static double ToDouble(string text)
    {
      if (text == "")
        return 0.0;
      double result;
      if (!DefaultConverter.TryParseDouble(text, out result))
        throw new Exception("Cannot convert " + text + " " + MethodBase.GetCurrentMethod().Name);
      return result;
    }

    public static float ToSingle(string text)
    {
      if (text == "")
        return 0.0f;
      float result;
      if (!DefaultConverter.TryParseFloat(text, out result))
        throw new Exception("Cannot convert " + text + " " + MethodBase.GetCurrentMethod().Name);
      return result;
    }

    public static bool ToBoolean(string text) => text == "True";

    public static object ToEnum(string text, Type type)
    {
      try
      {
        return Enum.Parse(type, text, true);
      }
      catch
      {
        throw new Exception("Cannot convert " + text + " to Enum<" + type.FullName + ">");
      }
    }

    public static T ToEnum<T>(string text)
    {
      try
      {
        return (T) Enum.Parse(typeof (T), text, true);
      }
      catch
      {
        throw new Exception("Cannot convert " + text + " to Enum<" + typeof (T).FullName + ">");
      }
    }

    public static Type ToType(string text) => Type.GetType(text);

    public static DateTime ToDateTime(string text)
    {
      DateTime result = DateTime.MinValue;
      if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        throw new Exception("Cannot convert " + text + " to DateTime");
      return result;
    }
  }
}
