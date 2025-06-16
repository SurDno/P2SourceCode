using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ParadoxNotion
{
  public static class ReflectionTools
  {
    private const BindingFlags flagsEverything = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
    public static object ContextObject;
    private static Dictionary<string, System.Type> typeMap = new Dictionary<string, System.Type>();
    private static Dictionary<System.Type, FieldInfo[]> _typeFields = new Dictionary<System.Type, FieldInfo[]>();

    public static System.Type GetType(
      string typeFullName,
      bool fallbackNoNamespace = false,
      System.Type fallbackAssignable = null)
    {
      if (string.IsNullOrEmpty(typeFullName))
        return (System.Type) null;
      System.Type type1 = (System.Type) null;
      if (ReflectionTools.typeMap.TryGetValue(typeFullName, out type1))
        return type1;
      System.Type typeDirect = ReflectionTools.GetTypeDirect(typeFullName);
      if (typeDirect != (System.Type) null)
        return ReflectionTools.typeMap[typeFullName] = typeDirect;
      System.Type type2 = ReflectionTools.TryResolveGenericType(typeFullName, fallbackNoNamespace, fallbackAssignable);
      if (type2 != (System.Type) null)
        return ReflectionTools.typeMap[typeFullName] = type2;
      if (fallbackNoNamespace)
      {
        System.Type type3 = ReflectionTools.TryResolveWithoutNamespace(typeFullName, fallbackAssignable);
        if (type3 != (System.Type) null)
          return ReflectionTools.typeMap[type3.FullName] = type3;
      }
      ReflectionTools.LateLog((object) string.Format("<b>(Type Request)</b> Type with name '{0}' could not be resolved.", (object) typeFullName), LogType.Error);
      return ReflectionTools.typeMap[typeFullName] = (System.Type) null;
    }

    public static string GetContext()
    {
      string str = "";
      object contextObject = ReflectionTools.ContextObject;
      string context;
      if (contextObject != null)
      {
        UnityEngine.Object @object = contextObject as UnityEngine.Object;
        context = !(@object != (UnityEngine.Object) null) ? str + "context object : " + contextObject.ToString() : str + "context unity object : " + @object.GetInfo();
      }
      else
        context = str + "context not found";
      return context;
    }

    private static void LateLog(object logMessage, LogType logType = LogType.Log)
    {
      string message = logMessage.ToString() + " , " + ReflectionTools.GetContext();
      Debug.unityLogger.Log(logType, (object) message);
    }

    private static System.Type GetTypeDirect(string typeFullName)
    {
      System.Type type1 = System.Type.GetType(typeFullName);
      if (type1 != (System.Type) null)
        return type1;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        System.Type type2;
        try
        {
          type2 = assembly.GetType(typeFullName);
        }
        catch
        {
          continue;
        }
        if (type2 != (System.Type) null)
          return type2;
      }
      return (System.Type) null;
    }

    private static System.Type TryResolveGenericType(
      string typeFullName,
      bool fallbackNoNamespace = false,
      System.Type fallbackAssignable = null)
    {
      if (!typeFullName.Contains<char>('`') || !typeFullName.Contains<char>('['))
        return (System.Type) null;
      try
      {
        int num1 = typeFullName.IndexOf('`');
        System.Type type1 = ReflectionTools.GetType(typeFullName.Substring(0, num1 + 2), fallbackNoNamespace, fallbackAssignable);
        if (type1 == (System.Type) null)
          return (System.Type) null;
        int int32 = Convert.ToInt32(typeFullName.Substring(num1 + 1, 1));
        string[] array;
        if (typeFullName.Substring(num1 + 2, typeFullName.Length - num1 - 2).StartsWith("[["))
        {
          int startIndex = typeFullName.IndexOf("[[") + 2;
          int num2 = typeFullName.LastIndexOf("]]");
          array = ((IEnumerable<string>) typeFullName.Substring(startIndex, num2 - startIndex).Split(new string[1]
          {
            "],["
          }, int32, StringSplitOptions.RemoveEmptyEntries)).ToArray<string>();
        }
        else
        {
          int startIndex = typeFullName.IndexOf('[') + 1;
          int num3 = typeFullName.LastIndexOf(']');
          array = ((IEnumerable<string>) typeFullName.Substring(startIndex, num3 - startIndex).Split(new char[1]
          {
            ','
          }, int32, StringSplitOptions.RemoveEmptyEntries)).ToArray<string>();
        }
        System.Type[] typeArgs = new System.Type[int32];
        for (int index = 0; index < array.Length; ++index)
        {
          string str = array[index];
          if (!str.Contains<char>('`') && str.Contains<char>(','))
            str = str.Substring(0, str.IndexOf(','));
          System.Type fallbackAssignable1 = (System.Type) null;
          if (fallbackNoNamespace)
          {
            System.Type[] parameterConstraints = type1.RTGetGenericArguments()[index].GetGenericParameterConstraints();
            fallbackAssignable1 = parameterConstraints.Length == 0 ? typeof (object) : parameterConstraints[0];
          }
          System.Type type2 = ReflectionTools.GetType(str, fallbackNoNamespace, fallbackAssignable1);
          if (type2 == (System.Type) null)
            return (System.Type) null;
          typeArgs[index] = type2;
        }
        return type1.RTMakeGenericType(typeArgs);
      }
      catch (Exception ex)
      {
        ReflectionTools.LateLog((object) ("<b>(Type Request)</b> BUG (Please report this): " + ex.Message), LogType.Error);
        return (System.Type) null;
      }
    }

    private static System.Type TryResolveWithoutNamespace(string typeName, System.Type fallbackAssignable = null)
    {
      if (typeName.Contains<char>('`') && typeName.Contains<char>('['))
        return (System.Type) null;
      if (typeName.Contains<char>(','))
        typeName = typeName.Substring(0, typeName.IndexOf(','));
      if (typeName.Contains<char>('.'))
      {
        int startIndex = typeName.LastIndexOf('.') + 1;
        typeName = typeName.Substring(startIndex, typeName.Length - startIndex);
      }
      return (System.Type) null;
    }

    private static System.Type[] RTGetExportedTypes(this Assembly asm) => asm.GetExportedTypes();

    public static string FriendlyName(this System.Type t, bool trueSignature = false)
    {
      if (t == (System.Type) null)
        return (string) null;
      if (!trueSignature && t == typeof (UnityEngine.Object))
        return "UnityObject";
      string str1 = trueSignature ? t.FullName : t.Name;
      if (!trueSignature)
      {
        if (str1 == "Single")
          str1 = "Float";
        if (str1 == "Int32")
          str1 = "Integer";
      }
      if (!trueSignature && str1.EndsWith("Node"))
        str1 = str1.Substring(0, str1.Length - 4);
      if (t.RTIsGenericParameter())
        str1 = "T";
      if (t.RTIsGenericType())
      {
        str1 = (trueSignature ? t.FullName : t.Name) ?? t.Namespace + "." + t.Name;
        System.Type[] genericArguments = t.RTGetGenericArguments();
        if (genericArguments.Length != 0)
        {
          string str2 = str1.Replace("`" + genericArguments.Length.ToString(), "") + "<";
          for (int index = 0; index < genericArguments.Length; ++index)
            str2 = str2 + (index == 0 ? "" : ", ") + genericArguments[index].FriendlyName(trueSignature);
          str1 = str2 + ">";
        }
      }
      return str1;
    }

    public static string SignatureName(this MethodInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      string str = (method.IsStatic ? "static " : "") + method.Name + " (";
      for (int index = 0; index < parameters.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters[index];
        str = str + (parameterInfo.ParameterType.IsByRef ? (parameterInfo.IsOut ? "out " : "ref ") : "") + parameterInfo.ParameterType.FriendlyName() + (index < parameters.Length - 1 ? ", " : "");
      }
      return str + ") : " + method.ReturnType.FriendlyName();
    }

    public static System.Type RTReflectedType(this System.Type type) => type.ReflectedType;

    public static System.Type RTReflectedType(this MemberInfo member) => member.ReflectedType;

    public static bool RTIsAssignableFrom(this System.Type type, System.Type second)
    {
      return type.IsAssignableFrom(second);
    }

    public static bool RTIsAbstract(this System.Type type) => type.IsAbstract;

    public static bool RTIsValueType(this System.Type type) => type.IsValueType;

    public static bool RTIsArray(this System.Type type) => type.IsArray;

    public static bool RTIsInterface(this System.Type type) => type.IsInterface;

    public static bool RTIsSubclassOf(this System.Type type, System.Type other)
    {
      return type.IsSubclassOf(other);
    }

    public static bool RTIsGenericParameter(this System.Type type) => type.IsGenericParameter;

    public static bool RTIsGenericType(this System.Type type) => type.IsGenericType;

    public static MethodInfo RTGetGetMethod(this PropertyInfo prop) => prop.GetGetMethod();

    public static MethodInfo RTGetSetMethod(this PropertyInfo prop) => prop.GetSetMethod();

    public static FieldInfo RTGetField(this System.Type type, string name)
    {
      return type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }

    public static PropertyInfo RTGetProperty(this System.Type type, string name)
    {
      return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }

    public static MethodInfo RTGetMethod(this System.Type type, string name)
    {
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }

    public static MethodInfo RTGetMethod(this System.Type type, string name, System.Type[] paramTypes)
    {
      return type.GetMethod(name, paramTypes);
    }

    public static EventInfo RTGetEvent(this System.Type type, string name)
    {
      return type.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }

    public static MethodInfo RTGetDelegateMethodInfo(this Delegate del) => del.Method;

    public static FieldInfo[] RTGetFields(this System.Type type)
    {
      FieldInfo[] fields;
      if (!ReflectionTools._typeFields.TryGetValue(type, out fields))
      {
        fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        ReflectionTools._typeFields[type] = fields;
      }
      return fields;
    }

    public static PropertyInfo[] RTGetProperties(this System.Type type)
    {
      return type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }

    public static MethodInfo[] RTGetMethods(this System.Type type)
    {
      return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }

    public static T RTGetAttribute<T>(this System.Type type, bool inherited) where T : Attribute
    {
      return (T) ((IEnumerable<object>) type.GetCustomAttributes(typeof (T), inherited)).FirstOrDefault<object>();
    }

    public static T RTGetAttribute<T>(this MemberInfo member, bool inherited) where T : Attribute
    {
      return (T) ((IEnumerable<object>) member.GetCustomAttributes(typeof (T), inherited)).FirstOrDefault<object>();
    }

    public static System.Type RTMakeGenericType(this System.Type type, System.Type[] typeArgs)
    {
      return type.MakeGenericType(typeArgs);
    }

    public static System.Type[] RTGetGenericArguments(this System.Type type)
    {
      return type.GetGenericArguments();
    }

    public static System.Type[] RTGetEmptyTypes() => System.Type.EmptyTypes;

    public static T RTCreateDelegate<T>(this MethodInfo method, object instance)
    {
      return (T) method.RTCreateDelegate(typeof (T), instance);
    }

    public static Delegate RTCreateDelegate(this MethodInfo method, System.Type type, object instance)
    {
      return Delegate.CreateDelegate(type, instance, method);
    }

    public static bool IsObsolete(this MemberInfo member)
    {
      if (member is MethodInfo)
      {
        MethodInfo methodInfo = (MethodInfo) member;
        if (methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_"))
          member = (MemberInfo) methodInfo.DeclaringType.RTGetProperty(methodInfo.Name.Replace("get_", "").Replace("set_", ""));
      }
      return member.RTGetAttribute<ObsoleteAttribute>(true) != null;
    }

    public static bool IsReadOnly(this FieldInfo field) => field.IsInitOnly || field.IsLiteral;

    public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo)
    {
      MethodInfo accessor = propertyInfo.GetAccessors(true)[0];
      if (accessor == (MethodInfo) null)
        return (PropertyInfo) null;
      MethodInfo baseDefinition = accessor.GetBaseDefinition();
      if (baseDefinition == accessor)
        return propertyInfo;
      System.Type[] array = ((IEnumerable<ParameterInfo>) propertyInfo.GetIndexParameters()).Select<ParameterInfo, System.Type>((Func<ParameterInfo, System.Type>) (p => p.ParameterType)).ToArray<System.Type>();
      return baseDefinition.DeclaringType.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, (Binder) null, propertyInfo.PropertyType, array, (ParameterModifier[]) null);
    }

    public static FieldInfo GetBaseDefinition(this FieldInfo fieldInfo)
    {
      return fieldInfo.DeclaringType.RTGetField(fieldInfo.Name);
    }

    public static System.Type GetEnumerableElementType(this System.Type enumType)
    {
      if (enumType == (System.Type) null || !typeof (IEnumerable).IsAssignableFrom(enumType))
        return (System.Type) null;
      if (enumType.RTIsArray())
        return enumType.GetElementType();
      foreach (System.Type type in enumType.GetInterfaces())
      {
        if (type.RTIsGenericType() && !(type.GetGenericTypeDefinition() != typeof (IEnumerable<>)))
          return type.RTGetGenericArguments()[0];
      }
      return (System.Type) null;
    }
  }
}
