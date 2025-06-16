using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public static class fsPortableReflection
  {
    public static Type[] EmptyTypes = new Type[0];
    private static IDictionary<AttributeQuery, Attribute> _cachedAttributeQueries = new Dictionary<AttributeQuery, Attribute>(new AttributeQueryComparator());
    private static BindingFlags DeclaredFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public static bool HasAttribute(MemberInfo element, Type attributeType)
    {
      return GetAttribute(element, attributeType, true) != null;
    }

    public static bool HasAttribute<TAttribute>(MemberInfo element)
    {
      return HasAttribute(element, typeof (TAttribute));
    }

    public static Attribute GetAttribute(MemberInfo element, Type attributeType, bool shouldCache)
    {
      AttributeQuery key = new AttributeQuery {
        MemberInfo = element,
        AttributeType = attributeType
      };
      Attribute attribute;
      if (!_cachedAttributeQueries.TryGetValue(key, out attribute))
      {
        attribute = (Attribute) element.GetCustomAttributes(attributeType, true).FirstOrDefault();
        if (shouldCache)
          _cachedAttributeQueries[key] = attribute;
      }
      return attribute;
    }

    public static TAttribute GetAttribute<TAttribute>(MemberInfo element, bool shouldCache) where TAttribute : Attribute
    {
      return (TAttribute) GetAttribute(element, typeof (TAttribute), shouldCache);
    }

    public static TAttribute GetAttribute<TAttribute>(MemberInfo element) where TAttribute : Attribute
    {
      return GetAttribute<TAttribute>(element, true);
    }

    public static PropertyInfo GetDeclaredProperty(this Type type, string propertyName)
    {
      PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
      for (int index = 0; index < declaredProperties.Length; ++index)
      {
        if (declaredProperties[index].Name == propertyName)
          return declaredProperties[index];
      }
      return null;
    }

    public static MethodInfo GetDeclaredMethod(this Type type, string methodName)
    {
      MethodInfo[] declaredMethods = type.GetDeclaredMethods();
      for (int index = 0; index < declaredMethods.Length; ++index)
      {
        if (declaredMethods[index].Name == methodName)
          return declaredMethods[index];
      }
      return null;
    }

    public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] parameters)
    {
      foreach (ConstructorInfo declaredConstructor in type.GetDeclaredConstructors())
      {
        ParameterInfo[] parameters1 = declaredConstructor.GetParameters();
        if (parameters.Length == parameters1.Length)
        {
          for (int index = 0; index < parameters1.Length; ++index)
          {
            if (!(parameters1[index].ParameterType != parameters[index]))
              ;
          }
          return declaredConstructor;
        }
      }
      return null;
    }

    public static ConstructorInfo[] GetDeclaredConstructors(this Type type)
    {
      return type.GetConstructors(DeclaredFlags);
    }

    public static MemberInfo[] GetFlattenedMember(this Type type, string memberName)
    {
      List<MemberInfo> memberInfoList = new List<MemberInfo>();
      for (; type != null; type = type.Resolve().BaseType)
      {
        MemberInfo[] declaredMembers = type.GetDeclaredMembers();
        for (int index = 0; index < declaredMembers.Length; ++index)
        {
          if (declaredMembers[index].Name == memberName)
            memberInfoList.Add(declaredMembers[index]);
        }
      }
      return memberInfoList.ToArray();
    }

    public static MethodInfo GetFlattenedMethod(this Type type, string methodName)
    {
      for (; type != null; type = type.Resolve().BaseType)
      {
        MethodInfo[] declaredMethods = type.GetDeclaredMethods();
        for (int index = 0; index < declaredMethods.Length; ++index)
        {
          if (declaredMethods[index].Name == methodName)
            return declaredMethods[index];
        }
      }
      return null;
    }

    public static IEnumerable<MethodInfo> GetFlattenedMethods(this Type type, string methodName)
    {
      while (type != null)
      {
        MethodInfo[] methods = type.GetDeclaredMethods();
        for (int i = 0; i < methods.Length; ++i)
        {
          if (methods[i].Name == methodName)
            yield return methods[i];
        }
        type = type.Resolve().BaseType;
        methods = null;
      }
    }

    public static PropertyInfo GetFlattenedProperty(this Type type, string propertyName)
    {
      for (; type != null; type = type.Resolve().BaseType)
      {
        PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
        for (int index = 0; index < declaredProperties.Length; ++index)
        {
          if (declaredProperties[index].Name == propertyName)
            return declaredProperties[index];
        }
      }
      return null;
    }

    public static MemberInfo GetDeclaredMember(this Type type, string memberName)
    {
      MemberInfo[] declaredMembers = type.GetDeclaredMembers();
      for (int index = 0; index < declaredMembers.Length; ++index)
      {
        if (declaredMembers[index].Name == memberName)
          return declaredMembers[index];
      }
      return null;
    }

    public static MethodInfo[] GetDeclaredMethods(this Type type)
    {
      return type.GetMethods(DeclaredFlags);
    }

    public static PropertyInfo[] GetDeclaredProperties(this Type type)
    {
      return type.GetProperties(DeclaredFlags);
    }

    public static FieldInfo[] GetDeclaredFields(this Type type)
    {
      return type.GetFields(DeclaredFlags);
    }

    public static MemberInfo[] GetDeclaredMembers(this Type type)
    {
      return type.GetMembers(DeclaredFlags);
    }

    public static MemberInfo AsMemberInfo(Type type) => type;

    public static bool IsType(MemberInfo member) => member is Type;

    public static Type AsType(MemberInfo member) => (Type) member;

    public static Type Resolve(this Type type) => type;

    private struct AttributeQuery
    {
      public MemberInfo MemberInfo;
      public Type AttributeType;
    }

    private class AttributeQueryComparator : IEqualityComparer<AttributeQuery>
    {
      public bool Equals(
        AttributeQuery x,
        AttributeQuery y)
      {
        return x.MemberInfo == y.MemberInfo && x.AttributeType == y.AttributeType;
      }

      public int GetHashCode(AttributeQuery obj)
      {
        return obj.MemberInfo.GetHashCode() + 17 * obj.AttributeType.GetHashCode();
      }
    }
  }
}
