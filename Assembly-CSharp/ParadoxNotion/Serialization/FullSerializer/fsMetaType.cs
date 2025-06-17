using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsMetaType
  {
    private static Dictionary<fsConfig, Dictionary<Type, fsMetaType>> _configMetaTypes = new();
    private Func<object> Generator;
    public Type ReflectedType;
    private bool _hasEmittedAotData;
    private bool? _hasDefaultConstructorCache;
    private bool _isDefaultConstructorPublic;

    public static fsMetaType Get(fsConfig config, Type type)
    {
      if (!_configMetaTypes.TryGetValue(config, out Dictionary<Type, fsMetaType> dictionary))
        dictionary = _configMetaTypes[config] = new Dictionary<Type, fsMetaType>();
      if (!dictionary.TryGetValue(type, out fsMetaType fsMetaType))
      {
        fsMetaType = new fsMetaType(config, type);
        dictionary[type] = fsMetaType;
      }
      return fsMetaType;
    }

    public static void ClearCache()
    {
      _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();
    }

    private fsMetaType(fsConfig config, Type reflectedType)
    {
      ReflectedType = reflectedType;
      List<fsMetaProperty> properties = [];
      CollectProperties(config, properties, reflectedType);
      Properties = properties.ToArray();
      try
      {
        if (ReflectedType.Resolve().IsValueType || !(ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != null))
          return;
        Generator = ((Expression<Func<object>>) (() => Expression.New(reflectedType))).Compile();
      }
      catch
      {
        Generator = null;
      }
    }

    private static void CollectProperties(
      fsConfig config,
      List<fsMetaProperty> properties,
      Type reflectedType)
    {
      bool flag = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
      bool annotationFreeValue = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;
      fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(reflectedType);
      if (attribute != null)
      {
        flag = attribute.MemberSerialization == fsMemberSerialization.OptIn;
        annotationFreeValue = attribute.MemberSerialization == fsMemberSerialization.OptOut;
      }
      MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
      foreach (MemberInfo memberInfo in declaredMembers)
      {
        MemberInfo member = memberInfo;
        if (!config.IgnoreSerializeAttributes.Any(t => fsPortableReflection.HasAttribute(member, t)))
        {
          PropertyInfo property = member as PropertyInfo;
          FieldInfo field = member as FieldInfo;
          if ((!(property == null) || !(field == null)) && (!flag || config.SerializeAttributes.Any(t => fsPortableReflection.HasAttribute(member, t))) && (!annotationFreeValue || !config.IgnoreSerializeAttributes.Any(t => fsPortableReflection.HasAttribute(member, t))))
          {
            if (property != null)
            {
              if (CanSerializeProperty(config, property, declaredMembers, annotationFreeValue))
                properties.Add(new fsMetaProperty(config, property));
            }
            else if (field != null && CanSerializeField(config, field, annotationFreeValue))
              properties.Add(new fsMetaProperty(config, field));
          }
        }
      }
      if (!(reflectedType.Resolve().BaseType != null))
        return;
      CollectProperties(config, properties, reflectedType.Resolve().BaseType);
    }

    private static bool IsAutoProperty(PropertyInfo property, MemberInfo[] members)
    {
      if (!property.CanWrite || !property.CanRead)
        return false;
      string str = "<" + property.Name + ">k__BackingField";
      for (int index = 0; index < members.Length; ++index)
      {
        if (members[index].Name == str)
          return true;
      }
      return false;
    }

    private static bool CanSerializeProperty(
      fsConfig config,
      PropertyInfo property,
      MemberInfo[] members,
      bool annotationFreeValue)
    {
      if (typeof (Delegate).IsAssignableFrom(property.PropertyType))
        return false;
      MethodInfo getMethod = property.GetGetMethod(false);
      MethodInfo setMethod = property.GetSetMethod(false);
      if (getMethod != null && getMethod.IsStatic || setMethod != null && setMethod.IsStatic || property.GetIndexParameters().Length != 0)
        return false;
      if (config.SerializeAttributes.Any(t => fsPortableReflection.HasAttribute(property, t)))
        return true;
      if (!property.CanRead || !property.CanWrite)
        return false;
      return (config.SerializeNonAutoProperties || IsAutoProperty(property, members)) && getMethod != null && (config.SerializeNonPublicSetProperties || setMethod != null) || annotationFreeValue;
    }

    private static bool CanSerializeField(
      fsConfig config,
      FieldInfo field,
      bool annotationFreeValue)
    {
      return !typeof (Delegate).IsAssignableFrom(field.FieldType) && !field.IsDefined(typeof (CompilerGeneratedAttribute), false) && !field.IsStatic && (config.SerializeAttributes.Any(t => fsPortableReflection.HasAttribute(field, t)) || annotationFreeValue || field.IsPublic);
    }

    public bool EmitAotData()
    {
      if (_hasEmittedAotData)
        return false;
      _hasEmittedAotData = true;
      for (int index = 0; index < Properties.Length; ++index)
      {
        if (!Properties[index].IsPublic)
          return false;
      }
      if (!HasDefaultConstructor)
        return false;
      fsAotCompilationManager.AddAotCompilation(ReflectedType, Properties, _isDefaultConstructorPublic);
      return true;
    }

    public fsMetaProperty[] Properties { get; private set; }

    public bool HasDefaultConstructor
    {
      get
      {
        if (!_hasDefaultConstructorCache.HasValue)
        {
          if (ReflectedType.Resolve().IsArray)
          {
            _hasDefaultConstructorCache = true;
            _isDefaultConstructorPublic = true;
          }
          else if (ReflectedType.Resolve().IsValueType)
          {
            _hasDefaultConstructorCache = true;
            _isDefaultConstructorPublic = true;
          }
          else
          {
            ConstructorInfo declaredConstructor = ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
            _hasDefaultConstructorCache = declaredConstructor != null;
            if (declaredConstructor != null)
              _isDefaultConstructorPublic = declaredConstructor.IsPublic;
          }
        }
        return _hasDefaultConstructorCache.Value;
      }
    }

    public object CreateInstance()
    {
      if (ReflectedType.Resolve().IsInterface || ReflectedType.Resolve().IsAbstract)
        throw new Exception("Cannot create an instance of an interface or abstract type for " + ReflectedType);
      if (typeof (ScriptableObject).IsAssignableFrom(ReflectedType))
        return ScriptableObject.CreateInstance(ReflectedType);
      if (typeof (string) == ReflectedType)
        return string.Empty;
      if (!HasDefaultConstructor)
        return FormatterServices.GetSafeUninitializedObject(ReflectedType);
      if (ReflectedType.Resolve().IsArray)
        return Array.CreateInstance(ReflectedType.GetElementType(), 0);
      try
      {
        return Generator != null ? Generator() : Activator.CreateInstance(ReflectedType, true);
      }
      catch (MissingMethodException ex)
      {
        throw new InvalidOperationException("Unable to create instance of " + ReflectedType + "; there is no default constructor", ex);
      }
      catch (TargetInvocationException ex)
      {
        throw new InvalidOperationException("Constructor of " + ReflectedType + " threw an exception when creating an instance", ex);
      }
      catch (MemberAccessException ex)
      {
        throw new InvalidOperationException("Unable to access constructor of " + ReflectedType, ex);
      }
    }
  }
}
