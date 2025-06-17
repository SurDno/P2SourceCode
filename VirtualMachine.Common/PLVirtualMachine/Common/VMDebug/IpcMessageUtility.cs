using System;
using System.Collections.Generic;
using System.Text;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common.VMDebug
{
  public static class IpcMessageUtility
  {
    public static void SerializeInt(int value, List<byte> dDestBytesList)
    {
      foreach (byte num in BitConverter.GetBytes(value))
        dDestBytesList.Add(num);
    }

    public static int DeserializeInt(byte[] data, ref int offset)
    {
      int int32 = BitConverter.ToInt32(data, offset);
      offset += 4;
      return int32;
    }

    public static void SerializeBool(bool value, List<byte> dDestBytesList)
    {
      foreach (byte num in BitConverter.GetBytes(value))
        dDestBytesList.Add(num);
    }

    public static bool DeserializeBool(byte[] data, ref int offset)
    {
      int num = BitConverter.ToBoolean(data, offset) ? 1 : 0;
      ++offset;
      return num != 0;
    }

    public static void SerializeUInt64(ulong value, List<byte> dDestBytesList)
    {
      foreach (byte num in BitConverter.GetBytes(value))
        dDestBytesList.Add(num);
    }

    public static ulong DeserializeUInt64(byte[] data, ref int offset)
    {
      long uint64 = (long) BitConverter.ToUInt64(data, offset);
      offset += 8;
      return (ulong) uint64;
    }

    public static void SerializeGuid(Guid value, List<byte> dDestBytesList)
    {
      foreach (byte num in value.ToByteArray())
        dDestBytesList.Add(num);
    }

    public static Guid DeserializeGuid(byte[] data, ref int offset)
    {
      byte[] numArray = new byte[16];
      Array.Copy(data, offset, numArray, 0, 16);
      Guid guid = new Guid(numArray);
      offset += numArray.Length;
      return guid;
    }

    public static void SerializeString(string value, List<byte> dDestBytesList)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(value);
      foreach (byte num in BitConverter.GetBytes(bytes.Length))
        dDestBytesList.Add(num);
      for (int index = 0; index < bytes.Length; ++index)
        dDestBytesList.Add(bytes[index]);
    }

    public static string DeserializeString(byte[] data, ref int offset)
    {
      int int32 = BitConverter.ToInt32(data, offset);
      offset += 4;
      byte[] numArray = new byte[int32];
      Array.Copy(data, offset, numArray, 0, int32);
      offset += int32;
      return Encoding.UTF8.GetString(numArray);
    }

    public static void SerializeUInt64List(List<ulong> dValues, List<byte> dDestBytesList)
    {
      foreach (byte num in BitConverter.GetBytes(dValues.Count))
        dDestBytesList.Add(num);
      for (int index = 0; index < dValues.Count; ++index)
      {
        foreach (byte num in BitConverter.GetBytes(dValues[index]))
          dDestBytesList.Add(num);
      }
    }

    public static List<ulong> DeSerializeUInt64List(byte[] data, ref int offset)
    {
      List<ulong> ulongList = [];
      int int32 = BitConverter.ToInt32(data, offset);
      offset += 4;
      for (int index = 0; index < int32; ++index)
      {
        ulong uint64 = BitConverter.ToUInt64(data, offset);
        ulongList.Add(uint64);
        offset += 8;
      }
      return ulongList;
    }

    public static void SerializeValue(object value, List<byte> dDestBytesList)
    {
      try
      {
        if (value.GetType() == typeof (GameTime))
          value = ((GameTime) value).ToString();
        else if (value.GetType().IsEnum)
          value = value.ToString();
        byte[] byteArray = ObjectUtility.ToByteArray(value);
        foreach (byte num in BitConverter.GetBytes(byteArray.Length))
          dDestBytesList.Add(num);
        for (int index = 0; index < byteArray.Length; ++index)
          dDestBytesList.Add(byteArray[index]);
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public static object DeserializeValue(byte[] data, ref int offset, Type valueType)
    {
      int int32 = BitConverter.ToInt32(data, offset);
      offset += 4;
      byte[] numArray = new byte[int32];
      Array.Copy(data, offset, numArray, 0, int32);
      offset += int32;
      return BinarySerializer.ReadValue(numArray, valueType);
    }

    public static void SerializeData(byte[] byteData, List<byte> dataBytesList)
    {
      foreach (byte num in BitConverter.GetBytes(byteData.Length))
        dataBytesList.Add(num);
      for (int index = 0; index < byteData.Length; ++index)
        dataBytesList.Add(byteData[index]);
    }

    public static byte[] DeserializeData(byte[] data, ref int offset)
    {
      int int32 = BitConverter.ToInt32(data, offset);
      offset += 4;
      byte[] destinationArray = new byte[int32];
      Array.Copy(data, offset, destinationArray, 0, int32);
      offset += int32;
      return destinationArray;
    }
  }
}
