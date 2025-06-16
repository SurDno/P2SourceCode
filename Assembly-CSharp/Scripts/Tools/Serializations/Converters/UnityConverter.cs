// Decompiled with JetBrains decompiler
// Type: Scripts.Tools.Serializations.Converters.UnityConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Cofe.Utility;
using System;
using UnityEngine;

#nullable disable
namespace Scripts.Tools.Serializations.Converters
{
  public static class UnityConverter
  {
    private static char[] separator = new char[1]{ ' ' };
    private static float[] floatBuffer = new float[4];

    public static bool TryParseComplex(string value, int count)
    {
      if (count > UnityConverter.floatBuffer.Length || value.IsNullOrEmpty())
        return false;
      string[] strArray = value.Split(UnityConverter.separator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != count)
        return false;
      for (int index = 0; index < count; ++index)
      {
        if (!DefaultConverter.TryParseFloat(strArray[index], out UnityConverter.floatBuffer[index]))
          return false;
      }
      return true;
    }

    public static bool TryParseVector2(string value, out Vector2 result)
    {
      if (UnityConverter.TryParseComplex(value, 2))
      {
        result = new Vector2(UnityConverter.floatBuffer[0], UnityConverter.floatBuffer[1]);
        return true;
      }
      result = Vector2.zero;
      return false;
    }

    public static Vector2 ParseVector2(string value)
    {
      Vector2 result;
      if (!UnityConverter.TryParseVector2(value, out result))
        Debug.LogWarning((object) ("Error parse value : " + (value != null ? value : "null") + "\n" + ObjectInfoUtility.GetStackTrace()));
      return result;
    }

    public static string ToString(Vector2 value)
    {
      return DefaultConverter.ToString(value.x) + " " + DefaultConverter.ToString(value.y);
    }

    public static bool TryParseVector3(string value, out Vector3 result)
    {
      if (UnityConverter.TryParseComplex(value, 3))
      {
        result = new Vector3(UnityConverter.floatBuffer[0], UnityConverter.floatBuffer[1], UnityConverter.floatBuffer[2]);
        return true;
      }
      result = Vector3.zero;
      return false;
    }

    public static Vector3 ParseVector3(string value)
    {
      Vector3 result;
      if (!UnityConverter.TryParseVector3(value, out result))
        Debug.LogWarning((object) ("Error parse value : " + (value != null ? value : "null") + "\n" + ObjectInfoUtility.GetStackTrace()));
      return result;
    }

    public static string ToString(Vector3 value)
    {
      return DefaultConverter.ToString(value.x) + " " + DefaultConverter.ToString(value.y) + " " + DefaultConverter.ToString(value.z);
    }

    public static bool TryParseVector4(string value, out Vector4 result)
    {
      if (UnityConverter.TryParseComplex(value, 4))
      {
        result = new Vector4(UnityConverter.floatBuffer[0], UnityConverter.floatBuffer[1], UnityConverter.floatBuffer[2], UnityConverter.floatBuffer[3]);
        return true;
      }
      result = Vector4.zero;
      return false;
    }

    public static Vector4 ParseVector4(string value)
    {
      Vector4 result;
      if (!UnityConverter.TryParseVector4(value, out result))
        Debug.LogWarning((object) ("Error parse value : " + (value != null ? value : "null") + "\n" + ObjectInfoUtility.GetStackTrace()));
      return result;
    }

    public static string ToString(Vector4 value)
    {
      return DefaultConverter.ToString(value.x) + " " + DefaultConverter.ToString(value.y) + " " + DefaultConverter.ToString(value.z) + " " + DefaultConverter.ToString(value.w);
    }

    public static bool TryParseQuaternion(string value, out Quaternion result)
    {
      if (UnityConverter.TryParseComplex(value, 4))
      {
        result = new Quaternion(UnityConverter.floatBuffer[0], UnityConverter.floatBuffer[1], UnityConverter.floatBuffer[2], UnityConverter.floatBuffer[3]);
        return true;
      }
      result = Quaternion.identity;
      return false;
    }

    public static Quaternion ParseQuaternion(string value)
    {
      Quaternion result;
      if (!UnityConverter.TryParseQuaternion(value, out result))
        Debug.LogWarning((object) ("Error parse value : " + (value != null ? value : "null") + "\n" + ObjectInfoUtility.GetStackTrace()));
      return result;
    }

    public static string ToString(Quaternion value)
    {
      return DefaultConverter.ToString(value.x) + " " + DefaultConverter.ToString(value.y) + " " + DefaultConverter.ToString(value.z) + " " + DefaultConverter.ToString(value.w);
    }

    public static bool TryParseColor(string value, out Color result)
    {
      if (UnityConverter.TryParseComplex(value, 4))
      {
        result = new Color(UnityConverter.floatBuffer[0], UnityConverter.floatBuffer[1], UnityConverter.floatBuffer[2], UnityConverter.floatBuffer[3]);
        return true;
      }
      if (UnityConverter.TryParseComplex(value, 3))
      {
        result = new Color(UnityConverter.floatBuffer[0], UnityConverter.floatBuffer[1], UnityConverter.floatBuffer[2], 1f);
        return true;
      }
      result = Color.clear;
      return false;
    }

    public static Color ParseColor(string value)
    {
      Color result;
      if (!UnityConverter.TryParseColor(value, out result))
        Debug.LogWarning((object) ("Error parse value : " + (value != null ? value : "null") + "\n" + ObjectInfoUtility.GetStackTrace()));
      return result;
    }

    public static string ToString(Color value)
    {
      return DefaultConverter.ToString(value.r) + " " + DefaultConverter.ToString(value.g) + " " + DefaultConverter.ToString(value.b) + " " + DefaultConverter.ToString(value.a);
    }
  }
}
