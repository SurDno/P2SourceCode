using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsMetaType
  {
    private static Dictionary<fsConfig, Dictionary<System.Type, fsMetaType>> _configMetaTypes = new Dictionary<fsConfig, Dictionary<System.Type, fsMetaType>>();
    private Func<object> Generator;
    public System.Type ReflectedType;
    private bool _hasEmittedAotData;
    private bool? _hasDefaultConstructorCache;
    private bool _isDefaultConstructorPublic;

    public static fsMetaType Get(fsConfig config, System.Type type)
    {
      Dictionary<System.Type, fsMetaType> dictionary;
      if (!fsMetaType._configMetaTypes.TryGetValue(config, out dictionary))
        dictionary = fsMetaType._configMetaTypes[config] = new Dictionary<System.Type, fsMetaType>();
      fsMetaType fsMetaType;
      if (!dictionary.TryGetValue(type, out fsMetaType))
      {
        fsMetaType = new fsMetaType(config, type);
        dictionary[type] = fsMetaType;
      }
      return fsMetaType;
    }

    public static void ClearCache()
    {
      fsMetaType._configMetaTypes = new Dictionary<fsConfig, Dictionary<System.Type, fsMetaType>>();
    }

    private fsMetaType(fsConfig config, System.Type reflectedType)
    {
      this.ReflectedType = reflectedType;
      List<fsMetaProperty> properties = new List<fsMetaProperty>();
      fsMetaType.CollectProperties(config, properties, reflectedType);
      this.Properties = properties.ToArray();
      try
      {
        if (this.ReflectedType.Resolve().IsValueType || !(this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != (ConstructorInfo) null))
          return;
        this.Generator = ((Expression<Func<object>>) (() => System.Linq.Expressions.Expression.New(reflectedType))).Compile();
      }
      catch
      {
        this.Generator = (Func<object>) null;
      }
    }

    private static void CollectProperties(
      fsConfig config,
      List<fsMetaProperty> properties,
      System.Type reflectedType)
    {
      bool flag = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
      bool annotationFreeValue = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;
      fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>((MemberInfo) reflectedType);
      if (attribute != null)
      {
        flag = attribute.MemberSerialization == fsMemberSerialization.OptIn;
        annotationFreeValue = attribute.MemberSerialization == fsMemberSerialization.OptOut;
      }
      MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
      foreach (MemberInfo memberInfo in declaredMembers)
      {
        MemberInfo member = memberInfo;
        if (!((IEnumerable<System.Type>) config.IgnoreSerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute(member, t))))
        {
          PropertyInfo property = member as PropertyInfo;
          FieldInfo field = member as FieldInfo;
          if ((!(property == (PropertyInfo) null) || !(field == (FieldInfo) null)) && (!flag || ((IEnumerable<System.Type>) config.SerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute(member, t)))) && (!annotationFreeValue || !((IEnumerable<System.Type>) config.IgnoreSerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute(member, t)))))
          {
            if (property != (PropertyInfo) null)
            {
              if (fsMetaType.CanSerializeProperty(config, property, declaredMembers, annotationFreeValue))
                properties.Add(new fsMetaProperty(config, property));
            }
            else if (field != (FieldInfo) null && fsMetaType.CanSerializeField(config, field, annotationFreeValue))
              properties.Add(new fsMetaProperty(config, field));
          }
        }
      }
      if (!(reflectedType.Resolve().BaseType != (System.Type) null))
        return;
      fsMetaType.CollectProperties(config, properties, reflectedType.Resolve().BaseType);
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
      if (getMethod != (MethodInfo) null && getMethod.IsStatic || setMethod != (MethodInfo) null && setMethod.IsStatic || property.GetIndexParameters().Length != 0)
        return false;
      if (((IEnumerable<System.Type>) config.SerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute((MemberInfo) property, t))))
        return true;
      if (!property.CanRead || !property.CanWrite)
        return false;
      return (config.SerializeNonAutoProperties || fsMetaType.IsAutoProperty(property, members)) && getMethod != (MethodInfo) null && (config.SerializeNonPublicSetProperties || setMethod != (MethodInfo) null) || annotationFreeValue;
    }

    private static bool CanSerializeField(
      fsConfig config,
      FieldInfo field,
      bool annotationFreeValue)
    {
      return !typeof (Delegate).IsAssignableFrom(field.FieldType) && !field.IsDefined(typeof (CompilerGeneratedAttribute), false) && !field.IsStatic && (((IEnumerable<System.Type>) config.SerializeAttributes).Any<System.Type>((Func<System.Type, bool>) (t => fsPortableReflection.HasAttribute((MemberInfo) field, t))) || annotationFreeValue || field.IsPublic);
    }

    public bool EmitAotData()
    {
      if (this._hasEmittedAotData)
        return false;
      this._hasEmittedAotData = true;
      for (int index = 0; index < this.Properties.Length; ++index)
      {
        if (!this.Properties[index].IsPublic)
          return false;
      }
      if (!this.HasDefaultConstructor)
        return false;
      fsAotCompilationManager.AddAotCompilation(this.ReflectedType, this.Properties, this._isDefaultConstructorPublic);
      return true;
    }

    public fsMetaProperty[] Properties { get; private set; }

    public bool HasDefaultConstructor
    {
      get
      {
        if (!this._hasDefaultConstructorCache.HasValue)
        {
          if (this.ReflectedType.Resolve().IsArray)
          {
            this._hasDefaultConstructorCache = new bool?(true);
            this._isDefaultConstructorPublic = true;
          }
          else if (this.ReflectedType.Resolve().IsValueType)
          {
            this._hasDefaultConstructorCache = new bool?(true);
            this._isDefaultConstructorPublic = true;
          }
          else
          {
            ConstructorInfo declaredConstructor = this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
            this._hasDefaultConstructorCache = new bool?(declaredConstructor != (ConstructorInfo) null);
            if (declaredConstructor != (ConstructorInfo) null)
              this._isDefaultConstructorPublic = declaredConstructor.IsPublic;
          }
        }
        return this._hasDefaultConstructorCache.Value;
      }
    }

    public object CreateInstance()
    {
      if (this.ReflectedType.Resolve().IsInterface || this.ReflectedType.Resolve().IsAbstract)
        throw new Exception("Cannot create an instance of an interface or abstract type for " + (object) this.ReflectedType);
      if (typeof (ScriptableObject).IsAssignableFrom(this.ReflectedType))
        return (object) ScriptableObject.CreateInstance(this.ReflectedType);
      if (typeof (string) == this.ReflectedType)
        return (object) string.Empty;
      if (!this.HasDefaultConstructor)
        return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
      if (this.ReflectedType.Resolve().IsArray)
        return (object) Array.CreateInstance(this.ReflectedType.GetElementType(), 0);
      try
      {
        return this.Generator != null ? this.Generator() : Activator.CreateInstance(this.ReflectedType, true);
      }
      catch (MissingMethodException ex)
      {
        throw new InvalidOperationException("Unable to create instance of " + (object) this.ReflectedType + "; there is no default constructor", (Exception) ex);
      }
      catch (TargetInvocationException ex)
      {
        throw new InvalidOperationException("Constructor of " + (object) this.ReflectedType + " threw an exception when creating an instance", (Exception) ex);
      }
      catch (MemberAccessException ex)
      {
        throw new InvalidOperationException("Unable to access constructor of " + (object) this.ReflectedType, (Exception) ex);
      }
    }
  }
}
