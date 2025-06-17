using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Tools.Serializations.Converters
{
  public static class BehaviorTreeDataWriteUtility
  {
    public static void WriteShared<T>(IDataWriter writer, string name, T value, bool common = false) where T : SharedVariable
    {
      if (value == null)
        return;
      if (value is ISerializeDataWrite serializeDataWrite)
      {
        Type type = ProxyFactory.GetType(value.GetType());
        if (value.IsShared && !common)
        {
          writer.Begin(name, type != typeof (T) ? type : null, true);
          writer.WriteSimple("IsShared", DefaultConverter.ToString(value.IsShared));
          writer.WriteSimple("Name", value.Name);
          writer.End(name, true);
        }
        else
        {
          writer.Begin(name, type != typeof (T) ? type : null, true);
          serializeDataWrite.DataWrite(writer);
          writer.End(name, true);
        }
      }
      else
        Debug.LogError("Type : " + value.GetType().Name + " is not " + typeof (ISerializeDataWrite).Name);
    }

    public static void WriteCommonSharedList<T>(IDataWriter writer, string name, List<T> value) where T : SharedVariable
    {
      WriteSharedList(writer, name, value, true);
    }

    public static void WriteSharedList<T>(
      IDataWriter writer,
      string name,
      List<T> value,
      bool common = false)
      where T : SharedVariable
    {
      if (value == null)
        return;
      Type type = typeof (T);
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteShared(writer, "Item", obj, common);
      writer.End(name, true);
    }

    public static void WriteSharedArray<T>(IDataWriter writer, string name, T[] value) where T : SharedVariable
    {
      if (value == null)
        return;
      Type type = typeof (T);
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteShared(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteTask<T>(IDataWriter writer, string name, T value) where T : Task
    {
      DefaultDataWriteUtility.WriteSerialize(writer, name, value);
    }

    public static void WriteTaskReference<T>(IDataWriter writer, string name, T value) where T : Task
    {
      if (value != null)
      {
        if (value.Id == -1)
          Debug.LogError("Node is not deserialized, name : " + name);
        writer.Begin(name, value.GetType(), false);
        writer.Write(DefaultConverter.ToString(value.Id));
        writer.End(name, false);
      }
      else
      {
        writer.Begin(name, null, false);
        writer.Write(DefaultConverter.ToString(-1));
        writer.End(name, false);
      }
    }

    public static void WriteTaskList<T>(IDataWriter writer, string name, List<T> value) where T : Task
    {
      if (value == null)
        value = [];
      Type type = typeof (T);
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteTask(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteUnity<T>(IDataWriter writer, string name, T value) where T : Object
    {
      if (value != null)
      {
        int objectIndex = BehaviorTreeDataContext.GetObjectIndex(value, BehaviorTreeDataContext.ContextUnityObjects);
        DefaultDataWriteUtility.Write(writer, name, objectIndex);
      }
      else
        DefaultDataWriteUtility.Write(writer, name, -1);
    }

    public static void WriteUnity(IDataWriter writer, string name, LayerMask value)
    {
      UnityDataWriteUtility.Write(writer, name, value);
    }

    public static void WriteUnity(IDataWriter writer, string name, Quaternion value)
    {
      UnityDataWriteUtility.Write(writer, name, value);
    }

    public static void WriteUnity(IDataWriter writer, string name, Vector3 value)
    {
      UnityDataWriteUtility.Write(writer, name, value);
    }

    public static void WriteUnity(IDataWriter writer, string name, Vector2 value)
    {
      UnityDataWriteUtility.Write(writer, name, value);
    }

    public static void WriteUnityList<T>(IDataWriter writer, string name, List<T> value) where T : Object
    {
      if (value == null)
        return;
      Type type = typeof (T);
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteUnity(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteUnityArray<T>(IDataWriter writer, string name, T[] value) where T : Object
    {
      if (value == null)
        return;
      Type type = typeof (T);
      writer.Begin(name, null, true);
      foreach (T obj in value)
        WriteUnity(writer, "Item", obj);
      writer.End(name, true);
    }
  }
}
