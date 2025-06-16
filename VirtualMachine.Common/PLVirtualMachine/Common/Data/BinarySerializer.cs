// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.Data.BinarySerializer
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Text;

#nullable disable
namespace PLVirtualMachine.Common.Data
{
  public static class BinarySerializer
  {
    public static string LastError { get; set; } = "";

    public static object ReadValue(byte[] data, Type valueType)
    {
      BinarySerializer.LastError = "";
      if ((Type) null == valueType)
      {
        BinarySerializer.LastError = string.Format("value {0} conversion error: conversion type is null at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(BinarySerializer.LastError);
        return (object) null;
      }
      if (!typeof (IVMStringSerializable).IsAssignableFrom(valueType))
        return BinarySerializer.ReadBinaryValue(data, valueType);
      if (valueType.IsInterface)
      {
        valueType = BaseSerializer.GetRealRefType(valueType);
        if ((Type) null == valueType)
        {
          BinarySerializer.LastError = string.Format("Invalid data interface type {0} at {1}", (object) valueType.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
          Logger.AddError(BinarySerializer.LastError);
          return (object) null;
        }
      }
      IVMStringSerializable instance = (IVMStringSerializable) Activator.CreateInstance(valueType);
      instance.Read(Encoding.UTF8.GetString(data));
      return (object) instance;
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
              return (object) @enum;
          }
        }
        else
        {
          if (valueType == typeof (bool))
            return (object) SerializerUtility.ReadBool(bytes);
          if (valueType == typeof (short))
            return (object) SerializerUtility.ReadInt16(bytes);
          if (valueType == typeof (ushort))
            return (object) SerializerUtility.ReadUInt16(bytes);
          if (valueType == typeof (int))
            return (object) SerializerUtility.ReadInt32(bytes);
          if (valueType == typeof (uint))
            return (object) SerializerUtility.ReadUInt32(bytes);
          if (valueType == typeof (long))
            return (object) SerializerUtility.ReadInt64(bytes);
          if (valueType == typeof (ulong))
            return (object) SerializerUtility.ReadUInt64(bytes);
          if (valueType == typeof (float))
            return (object) SerializerUtility.ReadSingle(bytes);
          if (valueType == typeof (double))
            return (object) SerializerUtility.ReadDouble(bytes);
          if (valueType == typeof (Guid))
            return (object) SerializerUtility.ReadGuid(bytes);
          if (valueType == typeof (string))
            return (object) Encoding.UTF8.GetString(bytes);
        }
      }
      catch (Exception ex)
      {
        BinarySerializer.LastError = string.Format("value {0} conversion error: {1} at {2}", (object) bytes, (object) ex, (object) EngineAPIManager.Instance.CurrentFSMStateInfo);
        Logger.AddError(BinarySerializer.LastError);
        return (object) null;
      }
      return BaseSerializer.GetDefaultValue(valueType);
    }
  }
}
