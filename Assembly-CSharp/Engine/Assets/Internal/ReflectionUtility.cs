using System;
using System.Reflection;
using UnityEngine;

namespace Engine.Assets.Internal
{
  public static class ReflectionUtility
  {
    public static void SetProperty(System.Type type, object target, string name, object value)
    {
      PropertyInfo property = ReflectionUtility.FindProperty(type, name);
      if (property != (PropertyInfo) null)
        property.SetValue(target, value, new object[0]);
      else
        throw new MissingMemberException(MethodBase.GetCurrentMethod().ToString() + " : " + (object) type + " : " + name);
    }

    public static object GetProperty(System.Type type, object target, string name)
    {
      PropertyInfo property = ReflectionUtility.FindProperty(type, name);
      return property != (PropertyInfo) null ? property.GetValue(target, new object[0]) : throw new MissingMemberException(MethodBase.GetCurrentMethod().ToString() + " : " + (object) type + " : " + name);
    }

    public static PropertyInfo FindProperty(System.Type type, string name)
    {
      for (; type != typeof (object); type = type.BaseType)
      {
        PropertyInfo property = type.GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != (PropertyInfo) null)
          return property;
      }
      return (PropertyInfo) null;
    }

    public static FieldInfo FindField(System.Type type, string name)
    {
      for (; type != typeof (object); type = type.BaseType)
      {
        FieldInfo field = type.GetField(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != (FieldInfo) null)
          return field;
      }
      return (FieldInfo) null;
    }

    public static void SetField(System.Type type, object target, string name, object value)
    {
      FieldInfo field = ReflectionUtility.FindField(type, name);
      if (field != (FieldInfo) null)
        field.SetValue(target, value);
      else
        throw new MissingMemberException(MethodBase.GetCurrentMethod().ToString() + " : " + (object) type + " : " + name);
    }

    public static object GetField(System.Type type, object target, string name)
    {
      FieldInfo field = ReflectionUtility.FindField(type, name);
      return field != (FieldInfo) null ? field.GetValue(target) : throw new MissingMemberException(MethodBase.GetCurrentMethod().ToString() + " : " + (object) type + " : " + name);
    }

    public static MethodInfo FindMethod(System.Type type, string name, params System.Type[] types)
    {
      for (; type != typeof (object); type = type.BaseType)
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (!(method.Name != name))
          {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == types.Length)
            {
              bool flag = true;
              for (int index = 0; index < parameters.Length; ++index)
              {
                if (!parameters[index].ParameterType.IsAssignableFrom(types[index]))
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
                return method;
            }
          }
        }
      }
      return (MethodInfo) null;
    }

    public static object MethodInvoke(System.Type type, object target, string name, params object[] args)
    {
      System.Type[] typeArray = new System.Type[args.Length];
      for (int index = 0; index < typeArray.Length; ++index)
        typeArray[index] = args[index].GetType();
      MethodInfo method = ReflectionUtility.FindMethod(type, name, typeArray);
      if (method != (MethodInfo) null)
        return method.Invoke(target, args);
      throw new MissingMemberException(MethodBase.GetCurrentMethod().ToString() + " : " + (object) type + " : " + name);
    }

    public static System.Type GetValueType(this MemberInfo member)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != (FieldInfo) null)
        return fieldInfo.FieldType;
      PropertyInfo propertyInfo = member as PropertyInfo;
      if (propertyInfo != (PropertyInfo) null)
        return propertyInfo.PropertyType;
      Debug.LogError((object) ("Error get value type from : " + (object) member.GetType()));
      return (System.Type) null;
    }

    public static object GetValue(this MemberInfo member, object target)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != (FieldInfo) null)
        return fieldInfo.GetValue(target);
      PropertyInfo propertyInfo = member as PropertyInfo;
      if (propertyInfo != (PropertyInfo) null)
      {
        MethodInfo getMethod = propertyInfo.GetGetMethod(true);
        if (getMethod != (MethodInfo) null)
          return getMethod.Invoke(target, (object[]) null);
      }
      Debug.LogError((object) ("Error get value from : " + (object) member.GetType() + " : " + (target != null ? target : (object) "null")));
      return (object) null;
    }

    public static void SetValue(this MemberInfo member, object target, object value)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != (FieldInfo) null)
      {
        fieldInfo.SetValue(target, value);
      }
      else
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        if (propertyInfo != (PropertyInfo) null)
        {
          MethodInfo setMethod = propertyInfo.GetSetMethod(true);
          if (setMethod != (MethodInfo) null)
          {
            setMethod.Invoke(target, new object[1]{ value });
            return;
          }
        }
        Debug.LogError((object) ("Error Set value from : " + (object) member.GetType() + " : " + (target != null ? target : (object) "null")));
      }
    }
  }
}
