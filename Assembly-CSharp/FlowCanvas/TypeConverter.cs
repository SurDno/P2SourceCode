// Decompiled with JetBrains decompiler
// Type: FlowCanvas.TypeConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common;
using ParadoxNotion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FlowCanvas
{
  public static class TypeConverter
  {
    public static ValueHandler<object> GetConverterFuncFromTo(System.Type targetType, ValueOutput source)
    {
      System.Type type = source.type;
      ValueHandler<object> sourceFunc = new ValueHandler<object>(source.GetValue);
      return TypeConverter.GetConverterFuncFromTo(targetType, type, sourceFunc);
    }

    public static ValueHandler<object> GetConverterFuncFromTo(
      System.Type targetType,
      System.Type sourceType,
      ValueHandler<object> sourceFunc)
    {
      if (targetType.RTIsAssignableFrom(sourceType))
        return sourceFunc;
      if (typeof (IConvertible).RTIsAssignableFrom(targetType) && typeof (IConvertible).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() => Convert.ChangeType(sourceFunc(), targetType));
      if (targetType == typeof (string) && sourceType != typeof (void))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) sourceFunc().ToString();
          }
          catch
          {
            return (object) null;
          }
        });
      if (targetType == typeof (Vector3) && typeof (Component).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as Component).transform.position;
          }
          catch
          {
            return (object) Vector3.zero;
          }
        });
      if (targetType == typeof (Vector3) && sourceType == typeof (GameObject))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as GameObject).transform.position;
          }
          catch
          {
            return (object) Vector3.zero;
          }
        });
      if (targetType == typeof (Quaternion) && typeof (Component).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as Component).transform.rotation;
          }
          catch
          {
            return (object) Quaternion.identity;
          }
        });
      if (targetType == typeof (Quaternion) && sourceType == typeof (GameObject))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as GameObject).transform.rotation;
          }
          catch
          {
            return (object) Quaternion.identity;
          }
        });
      if (typeof (Component).RTIsAssignableFrom(targetType) && typeof (Component).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as Component).GetComponent(targetType);
          }
          catch
          {
            return (object) null;
          }
        });
      if (typeof (Component).RTIsAssignableFrom(targetType) && sourceType == typeof (GameObject))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as GameObject).GetComponent(targetType);
          }
          catch
          {
            return (object) null;
          }
        });
      if (targetType == typeof (GameObject) && typeof (Component).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() =>
        {
          try
          {
            return (object) (sourceFunc() as Component).gameObject;
          }
          catch
          {
            return (object) null;
          }
        });
      if (targetType == typeof (bool) && typeof (UnityEngine.Object).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() => (object) (sourceFunc() as UnityEngine.Object != (UnityEngine.Object) null));
      if (targetType == typeof (int) && sourceType == typeof (LayerMask))
        return (ValueHandler<object>) (() => (object) ((LayerMask) sourceFunc()).value);
      if (targetType.RTIsSubclassOf(sourceType))
        return sourceFunc;
      if (typeof (IList).RTIsAssignableFrom(sourceType) && typeof (IList).RTIsAssignableFrom(targetType))
      {
        try
        {
          System.Type second = sourceType.IsArray ? sourceType.GetElementType() : sourceType.GetGenericArguments()[0];
          if ((targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0]).RTIsAssignableFrom(second))
            return (ValueHandler<object>) (() =>
            {
              List<object> source = new List<object>();
              IList list = sourceFunc() as IList;
              for (int index = 0; index < list.Count; ++index)
                source.Add(list[index]);
              return targetType.RTIsArray() ? (object) source.ToArray() : (object) source.ToList<object>();
            });
        }
        catch
        {
          return (ValueHandler<object>) null;
        }
      }
      return ((IEnumerable<MethodInfo>) sourceType.RTGetMethods()).Any<MethodInfo>((Func<MethodInfo, bool>) (m =>
      {
        if (!(m.ReturnType == targetType))
          return false;
        return m.Name == "op_Implicit" || m.Name == "op_Explicit";
      })) ? sourceFunc : TypeConverter.EngineConvert(targetType, sourceType, sourceFunc);
    }

    private static ValueHandler<object> EngineConvert(
      System.Type targetType,
      System.Type sourceType,
      ValueHandler<object> sourceFunc)
    {
      if (typeof (IEnumerable).RTIsAssignableFrom(sourceType) && typeof (IEnumerable).RTIsAssignableFrom(targetType))
      {
        System.Type second = sourceType.IsArray ? sourceType.GetElementType() : sourceType.GetGenericArguments()[0];
        System.Type elementTo = targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
        if (typeof (IComponent).RTIsAssignableFrom(second))
        {
          if (typeof (IComponent).RTIsAssignableFrom(elementTo))
            return (ValueHandler<object>) (() =>
            {
              IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(elementTo));
              foreach (IComponent component1 in (IEnumerable) sourceFunc())
              {
                IComponent component2 = component1.Owner.GetComponent(elementTo);
                if (component2 != null)
                  instance.Add((object) component2);
              }
              return (object) instance;
            });
          if (typeof (IEntity).RTIsAssignableFrom(elementTo))
            return (ValueHandler<object>) (() =>
            {
              IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(elementTo));
              foreach (IComponent component in (IEnumerable) sourceFunc())
                instance.Add((object) component.Owner);
              return (object) instance;
            });
        }
        else if (typeof (IEntity).RTIsAssignableFrom(second) && typeof (IComponent).RTIsAssignableFrom(elementTo))
          return (ValueHandler<object>) (() =>
          {
            IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(elementTo));
            foreach (IEntity entity in (IEnumerable) sourceFunc())
            {
              IComponent component = entity.GetComponent(elementTo);
              if (component != null)
                instance.Add((object) component);
            }
            return (object) instance;
          });
        return (ValueHandler<object>) null;
      }
      if (typeof (IComponent).RTIsAssignableFrom(targetType) && typeof (GameObject).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() =>
        {
          GameObject gameObject = sourceFunc() as GameObject;
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
          {
            IEntity entity = EntityUtility.GetEntity(gameObject);
            if (entity != null)
              return (object) entity.GetComponent(targetType);
          }
          return (object) null;
        });
      if (targetType == typeof (IEntity) && typeof (GameObject).RTIsAssignableFrom(sourceType))
        return (ValueHandler<object>) (() =>
        {
          GameObject gameObject = sourceFunc() as GameObject;
          return (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? (object) EntityUtility.GetEntity(gameObject) : (object) null;
        });
      if (TypeUtility.IsAssignableFrom(typeof (IObject), sourceType) && TypeUtility.IsAssignableFrom(typeof (IObject), targetType))
        return (ValueHandler<object>) (() => !(sourceFunc() is IObject @object) ? (object) null : (object) @object);
      if (TypeUtility.IsAssignableFrom(typeof (IObject), sourceType) && TypeUtility.IsAssignableFrom(typeof (IComponent), targetType))
        return (ValueHandler<object>) (() => !(sourceFunc() is IEntity entity1) ? (object) null : (object) entity1.GetComponent(targetType));
      if (TypeUtility.IsAssignableFrom(typeof (IComponent), sourceType) && TypeUtility.IsAssignableFrom(typeof (IObject), targetType))
        return (ValueHandler<object>) (() => !(sourceFunc() is IComponent component3) || !TypeUtility.IsAssignableFrom(typeof (IEntity), targetType) ? (object) null : (object) component3.Owner);
      return TypeUtility.IsAssignableFrom(typeof (IComponent), sourceType) && TypeUtility.IsAssignableFrom(typeof (IComponent), targetType) ? (ValueHandler<object>) (() => !(sourceFunc() is IComponent component4) || component4.Owner == null ? (object) null : (object) component4.Owner.GetComponent(targetType)) : (ValueHandler<object>) null;
    }

    public static bool HasConvertion(System.Type sourceType, System.Type targetType)
    {
      ValueHandler<object> sourceFunc = (ValueHandler<object>) (() => (object) null);
      return TypeConverter.GetConverterFuncFromTo(targetType, sourceType, sourceFunc) != null;
    }

    public static bool TryConnect(object dest, ValueOutput source)
    {
      if (dest is IValueInput<int> valueInput1)
      {
        ValueOutput<float> sourceFloat = source as ValueOutput<float>;
        if (sourceFloat != null)
        {
          valueInput1.getter = (ValueHandler<int>) (() => (int) sourceFloat.getter());
          return true;
        }
        ValueOutput<bool> sourceBool = source as ValueOutput<bool>;
        if (sourceBool != null)
        {
          valueInput1.getter = (ValueHandler<int>) (() => sourceBool.getter() ? 1 : 0);
          return true;
        }
      }
      if (dest is IValueInput<float> valueInput2)
      {
        ValueOutput<int> sourceInt = source as ValueOutput<int>;
        if (sourceInt != null)
        {
          valueInput2.getter = (ValueHandler<float>) (() => (float) sourceInt.getter());
          return true;
        }
        ValueOutput<bool> sourceBool = source as ValueOutput<bool>;
        if (sourceBool != null)
        {
          valueInput2.getter = (ValueHandler<float>) (() => sourceBool.getter() ? 1f : 0.0f);
          return true;
        }
      }
      if (dest is IValueInput<bool> valueInput3)
      {
        ValueOutput<int> sourceInt = source as ValueOutput<int>;
        if (sourceInt != null)
        {
          valueInput3.getter = (ValueHandler<bool>) (() => sourceInt.getter() != 0);
          return true;
        }
        ValueOutput<float> sourceFloat = source as ValueOutput<float>;
        if (sourceFloat != null)
        {
          valueInput3.getter = (ValueHandler<bool>) (() => (double) sourceFloat.getter() > 0.5);
          return true;
        }
      }
      return false;
    }
  }
}
