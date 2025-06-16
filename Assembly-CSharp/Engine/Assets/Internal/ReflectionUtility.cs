using System;
using System.Reflection;

namespace Engine.Assets.Internal
{
  public static class ReflectionUtility
  {
    public static void SetProperty(Type type, object target, string name, object value)
    {
      PropertyInfo property = FindProperty(type, name);
      if (property != null)
        property.SetValue(target, value, new object[0]);
      else
        throw new MissingMemberException(MethodBase.GetCurrentMethod() + " : " + type + " : " + name);
    }

    public static object GetProperty(Type type, object target, string name)
    {
      PropertyInfo property = FindProperty(type, name);
      return property != null ? property.GetValue(target, new object[0]) : throw new MissingMemberException(MethodBase.GetCurrentMethod() + " : " + type + " : " + name);
    }

    public static PropertyInfo FindProperty(Type type, string name)
    {
      for (; type != typeof (object); type = type.BaseType)
      {
        PropertyInfo property = type.GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
          return property;
      }
      return null;
    }

    public static FieldInfo FindField(Type type, string name)
    {
      for (; type != typeof (object); type = type.BaseType)
      {
        FieldInfo field = type.GetField(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
          return field;
      }
      return null;
    }

    public static void SetField(Type type, object target, string name, object value)
    {
      FieldInfo field = FindField(type, name);
      if (field != null)
        field.SetValue(target, value);
      else
        throw new MissingMemberException(MethodBase.GetCurrentMethod() + " : " + type + " : " + name);
    }

    public static object GetField(Type type, object target, string name)
    {
      FieldInfo field = FindField(type, name);
      return field != null ? field.GetValue(target) : throw new MissingMemberException(MethodBase.GetCurrentMethod() + " : " + type + " : " + name);
    }

    public static MethodInfo FindMethod(Type type, string name, params Type[] types)
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
      return null;
    }

    public static object MethodInvoke(Type type, object target, string name, params object[] args)
    {
      Type[] typeArray = new Type[args.Length];
      for (int index = 0; index < typeArray.Length; ++index)
        typeArray[index] = args[index].GetType();
      MethodInfo method = FindMethod(type, name, typeArray);
      if (method != null)
        return method.Invoke(target, args);
      throw new MissingMemberException(MethodBase.GetCurrentMethod() + " : " + type + " : " + name);
    }

    public static Type GetValueType(this MemberInfo member)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != null)
        return fieldInfo.FieldType;
      PropertyInfo propertyInfo = member as PropertyInfo;
      if (propertyInfo != null)
        return propertyInfo.PropertyType;
      Debug.LogError((object) ("Error get value type from : " + member.GetType()));
      return null;
    }

    public static object GetValue(this MemberInfo member, object target)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != null)
        return fieldInfo.GetValue(target);
      PropertyInfo propertyInfo = member as PropertyInfo;
      if (propertyInfo != null)
      {
        MethodInfo getMethod = propertyInfo.GetGetMethod(true);
        if (getMethod != null)
          return getMethod.Invoke(target, null);
      }
      Debug.LogError((object) ("Error get value from : " + member.GetType() + " : " + (target != null ? target : "null")));
      return null;
    }

    public static void SetValue(this MemberInfo member, object target, object value)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != null)
      {
        fieldInfo.SetValue(target, value);
      }
      else
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        if (propertyInfo != null)
        {
          MethodInfo setMethod = propertyInfo.GetSetMethod(true);
          if (setMethod != null)
          {
            setMethod.Invoke(target, new object[1]{ value });
            return;
          }
        }
        Debug.LogError((object) ("Error Set value from : " + member.GetType() + " : " + (target != null ? target : "null")));
      }
    }
  }
}
