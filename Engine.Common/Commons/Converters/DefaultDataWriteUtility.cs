using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;

namespace Engine.Common.Commons.Converters
{
  public static class DefaultDataWriteUtility
  {
    public static void Write(IDataWriter writer, string name, byte value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, sbyte value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, int value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, uint value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, short value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, ushort value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, long value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, ulong value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, float value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, double value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, char value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, bool value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, Guid value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, System.DateTime value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, TimeSpan value)
    {
      writer.Begin(name, null, false);
      writer.Write(DefaultConverter.ToString(value));
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, string value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, false);
      writer.Write(value);
      writer.End(name, false);
    }

    public static void Write(IDataWriter writer, string name, byte[] value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, false);
      writer.Write(Convert.ToBase64String(value));
      writer.End(name, false);
    }

    public static void WriteEnum<T>(IDataWriter writer, string name, T value) where T : struct, IComparable, IConvertible, IFormattable
    {
      writer.Begin(name, null, false);
      writer.Write(value.ToString());
      writer.End(name, false);
    }

    public static void WriteSerialize<T>(IDataWriter writer, string name, T value)
    {
      if (value == null)
        return;
      if (value is ISerializeDataWrite serializeDataWrite)
      {
        Type type = ProxyFactory.GetType(value.GetType());
        writer.Begin(name, type != typeof (T) ? type : null, true);
        serializeDataWrite.DataWrite(writer);
        writer.End(name, true);
      }
      else
        Logger.AddError("Type : " + value.GetType().Name + " is not " + typeof (ISerializeDataWrite).Name);
    }

    public static void WriteList(IDataWriter writer, string name, List<string> value)
    {
      if (value == null)
        value = new List<string>();
      writer.Begin(name, null, true);
      foreach (string str in value)
        Write(writer, "Item", str);
      writer.End(name, true);
    }

    public static void WriteList(IDataWriter writer, string name, List<double> value)
    {
      if (value == null)
        value = new List<double>();
      writer.Begin(name, null, true);
      foreach (double num in value)
        Write(writer, "Item", num);
      writer.End(name, true);
    }

    public static void WriteListEnum<T>(IDataWriter writer, string name, List<T> value) where T : struct, IComparable, IConvertible, IFormattable
    {
      if (value == null)
        value = new List<T>();
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteEnum(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteListSerialize<T>(IDataWriter writer, string name, List<T> value) where T : class
    {
      if (value == null)
        value = new List<T>();
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteSerialize(writer, "Item", obj);
      writer.End(name, true);
    }
  }
}
