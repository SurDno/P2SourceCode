using System;
using System.Collections.Generic;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsKeyValuePairConverter : fsConverter
  {
    public override bool CanProcess(Type type)
    {
      return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof (KeyValuePair<,>);
    }

    public override bool RequestCycleSupport(Type storageType) => false;

    public override bool RequestInheritanceSupport(Type storageType) => false;

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      fsResult fsResult1;
      if ((fsResult1 = fsResult.Success + CheckKey(data, "Key", out fsData subitem1)).Failed)
        return fsResult1;
      fsResult fsResult2;
      if ((fsResult2 = fsResult1 + CheckKey(data, "Value", out fsData subitem2)).Failed)
        return fsResult2;
      Type[] genericArguments = storageType.GetGenericArguments();
      Type storageType1 = genericArguments[0];
      Type storageType2 = genericArguments[1];
      object result1 = null;
      object result2 = null;
      fsResult2.AddMessages(Serializer.TryDeserialize(subitem1, storageType1, ref result1));
      fsResult2.AddMessages(Serializer.TryDeserialize(subitem2, storageType2, ref result2));
      instance = Activator.CreateInstance(storageType, result1, result2);
      return fsResult2;
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      PropertyInfo declaredProperty1 = storageType.GetDeclaredProperty("Key");
      PropertyInfo declaredProperty2 = storageType.GetDeclaredProperty("Value");
      object instance1 = declaredProperty1.GetValue(instance, null);
      object instance2 = declaredProperty2.GetValue(instance, null);
      Type[] genericArguments = storageType.GetGenericArguments();
      Type storageType1 = genericArguments[0];
      Type storageType2 = genericArguments[1];
      fsResult success = fsResult.Success;
      success.AddMessages(Serializer.TrySerialize(storageType1, instance1, out fsData data1));
      success.AddMessages(Serializer.TrySerialize(storageType2, instance2, out fsData data2));
      serialized = fsData.CreateDictionary();
      if (data1 != null)
        serialized.AsDictionary["Key"] = data1;
      if (data2 != null)
        serialized.AsDictionary["Value"] = data2;
      return success;
    }
  }
}
