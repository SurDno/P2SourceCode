using Cofe.Loggers;
using Engine.Common.Binders;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Globalization;
using System.Text;

namespace PLVirtualMachine.Common.Data
{
  public static class StringSerializer
  {
    public static string LastError { get; set; } = "";

    public static Type ReadTypeByString(string typeName)
    {
      Type type = Type.GetType(typeName);
      Type result;
      if ((Type) null == type && EnumTypeAttribute.TryGetValue(typeName, out result))
        type = result;
      return type;
    }

    public static string WriteValue(object value)
    {
      if (value == null)
        return "";
      if (value.GetType() == typeof (string))
        return (string) value;
      try
      {
        if (typeof (IVMStringSerializable).IsAssignableFrom(value.GetType()))
          return ((IVMStringSerializable) value).Write();
        StringSerializer.LastError = "";
        if (value.GetType() == typeof (Guid))
          return GuidUtility.GetGuidString((Guid) value);
        if (value.GetType() == typeof (HierarchyGuid))
          return ((HierarchyGuid) value).Write();
        if (value.GetType() == typeof (byte[]))
          return Encoding.UTF8.GetString((byte[]) value);
        if (value.GetType() == typeof (float))
          return ((float) value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        return value.GetType() == typeof (double) ? ((double) value).ToString((IFormatProvider) CultureInfo.InvariantCulture) : value.ToString();
      }
      catch (Exception ex)
      {
        StringSerializer.LastError = string.Format("Cannot serialize value of type {0} to string: error {1} at {2}", (object) value.GetType(), (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(StringSerializer.LastError);
        return "";
      }
    }

    public static object ReadValue(string data, Type valueType)
    {
      StringSerializer.LastError = "";
      if ((Type) null == valueType)
      {
        StringSerializer.LastError = string.Format("value {0} conversion error: conversion type is null at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(StringSerializer.LastError);
        return (object) null;
      }
      if (!typeof (IVMStringSerializable).IsAssignableFrom(valueType))
        return StringSerializer.ReadStringValue(data, valueType);
      if (valueType.IsInterface)
      {
        valueType = BaseSerializer.GetRealRefType(valueType);
        if ((Type) null == valueType)
        {
          StringSerializer.LastError = string.Format("Invalid data interface type {0} at {1}", (object) valueType.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
          Logger.AddError(StringSerializer.LastError);
          return (object) null;
        }
      }
      IVMStringSerializable instance = (IVMStringSerializable) Activator.CreateInstance(valueType);
      instance.Read(data);
      return (object) instance;
    }

    private static object ReadStringValue(string data, Type valueType)
    {
      StringSerializer.LastError = "";
      try
      {
        if (valueType == typeof (string))
          return (object) data;
        if ((data == "0" || data == "") && valueType != typeof (object) && valueType != typeof (string) && (!valueType.IsValueType || valueType == typeof (bool)) || (data == "0" || data == "") && valueType.IsEnum)
          return BaseSerializer.GetDefaultValue(valueType);
        if (!valueType.IsEnum)
          return StringUtility.To(data, valueType);
        foreach (Enum @enum in Enum.GetValues(valueType))
        {
          if (@enum.ToString() == data)
            return (object) @enum;
        }
        StringSerializer.LastError = string.Format("Cannot convert value {0} to enum type {1} at {2}", (object) data, (object) valueType, (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(StringSerializer.LastError);
      }
      catch (Exception ex)
      {
        StringSerializer.LastError = string.Format("value {0} conversion error: {1} at {2}", (object) data, (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(StringSerializer.LastError);
      }
      return BaseSerializer.GetDefaultValue(valueType);
    }
  }
}
