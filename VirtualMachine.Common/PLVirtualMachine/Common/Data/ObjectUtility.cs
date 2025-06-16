// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.Data.ObjectUtility
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;

#nullable disable
namespace PLVirtualMachine.Common.Data
{
  public static class ObjectUtility
  {
    public static T To<T>(object obj) => (T) ObjectUtility.To(obj, typeof (T));

    public static object To(object obj, Type type)
    {
      if (type == typeof (ulong))
        return (object) Convert.ToUInt64(obj);
      if (type == typeof (long))
        return (object) Convert.ToInt64(obj);
      if (type == typeof (uint))
        return (object) Convert.ToUInt32(obj);
      if (type == typeof (int))
        return (object) Convert.ToInt32(obj);
      if (type == typeof (ushort))
        return (object) Convert.ToUInt16(obj);
      if (type == typeof (short))
        return (object) Convert.ToInt16(obj);
      if (type == typeof (byte))
        return (object) Convert.ToByte(obj);
      if (type == typeof (sbyte))
        return (object) Convert.ToSByte(obj);
      if (type == typeof (double))
        return (object) Convert.ToDouble(obj);
      if (type == typeof (float))
      {
        string s = obj as string;
        if (!string.IsNullOrEmpty(s))
        {
          float result;
          if (float.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            return (object) result;
          Logger.AddWarning("Error parse , method : " + MethodBase.GetCurrentMethod().Name + " , value : " + s);
        }
        return (object) 0.0f;
      }
      if (type == typeof (bool))
        return (object) Convert.ToBoolean(obj);
      if (type == typeof (DateTime))
        return (object) Convert.ToDateTime(obj);
      if (type == typeof (string))
      {
        switch (obj)
        {
          case byte[] _:
            return (object) ObjectUtility.To<string>((object) (byte[]) obj);
          case double num1:
            return (object) num1.ToString("");
          case float num2:
            return (object) num2.ToString("");
          default:
            return (object) obj.ToString();
        }
      }
      else
      {
        if (type == typeof (byte[]))
          return (object) ObjectUtility.ToByteArray(obj);
        if (!(type == typeof (Enum)))
          throw new Exception("Error: Type " + type.Name + " == typeof not supported!");
        return obj is string ? Enum.Parse(type, (string) obj) : Convert.ChangeType(obj, type);
      }
    }

    public static byte[] ToByteArray(object obj)
    {
      switch (obj)
      {
        case null:
        case DBNull _:
          throw new Exception("Error: Object == typeof(null!");
        default:
          byte[] byteArray = (byte[]) null;
          if (obj.GetType() == typeof (byte[]))
            byteArray = (byte[]) obj;
          else if (obj.GetType() == typeof (object))
          {
            byteArray = (byte[]) obj;
          }
          else
          {
            switch (obj)
            {
              case ulong num1:
                byteArray = BitConverter.GetBytes(num1);
                break;
              case long num2:
                byteArray = BitConverter.GetBytes(num2);
                break;
              case uint num3:
                byteArray = BitConverter.GetBytes(num3);
                break;
              case int num4:
                byteArray = BitConverter.GetBytes(num4);
                break;
              case ushort num5:
                byteArray = BitConverter.GetBytes(num5);
                break;
              case short num6:
                byteArray = BitConverter.GetBytes(num6);
                break;
              case byte num7:
                byteArray = BitConverter.GetBytes((short) num7);
                break;
              case sbyte num8:
                byteArray = BitConverter.GetBytes((short) num8);
                break;
              case double num9:
                byteArray = BitConverter.GetBytes(num9);
                break;
              case float num10:
                byteArray = BitConverter.GetBytes(num10);
                break;
              case bool flag:
                byteArray = BitConverter.GetBytes(flag);
                break;
              case string _:
                byteArray = Encoding.UTF8.GetBytes((string) obj);
                break;
            }
          }
          return byteArray;
      }
    }
  }
}
