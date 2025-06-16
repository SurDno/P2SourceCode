using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cofe.Utility;
using Engine.Common;
using ParadoxNotion;

namespace FlowCanvas
{
  public static class TypeConverter
  {
    public static ValueHandler<object> GetConverterFuncFromTo(Type targetType, ValueOutput source)
    {
      Type type = source.type;
      ValueHandler<object> sourceFunc = source.GetValue;
      return GetConverterFuncFromTo(targetType, type, sourceFunc);
    }

    public static ValueHandler<object> GetConverterFuncFromTo(
      Type targetType,
      Type sourceType,
      ValueHandler<object> sourceFunc)
    {
      if (targetType.RTIsAssignableFrom(sourceType))
        return sourceFunc;
      if (typeof (IConvertible).RTIsAssignableFrom(targetType) && typeof (IConvertible).RTIsAssignableFrom(sourceType))
        return () => Convert.ChangeType(sourceFunc(), targetType);
      if (targetType == typeof (string) && sourceType != typeof (void))
        return () =>
        {
          try
          {
            return sourceFunc().ToString();
          }
          catch
          {
            return null;
          }
        };
      if (targetType == typeof (Vector3) && typeof (Component).RTIsAssignableFrom(sourceType))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as Component).transform.position;
          }
          catch
          {
            return (object) Vector3.zero;
          }
        };
      if (targetType == typeof (Vector3) && sourceType == typeof (GameObject))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as GameObject).transform.position;
          }
          catch
          {
            return (object) Vector3.zero;
          }
        };
      if (targetType == typeof (Quaternion) && typeof (Component).RTIsAssignableFrom(sourceType))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as Component).transform.rotation;
          }
          catch
          {
            return (object) Quaternion.identity;
          }
        };
      if (targetType == typeof (Quaternion) && sourceType == typeof (GameObject))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as GameObject).transform.rotation;
          }
          catch
          {
            return (object) Quaternion.identity;
          }
        };
      if (typeof (Component).RTIsAssignableFrom(targetType) && typeof (Component).RTIsAssignableFrom(sourceType))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as Component).GetComponent(targetType);
          }
          catch
          {
            return null;
          }
        };
      if (typeof (Component).RTIsAssignableFrom(targetType) && sourceType == typeof (GameObject))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as GameObject).GetComponent(targetType);
          }
          catch
          {
            return null;
          }
        };
      if (targetType == typeof (GameObject) && typeof (Component).RTIsAssignableFrom(sourceType))
        return () =>
        {
          try
          {
            return (object) (sourceFunc() as Component).gameObject;
          }
          catch
          {
            return null;
          }
        };
      if (targetType == typeof (bool) && typeof (UnityEngine.Object).RTIsAssignableFrom(sourceType))
        return () => (object) (sourceFunc() as UnityEngine.Object != (UnityEngine.Object) null);
      if (targetType == typeof (int) && sourceType == typeof (LayerMask))
        return () => (object) ((LayerMask) sourceFunc()).value;
      if (targetType.RTIsSubclassOf(sourceType))
        return sourceFunc;
      if (typeof (IList).RTIsAssignableFrom(sourceType) && typeof (IList).RTIsAssignableFrom(targetType))
      {
        try
        {
          Type second = sourceType.IsArray ? sourceType.GetElementType() : sourceType.GetGenericArguments()[0];
          if ((targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0]).RTIsAssignableFrom(second))
            return () =>
            {
              List<object> source = new List<object>();
              IList list = sourceFunc() as IList;
              for (int index = 0; index < list.Count; ++index)
                source.Add(list[index]);
              return targetType.RTIsArray() ? source.ToArray() : source.ToList();
            };
        }
        catch
        {
          return null;
        }
      }
      return sourceType.RTGetMethods().Any(m =>
      {
        if (!(m.ReturnType == targetType))
          return false;
        return m.Name == "op_Implicit" || m.Name == "op_Explicit";
      }) ? sourceFunc : EngineConvert(targetType, sourceType, sourceFunc);
    }

    private static ValueHandler<object> EngineConvert(
      Type targetType,
      Type sourceType,
      ValueHandler<object> sourceFunc)
    {
      if (typeof (IEnumerable).RTIsAssignableFrom(sourceType) && typeof (IEnumerable).RTIsAssignableFrom(targetType))
      {
        Type second = sourceType.IsArray ? sourceType.GetElementType() : sourceType.GetGenericArguments()[0];
        Type elementTo = targetType.IsArray ? targetType.GetElementType() : targetType.GetGenericArguments()[0];
        if (typeof (IComponent).RTIsAssignableFrom(second))
        {
          if (typeof (IComponent).RTIsAssignableFrom(elementTo))
            return () =>
            {
              IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(elementTo));
              foreach (IComponent component1 in (IEnumerable) sourceFunc())
              {
                IComponent component2 = component1.Owner.GetComponent(elementTo);
                if (component2 != null)
                  instance.Add(component2);
              }
              return instance;
            };
          if (typeof (IEntity).RTIsAssignableFrom(elementTo))
            return () =>
            {
              IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(elementTo));
              foreach (IComponent component in (IEnumerable) sourceFunc())
                instance.Add(component.Owner);
              return instance;
            };
        }
        else if (typeof (IEntity).RTIsAssignableFrom(second) && typeof (IComponent).RTIsAssignableFrom(elementTo))
          return () =>
          {
            IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(elementTo));
            foreach (IEntity entity in (IEnumerable) sourceFunc())
            {
              IComponent component = entity.GetComponent(elementTo);
              if (component != null)
                instance.Add(component);
            }
            return instance;
          };
        return null;
      }
      if (typeof (IComponent).RTIsAssignableFrom(targetType) && typeof (GameObject).RTIsAssignableFrom(sourceType))
        return () =>
        {
          GameObject gameObject = sourceFunc() as GameObject;
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
          {
            IEntity entity = EntityUtility.GetEntity(gameObject);
            if (entity != null)
              return entity.GetComponent(targetType);
          }
          return null;
        };
      if (targetType == typeof (IEntity) && typeof (GameObject).RTIsAssignableFrom(sourceType))
        return () =>
        {
          GameObject gameObject = sourceFunc() as GameObject;
          return (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? EntityUtility.GetEntity(gameObject) : (object) null;
        };
      if (TypeUtility.IsAssignableFrom(typeof (IObject), sourceType) && TypeUtility.IsAssignableFrom(typeof (IObject), targetType))
        return () => !(sourceFunc() is IObject @object) ? null : (object) @object;
      if (TypeUtility.IsAssignableFrom(typeof (IObject), sourceType) && TypeUtility.IsAssignableFrom(typeof (IComponent), targetType))
        return () => !(sourceFunc() is IEntity entity1) ? null : (object) entity1.GetComponent(targetType);
      if (TypeUtility.IsAssignableFrom(typeof (IComponent), sourceType) && TypeUtility.IsAssignableFrom(typeof (IObject), targetType))
        return () => !(sourceFunc() is IComponent component3) || !TypeUtility.IsAssignableFrom(typeof (IEntity), targetType) ? null : (object) component3.Owner;
      return TypeUtility.IsAssignableFrom(typeof (IComponent), sourceType) && TypeUtility.IsAssignableFrom(typeof (IComponent), targetType) ? () => !(sourceFunc() is IComponent component4) || component4.Owner == null ? null : (object) component4.Owner.GetComponent(targetType) : null;
    }

    public static bool HasConvertion(Type sourceType, Type targetType)
    {
      ValueHandler<object> sourceFunc = () => null;
      return GetConverterFuncFromTo(targetType, sourceType, sourceFunc) != null;
    }

    public static bool TryConnect(object dest, ValueOutput source)
    {
      if (dest is IValueInput<int> valueInput1)
      {
        ValueOutput<float> sourceFloat = source as ValueOutput<float>;
        if (sourceFloat != null)
        {
          valueInput1.getter = () => (int) sourceFloat.getter();
          return true;
        }
        ValueOutput<bool> sourceBool = source as ValueOutput<bool>;
        if (sourceBool != null)
        {
          valueInput1.getter = () => sourceBool.getter() ? 1 : 0;
          return true;
        }
      }
      if (dest is IValueInput<float> valueInput2)
      {
        ValueOutput<int> sourceInt = source as ValueOutput<int>;
        if (sourceInt != null)
        {
          valueInput2.getter = () => sourceInt.getter();
          return true;
        }
        ValueOutput<bool> sourceBool = source as ValueOutput<bool>;
        if (sourceBool != null)
        {
          valueInput2.getter = () => sourceBool.getter() ? 1f : 0.0f;
          return true;
        }
      }
      if (dest is IValueInput<bool> valueInput3)
      {
        ValueOutput<int> sourceInt = source as ValueOutput<int>;
        if (sourceInt != null)
        {
          valueInput3.getter = () => sourceInt.getter() != 0;
          return true;
        }
        ValueOutput<float> sourceFloat = source as ValueOutput<float>;
        if (sourceFloat != null)
        {
          valueInput3.getter = () => sourceFloat.getter() > 0.5;
          return true;
        }
      }
      return false;
    }
  }
}
