using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsNullableConverter : fsConverter
  {
    public override bool CanProcess(Type type)
    {
      return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      return this.Serializer.TrySerialize(Nullable.GetUnderlyingType(storageType), instance, out serialized);
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      return this.Serializer.TryDeserialize(data, Nullable.GetUnderlyingType(storageType), ref instance);
    }

    public override object CreateInstance(fsData data, Type storageType) => (object) storageType;
  }
}
