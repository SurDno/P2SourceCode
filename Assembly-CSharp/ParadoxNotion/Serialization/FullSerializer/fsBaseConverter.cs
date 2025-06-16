// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsBaseConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public abstract class fsBaseConverter
  {
    public fsSerializer Serializer;

    public virtual object CreateInstance(fsData data, Type storageType)
    {
      return !this.RequestCycleSupport(storageType) ? (object) storageType : throw new InvalidOperationException("Please override CreateInstance for " + this.GetType().FullName + "; the object graph for " + (object) storageType + " can contain potentially contain cycles, so separated instance creation is needed");
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
      return fsResult.Fail(this.GetType().Name + " expected one of " + string.Join(", ", ((IEnumerable<fsDataType>) types).Select<fsDataType, string>((Func<fsDataType, string>) (t => t.ToString())).ToArray<string>()) + " but got " + (object) data.Type + " in " + (object) data);
    }

    protected fsResult CheckType(fsData data, fsDataType type)
    {
      if (data.Type == type)
        return fsResult.Success;
      return fsResult.Fail(this.GetType().Name + " expected " + (object) type + " but got " + (object) data.Type + " in " + (object) data);
    }

    protected fsResult CheckKey(fsData data, string key, out fsData subitem)
    {
      return this.CheckKey(data.AsDictionary, key, out subitem);
    }

    protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
    {
      if (data.TryGetValue(key, out subitem))
        return fsResult.Success;
      return fsResult.Fail(this.GetType().Name + " requires a <" + key + "> key in the data " + (object) data);
    }

    protected fsResult SerializeMember<T>(
      Dictionary<string, fsData> data,
      Type overrideConverterType,
      string name,
      T value)
    {
      fsData data1;
      fsResult fsResult = this.Serializer.TrySerialize(typeof (T), overrideConverterType, (object) value, out data1);
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
      object result = (object) null;
      fsResult fsResult = this.Serializer.TryDeserialize(data1, typeof (T), overrideConverterType, ref result);
      value = (T) result;
      return fsResult;
    }
  }
}
