using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public abstract class fsBaseConverter
  {
    public fsSerializer Serializer;

    public virtual object CreateInstance(fsData data, Type storageType)
    {
      return !RequestCycleSupport(storageType) ? (object) storageType : throw new InvalidOperationException("Please override CreateInstance for " + GetType().FullName + "; the object graph for " + storageType + " can contain potentially contain cycles, so separated instance creation is needed");
    }

    public virtual bool RequestCycleSupport(Type storageType)
    {
      return !(storageType == typeof (string)) && (storageType.Resolve().IsClass || storageType.Resolve().IsInterface);
    }

    public virtual bool RequestInheritanceSupport(Type storageType)
    {
      return !storageType.Resolve().IsSealed;
    }

    public abstract fsResult TrySerialize(object instance, out fsData serialized, Type storageType);

    public abstract fsResult TryDeserialize(fsData data, ref object instance, Type storageType);

    protected fsResult FailExpectedType(fsData data, params fsDataType[] types)
    {
      return fsResult.Fail(GetType().Name + " expected one of " + string.Join(", ", types.Select(t => t.ToString()).ToArray()) + " but got " + data.Type + " in " + data);
    }

    protected fsResult CheckType(fsData data, fsDataType type)
    {
      if (data.Type == type)
        return fsResult.Success;
      return fsResult.Fail(GetType().Name + " expected " + type + " but got " + data.Type + " in " + data);
    }

    protected fsResult CheckKey(fsData data, string key, out fsData subitem)
    {
      return CheckKey(data.AsDictionary, key, out subitem);
    }

    protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
    {
      if (data.TryGetValue(key, out subitem))
        return fsResult.Success;
      return fsResult.Fail(GetType().Name + " requires a <" + key + "> key in the data " + data);
    }

    protected fsResult SerializeMember<T>(
      Dictionary<string, fsData> data,
      Type overrideConverterType,
      string name,
      T value)
    {
      fsData data1;
      fsResult fsResult = Serializer.TrySerialize(typeof (T), overrideConverterType, value, out data1);
      if (fsResult.Succeeded)
        data[name] = data1;
      return fsResult;
    }

    protected fsResult DeserializeMember<T>(
      Dictionary<string, fsData> data,
      Type overrideConverterType,
      string name,
      out T value)
    {
      fsData data1;
      if (!data.TryGetValue(name, out data1))
      {
        value = default (T);
        return fsResult.Fail("Unable to find member \"" + name + "\"");
      }
      object result = null;
      fsResult fsResult = Serializer.TryDeserialize(data1, typeof (T), overrideConverterType, ref result);
      value = (T) result;
      return fsResult;
    }
  }
}
