// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.TaskUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public static class TaskUtility
  {
    private static Dictionary<string, Type> typeLookup = new Dictionary<string, Type>();
    private static List<string> loadedAssemblies = (List<string>) null;
    private static Dictionary<Type, FieldInfo[]> allFieldsLookup = new Dictionary<Type, FieldInfo[]>();
    private static Dictionary<Type, FieldInfo[]> publicFieldsLookup = new Dictionary<Type, FieldInfo[]>();
    private static Dictionary<FieldInfo, Dictionary<Type, bool>> hasFieldLookup = new Dictionary<FieldInfo, Dictionary<Type, bool>>();
    private static List<FieldInfo> tmp = new List<FieldInfo>();

    public static object CreateInstance(Type t)
    {
      if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Nullable<>))
        t = Nullable.GetUnderlyingType(t);
      return Activator.CreateInstance(t, true);
    }

    public static FieldInfo[] GetAllFields(Type t)
    {
      FieldInfo[] allFields = (FieldInfo[]) null;
      if (!TaskUtility.allFieldsLookup.TryGetValue(t, out allFields))
      {
        TaskUtility.tmp.Clear();
        BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        TaskUtility.GetFields(t, ref TaskUtility.tmp, (int) flags);
        allFields = TaskUtility.tmp.ToArray();
        TaskUtility.allFieldsLookup.Add(t, allFields);
      }
      return allFields;
    }

    public static FieldInfo[] GetPublicFields(Type t)
    {
      FieldInfo[] publicFields = (FieldInfo[]) null;
      if (!TaskUtility.publicFieldsLookup.TryGetValue(t, out publicFields))
      {
        TaskUtility.tmp.Clear();
        BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        TaskUtility.GetFields(t, ref TaskUtility.tmp, (int) flags);
        publicFields = TaskUtility.tmp.ToArray();
        TaskUtility.publicFieldsLookup.Add(t, publicFields);
      }
      return publicFields;
    }

    private static void GetFields(Type t, ref List<FieldInfo> fieldList, int flags)
    {
      if (t == (Type) null || t.Equals(typeof (ParentTask)) || t.Equals(typeof (Task)) || t.Equals(typeof (SharedVariable)))
        return;
      foreach (FieldInfo field in t.GetFields((BindingFlags) flags))
        fieldList.Add(field);
      TaskUtility.GetFields(t.BaseType, ref fieldList, flags);
    }

    public static Type GetTypeWithinAssembly(string typeName)
    {
      Type typeWithinAssembly;
      if (TaskUtility.typeLookup.TryGetValue(typeName, out typeWithinAssembly))
        return typeWithinAssembly;
      Type type = Type.GetType(typeName);
      if (type == (Type) null)
      {
        if (TaskUtility.loadedAssemblies == null)
        {
          TaskUtility.loadedAssemblies = new List<string>();
          foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            TaskUtility.loadedAssemblies.Add(assembly.FullName);
        }
        for (int index = 0; index < TaskUtility.loadedAssemblies.Count; ++index)
        {
          type = Type.GetType(typeName + "," + TaskUtility.loadedAssemblies[index]);
          if (type != (Type) null)
            break;
        }
      }
      if (type != (Type) null)
        TaskUtility.typeLookup.Add(typeName, type);
      return type;
    }

    public static bool CompareType(Type t, string typeName)
    {
      Type type = Type.GetType(typeName + ", Assembly-CSharp");
      if (type == (Type) null)
        type = Type.GetType(typeName + ", Assembly-CSharp-firstpass");
      return t.Equals(type);
    }

    public static bool HasAttribute(FieldInfo field, Type attribute)
    {
      if (field == (FieldInfo) null)
        return false;
      Dictionary<Type, bool> dictionary;
      if (!TaskUtility.hasFieldLookup.TryGetValue(field, out dictionary))
      {
        dictionary = new Dictionary<Type, bool>();
        TaskUtility.hasFieldLookup.Add(field, dictionary);
      }
      bool flag;
      if (!dictionary.TryGetValue(attribute, out flag))
      {
        flag = field.GetCustomAttributes(attribute, false).Length != 0;
        dictionary.Add(attribute, flag);
      }
      return flag;
    }
  }
}
