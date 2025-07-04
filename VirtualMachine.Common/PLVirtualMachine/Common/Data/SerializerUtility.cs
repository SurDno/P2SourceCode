﻿using System;

namespace PLVirtualMachine.Common.Data
{
  public static class SerializerUtility
  {
    public static bool ReadBool(byte[] data) => BitConverter.ToBoolean(data, 0);

    public static short ReadInt16(byte[] data) => BitConverter.ToInt16(data, 0);

    public static ushort ReadUInt16(byte[] data) => BitConverter.ToUInt16(data, 0);

    public static int ReadInt32(byte[] data) => BitConverter.ToInt32(data, 0);

    public static uint ReadUInt32(byte[] data) => BitConverter.ToUInt32(data, 0);

    public static long ReadInt64(byte[] data) => BitConverter.ToInt64(data, 0);

    public static ulong ReadUInt64(byte[] data) => BitConverter.ToUInt64(data, 0);

    public static float ReadSingle(byte[] data) => BitConverter.ToSingle(data, 0);

    public static double ReadDouble(byte[] data) => BitConverter.ToDouble(data, 0);

    public static Guid ReadGuid(byte[] data)
    {
      byte[] numArray = new byte[16];
      Array.Copy(data, 0, numArray, 0, 16);
      return new Guid(numArray);
    }
  }
}
