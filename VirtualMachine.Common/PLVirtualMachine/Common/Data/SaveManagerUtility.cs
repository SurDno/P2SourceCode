using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;

namespace PLVirtualMachine.Common.Data
{
  public static class SaveManagerUtility
  {
    public static void Save(IDataWriter writer, string name, string value)
    {
      writer.WriteSimple(name, value);
    }

    public static void Save(IDataWriter writer, string name, byte value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, sbyte value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, int value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, uint value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, short value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, ushort value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, long value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, ulong value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, float value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, double value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, char value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void Save(IDataWriter writer, string name, bool value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void SaveEnum<T>(IDataWriter writer, string name, T value) where T : struct, IComparable, IFormattable, IConvertible
    {
      writer.WriteSimple(name, value.ToString());
    }

    public static void Save(IDataWriter writer, string name, Guid value)
    {
      writer.WriteSimple(name, DefaultConverter.ToString(value));
    }

    public static void SaveDynamicSerializable(
      IDataWriter writer,
      string name,
      ISerializeStateSave value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      value.StateSave(writer);
      writer.End(name, true);
    }

    public static void SaveSerializable(
      IDataWriter writer,
      string name,
      IVMStringSerializable value)
    {
      if (value == null)
        return;
      string str = value.Write();
      Save(writer, name, str);
    }

    public static void SaveList(IDataWriter writer, string name, List<string> value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      for (int index = 0; index < value.Count; ++index)
        Save(writer, "Item", value[index]);
      writer.End(name, true);
    }

    public static void SaveList(IDataWriter writer, string name, Stack<int> value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      foreach (int num in value)
        Save(writer, "Item", num);
      writer.End(name, true);
    }

    public static void SaveList(IDataWriter writer, string name, List<ulong> value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      for (int index = 0; index < value.Count; ++index)
        Save(writer, "Item", value[index]);
      writer.End(name, true);
    }

    public static void SaveList(IDataWriter writer, string name, IEnumerable<ulong> value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      foreach (ulong num in value)
        Save(writer, "Item", num);
      writer.End(name, true);
    }

    public static void SaveDynamicSerializableList<T>(
      IDataWriter writer,
      string name,
      List<T> value)
      where T : class
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      for (int index = 0; index < value.Count; ++index)
      {
        T obj = value[index];
        if (obj != null)
        {
          if (obj is ISerializeStateSave serializeStateSave)
          {
            if (!(serializeStateSave is INeedSave needSave) || needSave.NeedSave)
            {
              writer.Begin("Item", null, true);
              serializeStateSave.StateSave(writer);
              writer.End("Item", true);
            }
          }
          else
            Logger.AddError("Type : " + obj.GetType().Name + " is not ISerializeStateSave");
        }
        else
          Logger.AddError("Item is null");
      }
      writer.End(name, true);
    }

    public static void SaveDynamicSerializableList<T>(
      IDataWriter writer,
      string name,
      IEnumerable<T> value)
      where T : class
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      foreach (T obj in value)
      {
        if (obj != null)
        {
          if (obj is ISerializeStateSave serializeStateSave)
          {
            if (name == "HierarchyObjects" && typeof (VMBaseEntity).IsAssignableFrom(serializeStateSave.GetType()))
            {
              VMBaseEntity vmBaseEntity = (VMBaseEntity) serializeStateSave;
              if (!vmBaseEntity.IsHierarchy && !vmBaseEntity.IsEngineRoot)
                EngineAPIManager.Instance.SetFatalError(string.Format("UNUSUAL HIERARCHY SYSTEM ERROR: INCORRECT OBJECT {0} WORLD ADDING !!!", vmBaseEntity.Name));
            }
            if (!(serializeStateSave is INeedSave needSave) || needSave.NeedSave)
            {
              writer.Begin("Item", null, true);
              serializeStateSave.StateSave(writer);
              writer.End("Item", true);
            }
          }
          else
            Logger.AddError("Type : " + obj.GetType().Name + " is not ISerializeStateSave");
        }
        else
          Logger.AddError("Item is null");
      }
      writer.End(name, true);
    }

    public static void SaveDictionary(
      IDataWriter writer,
      string name,
      Dictionary<ulong, int> value)
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      foreach (KeyValuePair<ulong, int> keyValuePair in value)
      {
        writer.Begin("Item", null, true);
        Save(writer, "Key", keyValuePair.Key);
        Save(writer, "Value", keyValuePair.Value);
        writer.End("Item", true);
      }
      writer.End(name, true);
    }

    public static void SaveDictionaryCommon<T>(
      IDataWriter writer,
      string name,
      Dictionary<string, T> value)
      where T : class
    {
      if (value == null)
        return;
      writer.Begin(name, null, true);
      foreach (KeyValuePair<string, T> keyValuePair in value)
      {
        writer.Begin("Item", null, true);
        Save(writer, "Key", keyValuePair.Key);
        SaveCommon(writer, "Value", keyValuePair.Value);
        writer.End("Item", true);
      }
      writer.End(name, true);
    }

    public static void SaveCommon(IDataWriter writer, string name, object value)
    {
      switch (value)
      {
        case null:
          break;
        case ISerializeStateSave serializeStateSave:
          SaveDynamicSerializable(writer, name, serializeStateSave);
          break;
        case IVMStringSerializable stringSerializable:
          SaveSerializable(writer, name, stringSerializable);
          break;
        default:
          string str = StringSerializer.WriteValue(value);
          Save(writer, name, str);
          break;
      }
    }
  }
}
