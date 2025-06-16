using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters
{
  public static class BehaviorTreeDataReadUtility
  {
    private static T ReadShared<T>(IDataReader reader) where T : SharedVariable
    {
      if (DefaultConverter.ParseBool(reader.GetChild("IsShared").Read()))
        return BehaviorTreeDataReadUtility.GetOrCreateVariable<T>(reader);
      System.Type realType = MappingUtility.GetRealType(reader, typeof (T));
      T obj = (T) ProxyFactory.Create(realType);
      if (!(obj is ISerializeDataRead serializeDataRead))
      {
        Debug.LogError((object) ("Type : " + obj.GetType().Name + " is not " + (object) typeof (ISerializeDataRead)));
        return default (T);
      }
      serializeDataRead.DataRead(reader, realType);
      return obj;
    }

    public static T ReadShared<T>(IDataReader reader, string name, T value) where T : SharedVariable
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? default (T) : BehaviorTreeDataReadUtility.ReadShared<T>(child);
    }

    public static List<T> ReadCommonSharedList<T>(IDataReader reader, string name, List<T> value) where T : SharedVariable
    {
      if (value == null)
        value = new List<T>();
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T variable = BehaviorTreeDataReadUtility.GetOrCreateVariable<T>(child2);
        if (!(variable is ISerializeDataRead serializeDataRead))
        {
          Debug.LogError((object) ("Type : " + variable.GetType().Name + " is not " + (object) typeof (ISerializeDataRead)));
        }
        else
        {
          System.Type realType = MappingUtility.GetRealType(child2, typeof (T));
          serializeDataRead.DataRead(child2, realType);
          value.Add(variable);
        }
      }
      return value;
    }

    public static T GetOrCreateVariable<T>(IDataReader reader) where T : SharedVariable
    {
      string key = reader.GetChild("Name").Read();
      SharedVariable variable;
      if (!BehaviorTreeDataContext.Variables.TryGetValue(key, out variable))
      {
        variable = (SharedVariable) ProxyFactory.Create(MappingUtility.GetRealType(reader, typeof (T)));
        BehaviorTreeDataContext.Variables.Add(key, variable);
      }
      return (T) variable;
    }

    public static List<T> ReadSharedList<T>(IDataReader reader, string name, List<T> value) where T : SharedVariable
    {
      if (value == null)
        value = new List<T>();
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = BehaviorTreeDataReadUtility.ReadShared<T>(child2);
        value.Add(obj);
      }
      return value;
    }

    public static T[] ReadSharedArray<T>(IDataReader reader, string name, T[] value) where T : SharedVariable
    {
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return Array.Empty<T>();
      List<T> objList = new List<T>();
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = BehaviorTreeDataReadUtility.ReadShared<T>(child2);
        objList.Add(obj);
      }
      return objList.ToArray();
    }

    public static T ReadTask<T>(IDataReader reader, string name, T value) where T : Task
    {
      return BehaviorTreeDataReadUtility.ReadTaskSerialize<T>(reader, name);
    }

    public static T ReadTaskReference<T>(IDataReader reader, string name, T value) where T : Task
    {
      IDataReader child = reader.GetChild(name);
      if (child == null)
        return default (T);
      System.Type realType = MappingUtility.GetRealType(child, typeof (T));
      int id = DefaultConverter.ParseInt(child.Read());
      return id == -1 ? default (T) : BehaviorTreeDataReadUtility.GetOrCreateNode<T>(realType, id);
    }

    public static List<T> ReadTaskList<T>(IDataReader reader, string name, List<T> value) where T : Task
    {
      if (value == null)
        value = new List<T>();
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = BehaviorTreeDataReadUtility.ReadTaskSerialize<T>(child2);
        value.Add(obj);
      }
      return value;
    }

    public static T ReadTaskSerialize<T>(IDataReader reader) where T : Task
    {
      int id = DefaultConverter.ParseInt(reader.GetChild("Id").Read());
      System.Type realType = MappingUtility.GetRealType(reader, typeof (T));
      T node = BehaviorTreeDataReadUtility.GetOrCreateNode<T>(realType, id);
      if (!(node is ISerializeDataRead serializeDataRead))
      {
        Debug.LogError((object) ("Type : " + node.GetType().Name + " is not " + (object) typeof (ISerializeDataRead)));
        return default (T);
      }
      serializeDataRead.DataRead(reader, realType);
      return node;
    }

    public static T ReadTaskSerialize<T>(IDataReader reader, string name) where T : Task
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? default (T) : BehaviorTreeDataReadUtility.ReadTaskSerialize<T>(child);
    }

    private static T GetOrCreateNode<T>(System.Type realType, int id) where T : Task
    {
      Task task;
      if (!BehaviorTreeDataContext.Tasks.TryGetValue(id, out task))
      {
        object obj = ProxyFactory.Create(realType);
        if (obj == null)
        {
          Debug.LogError((object) ("Instance is null, type : " + (object) realType + " , id : " + (object) id));
          return default (T);
        }
        task = (Task) (obj as T);
        if (task == null)
        {
          Debug.LogError((object) ("Error cast type : " + (object) obj.GetType() + " , to type : " + (object) typeof (T) + " , id : " + (object) id));
          return default (T);
        }
        BehaviorTreeDataContext.Tasks.Add(id, task);
      }
      if (task == null)
      {
        Debug.LogError((object) ("Result is null, id : " + (object) id));
        return default (T);
      }
      if (task is T node)
        return node;
      Debug.LogError((object) ("Error cast type : " + (object) task.GetType() + " , to type : " + (object) typeof (T) + " , id : " + (object) id));
      return default (T);
    }

    public static T ReadUnity<T>(IDataReader reader, string name, T value) where T : UnityEngine.Object
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? default (T) : BehaviorTreeDataReadUtility.ReadUnity<T>(child);
    }

    public static T ReadUnity<T>(IDataReader reader) where T : UnityEngine.Object
    {
      string str = reader.Read();
      if (str == null)
        return default (T);
      int index = DefaultConverter.ParseInt(str);
      if (index == -1)
        return default (T);
      if (index < 0 || index >= BehaviorTreeDataContext.ContextUnityObjects.Count)
      {
        Debug.LogError((object) ("Index not found : " + (object) index));
        return default (T);
      }
      UnityEngine.Object contextUnityObject = BehaviorTreeDataContext.ContextUnityObjects[index];
      T obj = contextUnityObject as T;
      if ((UnityEngine.Object) obj == (UnityEngine.Object) null)
        Debug.LogError((object) ("Error cast type : " + ((object) contextUnityObject).GetType().FullName + " , to type : " + (object) typeof (T)));
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

    public static List<T> ReadUnityList<T>(IDataReader reader, string name, List<T> value) where T : UnityEngine.Object
    {
      if (value == null)
        value = new List<T>();
      else
        value.Clear();
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return value;
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = BehaviorTreeDataReadUtility.ReadUnity<T>(child2);
        value.Add(obj);
      }
      return value;
    }

    public static T[] ReadUnityArray<T>(IDataReader reader, string name, T[] value) where T : UnityEngine.Object
    {
      IDataReader child1 = reader.GetChild(name);
      if (child1 == null)
        return Array.Empty<T>();
      List<T> objList = new List<T>();
      foreach (IDataReader child2 in child1.GetChilds())
      {
        T obj = BehaviorTreeDataReadUtility.ReadUnity<T>(child2);
        objList.Add(obj);
      }
      return objList.ToArray();
    }
  }
}
