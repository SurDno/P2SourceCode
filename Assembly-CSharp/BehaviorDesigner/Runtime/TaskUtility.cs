using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public static class TaskUtility
  {
    private static Dictionary<string, Type> typeLookup = new();
    private static List<string> loadedAssemblies;
    private static Dictionary<Type, FieldInfo[]> allFieldsLookup = new();
    private static Dictionary<Type, FieldInfo[]> publicFieldsLookup = new();
    private static Dictionary<FieldInfo, Dictionary<Type, bool>> hasFieldLookup = new();
    private static List<FieldInfo> tmp = [];

    public static object CreateInstance(Type t)
    {
      if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Nullable<>))
        t = Nullable.GetUnderlyingType(t);
      return Activator.CreateInstance(t, true);
    }

    public static FieldInfo[] GetAllFields(Type t)
    {
      if (!allFieldsLookup.TryGetValue(t, out FieldInfo[] allFields))
      {
        tmp.Clear();
        BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        GetFields(t, ref tmp, (int) flags);
        allFields = tmp.ToArray();
        allFieldsLookup.Add(t, allFields);
      }
      return allFields;
    }

    public static FieldInfo[] GetPublicFields(Type t)
    {
      if (!publicFieldsLookup.TryGetValue(t, out FieldInfo[] publicFields))
      {
        tmp.Clear();
        BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        GetFields(t, ref tmp, (int) flags);
        publicFields = tmp.ToArray();
        publicFieldsLookup.Add(t, publicFields);
      }
      return publicFields;
    }

    private static void GetFields(Type t, ref List<FieldInfo> fieldList, int flags)
    {
      if (t == null || t.Equals(typeof (ParentTask)) || t.Equals(typeof (Task)) || t.Equals(typeof (SharedVariable)))
        return;
      foreach (FieldInfo field in t.GetFields((BindingFlags) flags))
        fieldList.Add(field);
      GetFields(t.BaseType, ref fieldList, flags);
    }

    public static Type GetTypeWithinAssembly(string typeName)
    {
      if (typeLookup.TryGetValue(typeName, out Type typeWithinAssembly))
        return typeWithinAssembly;
      Type type = Type.GetType(typeName);
      if (type == null)
      {
        if (loadedAssemblies == null)
        {
          loadedAssemblies = [];
          foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            loadedAssemblies.Add(assembly.FullName);
        }
        for (int index = 0; index < loadedAssemblies.Count; ++index)
        {
          type = Type.GetType(typeName + "," + loadedAssemblies[index]);
          if (type != null)
            break;
        }
      }
      if (type != null)
        typeLookup.Add(typeName, type);
      return type;
    }

    public static bool CompareType(Type t, string typeName)
    {
      Type type = Type.GetType(typeName + ", Assembly-CSharp");
      if (type == null)
        type = Type.GetType(typeName + ", Assembly-CSharp-firstpass");
      return t.Equals(type);
    }

    public static bool HasAttribute(FieldInfo field, Type attribute)
    {
      if (field == null)
        return false;
      if (!hasFieldLookup.TryGetValue(field, out Dictionary<Type, bool> dictionary))
      {
        dictionary = new Dictionary<Type, bool>();
        hasFieldLookup.Add(field, dictionary);
      }

      if (!dictionary.TryGetValue(attribute, out bool flag))
      {
        flag = field.GetCustomAttributes(attribute, false).Length != 0;
        dictionary.Add(attribute, flag);
      }
      return flag;
    }
  }
}
