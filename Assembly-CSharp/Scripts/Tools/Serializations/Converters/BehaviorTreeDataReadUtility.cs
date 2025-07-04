﻿using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Tools.Serializations.Converters
{
  public static class BehaviorTreeDataReadUtility
  {
    private static T ReadShared<T>(IDataReader reader) where T : SharedVariable
    {
      if (DefaultConverter.ParseBool(reader.GetChild("IsShared").Read()))
        return GetOrCreateVariable<T>(reader);
      Type realType = MappingUtility.GetRealType(reader, typeof (T));
      T obj = (T) ProxyFactory.Create(realType);
      if (!(obj is ISerializeDataRead serializeDataRead))
      {
        Debug.LogError("Type : " + obj.GetType().Name + " is not " + typeof (ISerializeDataRead));
        return default (T);
      }
      serializeDataRead.DataRead(reader, realType);
      return obj;
    }

    public static T ReadShared<T>(IDataReader reader, string name, T value) where T : SharedVariable
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? default (T) : ReadShared<T>(child);
    }

    public static List<T> ReadCommonSharedList<T>(IDataReader reader, string name, List<T> value) where T : SharedVariable
    {
      if (value == null)
        value = [];
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T variable = GetOrCreateVariable<T>(child2);
        if (!(variable is ISerializeDataRead serializeDataRead))
        {
          Debug.LogError("Type : " + variable.GetType().Name + " is not " + typeof (ISerializeDataRead));
        }
        else
        {
          Type realType = MappingUtility.GetRealType(child2, typeof (T));
          serializeDataRead.DataRead(child2, realType);
          value.Add(variable);
        }
      }
      return value;
    }

    public static T GetOrCreateVariable<T>(IDataReader reader) where T : SharedVariable
    {
      string key = reader.GetChild("Name").Read();
      if (!BehaviorTreeDataContext.Variables.TryGetValue(key, out SharedVariable variable))
      {
        variable = (SharedVariable) ProxyFactory.Create(MappingUtility.GetRealType(reader, typeof (T)));
        BehaviorTreeDataContext.Variables.Add(key, variable);
      }
      return (T) variable;
    }

    public static List<T> ReadSharedList<T>(IDataReader reader, string name, List<T> value) where T : SharedVariable
    {
      if (value == null)
        value = [];
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = ReadShared<T>(child2);
        value.Add(obj);
      }
      return value;
    }

    public static T[] ReadSharedArray<T>(IDataReader reader, string name, T[] value) where T : SharedVariable
    {
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return [];
      List<T> objList = [];
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = ReadShared<T>(child2);
        objList.Add(obj);
      }
      return objList.ToArray();
    }

    public static T ReadTask<T>(IDataReader reader, string name, T value) where T : Task
    {
      return ReadTaskSerialize<T>(reader, name);
    }

    public static T ReadTaskReference<T>(IDataReader reader, string name, T value) where T : Task
    {
      IDataReader child = reader.GetChild(name);
      if (child == null)
        return default (T);
      Type realType = MappingUtility.GetRealType(child, typeof (T));
      int id = DefaultConverter.ParseInt(child.Read());
      return id == -1 ? default (T) : GetOrCreateNode<T>(realType, id);
    }

    public static List<T> ReadTaskList<T>(IDataReader reader, string name, List<T> value) where T : Task
    {
      if (value == null)
        value = [];
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = ReadTaskSerialize<T>(child2);
        value.Add(obj);
      }
      return value;
    }

    public static T ReadTaskSerialize<T>(IDataReader reader) where T : Task
    {
      int id = DefaultConverter.ParseInt(reader.GetChild("Id").Read());
      Type realType = MappingUtility.GetRealType(reader, typeof (T));
      T node = GetOrCreateNode<T>(realType, id);
      if (!(node is ISerializeDataRead serializeDataRead))
      {
        Debug.LogError("Type : " + node.GetType().Name + " is not " + typeof (ISerializeDataRead));
        return default (T);
      }
      serializeDataRead.DataRead(reader, realType);
      return node;
    }

    public static T ReadTaskSerialize<T>(IDataReader reader, string name) where T : Task
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? default (T) : ReadTaskSerialize<T>(child);
    }

    private static T GetOrCreateNode<T>(Type realType, int id) where T : Task
    {
      if (!BehaviorTreeDataContext.Tasks.TryGetValue(id, out Task task))
      {
        object obj = ProxyFactory.Create(realType);
        if (obj == null)
        {
          Debug.LogError("Instance is null, type : " + realType + " , id : " + id);
          return default (T);
        }
        task = obj as T;
        if (task == null)
        {
          Debug.LogError("Error cast type : " + obj.GetType() + " , to type : " + typeof (T) + " , id : " + id);
          return default (T);
        }
        BehaviorTreeDataContext.Tasks.Add(id, task);
      }
      if (task == null)
      {
        Debug.LogError("Result is null, id : " + id);
        return default (T);
      }
      if (task is T node)
        return node;
      Debug.LogError("Error cast type : " + task.GetType() + " , to type : " + typeof (T) + " , id : " + id);
      return default (T);
    }

    public static T ReadUnity<T>(IDataReader reader, string name, T value) where T : Object
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? default (T) : ReadUnity<T>(child);
    }

    public static T ReadUnity<T>(IDataReader reader) where T : Object
    {
      string str = reader.Read();
      if (str == null)
        return default (T);
      int index = DefaultConverter.ParseInt(str);
      if (index == -1)
        return default (T);
      if (index < 0 || index >= BehaviorTreeDataContext.ContextUnityObjects.Count)
      {
        Debug.LogError("Index not found : " + index);
        return default (T);
      }
      Object contextUnityObject = BehaviorTreeDataContext.ContextUnityObjects[index];
      T obj = contextUnityObject as T;
      if (obj == null)
        Debug.LogError("Error cast type : " + contextUnityObject.GetType().FullName + " , to type : " + typeof (T));
      return obj;
    }

    public static Quaternion ReadUnity(IDataReader reader, string name, Quaternion value)
    {
      return UnityDataReadUtility.Read(reader, name, value);
    }

    public static LayerMask ReadUnity(IDataReader reader, string name, LayerMask value)
    {
      return UnityDataReadUtility.Read(reader, name, value);
    }

    public static Vector2 ReadUnity(IDataReader reader, string name, Vector2 value)
    {
      return UnityDataReadUtility.Read(reader, name, value);
    }

    public static Vector3 ReadUnity(IDataReader reader, string name, Vector3 value)
    {
      return UnityDataReadUtility.Read(reader, name, value);
    }

    public static List<T> ReadUnityList<T>(IDataReader reader, string name, List<T> value) where T : Object
    {
      if (value == null)
        value = [];
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = ReadUnity<T>(child2);
        value.Add(obj);
      }
      return value;
    }

    public static T[] ReadUnityArray<T>(IDataReader reader, string name, T[] value) where T : Object
    {
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return [];
      List<T> objList = [];
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = ReadUnity<T>(child2);
        objList.Add(obj);
      }
      return objList.ToArray();
    }
  }
}
