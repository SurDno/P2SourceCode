// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsIEnumerableConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsIEnumerableConverter : fsConverter
  {
    public override bool CanProcess(Type type)
    {
      return typeof (IEnumerable).IsAssignableFrom(type) && fsIEnumerableConverter.GetAddMethod(type) != (MethodInfo) null;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
    }

    public override fsResult TrySerialize(
      object instance_,
      out fsData serialized,
      Type storageType)
    {
      IEnumerable collection = (IEnumerable) instance_;
      fsResult success = fsResult.Success;
      Type elementType = fsIEnumerableConverter.GetElementType(storageType);
      serialized = fsData.CreateList(fsIEnumerableConverter.HintSize(collection));
      List<fsData> asList = serialized.AsList;
      foreach (object instance in collection)
      {
        fsData data;
        fsResult result = this.Serializer.TrySerialize(elementType, instance, out data);
        success.AddMessages(result);
        if (!result.Failed)
          asList.Add(data);
      }
      if (this.IsStack(collection.GetType()))
        asList.Reverse();
      return success;
    }

    private bool IsStack(Type type)
    {
      return type.Resolve().IsGenericType && type.Resolve().GetGenericTypeDefinition() == typeof (Stack<>);
    }

    public override fsResult TryDeserialize(fsData data, ref object instance_, Type storageType)
    {
      IEnumerable instance = (IEnumerable) instance_;
      fsResult fsResult;
      if ((fsResult = fsResult.Success + this.CheckType(data, fsDataType.Array)).Failed)
        return fsResult;
      if (data.AsList.Count == 0)
        return fsResult.Success;
      if (instance is IList)
      {
        Type[] genericArguments = storageType.GetGenericArguments();
        if (genericArguments.Length == 1)
        {
          IList list = (IList) instance;
          Type storageType1 = genericArguments[0];
          for (int index = 0; index < data.AsList.Count; ++index)
          {
            object result = (object) null;
            this.Serializer.TryDeserialize(data.AsList[index], storageType1, ref result);
            list.Add(result);
          }
          return fsResult.Success;
        }
      }
      Type elementType = fsIEnumerableConverter.GetElementType(storageType);
      MethodInfo addMethod = fsIEnumerableConverter.GetAddMethod(storageType);
      MethodInfo flattenedMethod1 = storageType.GetFlattenedMethod("get_Item");
      MethodInfo flattenedMethod2 = storageType.GetFlattenedMethod("set_Item");
      if (flattenedMethod2 == (MethodInfo) null)
        fsIEnumerableConverter.TryClear(storageType, (object) instance);
      int existingSize = fsIEnumerableConverter.TryGetExistingSize(storageType, (object) instance);
      List<fsData> asList = data.AsList;
      for (int index = 0; index < asList.Count; ++index)
      {
        fsData data1 = asList[index];
        object result1 = (object) null;
        if (flattenedMethod1 != (MethodInfo) null && index < existingSize)
          result1 = flattenedMethod1.Invoke((object) instance, new object[1]
          {
            (object) index
          });
        fsResult result2 = this.Serializer.TryDeserialize(data1, elementType, ref result1);
        fsResult.AddMessages(result2);
        if (!result2.Failed)
        {
          if (flattenedMethod2 != (MethodInfo) null && index < existingSize)
            flattenedMethod2.Invoke((object) instance, new object[2]
            {
              (object) index,
              result1
            });
          else
            addMethod.Invoke((object) instance, new object[1]
            {
              result1
            });
        }
      }
      return fsResult;
    }

    private static int HintSize(IEnumerable collection)
    {
      return collection is ICollection ? ((ICollection) collection).Count : 0;
    }

    private static Type GetElementType(Type objectType)
    {
      if (objectType.HasElementType)
        return objectType.GetElementType();
      Type type = fsReflectionUtility.GetInterface(objectType, typeof (IEnumerable<>));
      return type != (Type) null ? type.GetGenericArguments()[0] : typeof (object);
    }

    private static void TryClear(Type type, object instance)
    {
      MethodInfo flattenedMethod = type.GetFlattenedMethod("Clear");
      if (!(flattenedMethod != (MethodInfo) null))
        return;
      flattenedMethod.Invoke(instance, (object[]) null);
    }

    private static int TryGetExistingSize(Type type, object instance)
    {
      PropertyInfo flattenedProperty = type.GetFlattenedProperty("Count");
      return flattenedProperty != (PropertyInfo) null ? (int) flattenedProperty.GetGetMethod().Invoke(instance, (object[]) null) : 0;
    }

    private static MethodInfo GetAddMethod(Type type)
    {
      Type type1 = fsReflectionUtility.GetInterface(type, typeof (ICollection<>));
      if (type1 != (Type) null)
      {
        MethodInfo declaredMethod = type1.GetDeclaredMethod("Add");
        if (declaredMethod != (MethodInfo) null)
          return declaredMethod;
      }
      MethodInfo addMethod = type.GetFlattenedMethod("Add");
      if ((object) addMethod == null)
        addMethod = type.GetFlattenedMethod("Push") ?? type.GetFlattenedMethod("Enqueue");
      return addMethod;
    }
  }
}
