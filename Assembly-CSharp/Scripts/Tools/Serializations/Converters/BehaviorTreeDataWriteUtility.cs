using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters
{
  public static class BehaviorTreeDataWriteUtility
  {
    public static void WriteShared<T>(IDataWriter writer, string name, T value, bool common = false) where T : SharedVariable
    {
      if ((object) value == null)
        return;
      if (value is ISerializeDataWrite serializeDataWrite)
      {
        System.Type type = ProxyFactory.GetType(value.GetType());
        if (value.IsShared && !common)
        {
          writer.Begin(name, type != typeof (T) ? type : (System.Type) null, true);
          writer.WriteSimple("IsShared", DefaultConverter.ToString(value.IsShared));
          writer.WriteSimple("Name", value.Name);
          writer.End(name, true);
        }
        else
        {
          writer.Begin(name, type != typeof (T) ? type : (System.Type) null, true);
          serializeDataWrite.DataWrite(writer);
          writer.End(name, true);
        }
      }
      else
        Debug.LogError((object) ("Type : " + value.GetType().Name + " is not " + typeof (ISerializeDataWrite).Name));
    }

    public static void WriteCommonSharedList<T>(IDataWriter writer, string name, List<T> value) where T : SharedVariable
    {
      BehaviorTreeDataWriteUtility.WriteSharedList<T>(writer, name, value, true);
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
      System.Type type = typeof (T);
      writer.Begin(name, (System.Type) null, true);
      foreach (T obj in value)
        BehaviorTreeDataWriteUtility.WriteShared<T>(writer, "Item", obj, common);
      writer.End(name, true);
    }

    public static void WriteSharedArray<T>(IDataWriter writer, string name, T[] value) where T : SharedVariable
    {
      if (value == null)
        return;
      System.Type type = typeof (T);
      writer.Begin(name, (System.Type) null, true);
      foreach (T obj in value)
        BehaviorTreeDataWriteUtility.WriteShared<T>(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteTask<T>(IDataWriter writer, string name, T value) where T : Task
    {
      DefaultDataWriteUtility.WriteSerialize<T>(writer, name, value);
    }

    public static void WriteTaskReference<T>(IDataWriter writer, string name, T value) where T : Task
    {
      if ((object) value != null)
      {
        if (value.Id == -1)
          Debug.LogError((object) ("Node is not deserialized, name : " + name));
        writer.Begin(name, value.GetType(), false);
        writer.Write(DefaultConverter.ToString(value.Id));
        writer.End(name, false);
      }
      else
      {
        writer.Begin(name, (System.Type) null, false);
        writer.Write(DefaultConverter.ToString(-1));
        writer.End(name, false);
      }
    }

    public static void WriteTaskList<T>(IDataWriter writer, string name, List<T> value) where T : Task
    {
      if (value == null)
        value = new List<T>();
      System.Type type = typeof (T);
      writer.Begin(name, (System.Type) null, true);
      foreach (T obj in value)
        BehaviorTreeDataWriteUtility.WriteTask<T>(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteUnity<T>(IDataWriter writer, string name, T value) where T : UnityEngine.Object
    {
      if ((UnityEngine.Object) value != (UnityEngine.Object) null)
      {
        int objectIndex = BehaviorTreeDataContext.GetObjectIndex((UnityEngine.Object) value, BehaviorTreeDataContext.ContextUnityObjects);
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

    public static void WriteUnityList<T>(IDataWriter writer, string name, List<T> value) where T : UnityEngine.Object
    {
      if (value == null)
        return;
      System.Type type = typeof (T);
      writer.Begin(name, (System.Type) null, true);
      foreach (T obj in value)
        BehaviorTreeDataWriteUtility.WriteUnity<T>(writer, "Item", obj);
      writer.End(name, true);
    }

    public static void WriteUnityArray<T>(IDataWriter writer, string name, T[] value) where T : UnityEngine.Object
    {
      if (value == null)
        return;
      System.Type type = typeof (T);
      writer.Begin(name, (System.Type) null, true);
      foreach (T obj in value)
        BehaviorTreeDataWriteUtility.WriteUnity<T>(writer, "Item", obj);
      writer.End(name, true);
    }
  }
}
