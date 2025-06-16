using System;
using System.Text;
using Cofe.Loggers;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common.Data
{
  public static class BinarySerializer
  {
    public static string LastError { get; set; } = "";

    public static object ReadValue(byte[] data, Type valueType)
    {
      LastError = "";
      if (null == valueType)
      {
        LastError = string.Format("value {0} conversion error: conversion type is null at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(LastError);
        return null;
      }
      if (!typeof (IVMStringSerializable).IsAssignableFrom(valueType))
        return ReadBinaryValue(data, valueType);
      if (valueType.IsInterface)
      {
        valueType = BaseSerializer.GetRealRefType(valueType);
        if (null == valueType)
        {
          LastError = string.Format("Invalid data interface type {0} at {1}", valueType, EngineAPIManager.Instance.CurrentFSMStateInfo);
          Logger.AddError(LastError);
          return null;
        }
      }
      IVMStringSerializable instance = (IVMStringSerializable) Activator.CreateInstance(valueType);
      instance.Read(Encoding.UTF8.GetString(data));
      return instance;
    }

    private static object ReadBinaryValue(byte[] bytes, Type valueType)
    {
      try
      {
        if (valueType.IsEnum)
        {
          string str = Encoding.UTF8.GetString(bytes);
          foreach (Enum @enum in Enum.GetValues(valueType))
          {
            if (@enum.ToString() == str)
              return @enum;
          }
        }
        else
        {
          if (valueType == typeof (bool))
            return SerializerUtility.ReadBool(bytes);
          if (valueType == typeof (short))
            return SerializerUtility.ReadInt16(bytes);
          if (valueType == typeof (ushort))
            return SerializerUtility.ReadUInt16(bytes);
          if (valueType == typeof (int))
            return SerializerUtility.ReadInt32(bytes);
          if (valueType == typeof (uint))
            return SerializerUtility.ReadUInt32(bytes);
          if (valueType == typeof (long))
            return SerializerUtility.ReadInt64(bytes);
          if (valueType == typeof (ulong))
            return SerializerUtility.ReadUInt64(bytes);
          if (valueType == typeof (float))
            return SerializerUtility.ReadSingle(bytes);
          if (valueType == typeof (double))
            return SerializerUtility.ReadDouble(bytes);
          if (valueType == typeof (Guid))
            return SerializerUtility.ReadGuid(bytes);
          if (valueType == typeof (string))
            return Encoding.UTF8.GetString(bytes);
        }
      }
      catch (Exception ex)
      {
        LastError = string.Format("value {0} conversion error: {1} at {2}", bytes, ex, EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(LastError);
        return null;
      }
      return BaseSerializer.GetDefaultValue(valueType);
    }
  }
}
