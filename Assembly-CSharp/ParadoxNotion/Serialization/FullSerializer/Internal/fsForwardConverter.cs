using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsForwardConverter(fsForwardAttribute attribute) : fsConverter 
  {
    private string _memberName = attribute.MemberName;

    public override bool CanProcess(Type type)
    {
      throw new NotSupportedException("Please use the [fsForward(...)] attribute.");
    }

    private fsResult GetProperty(object instance, out fsMetaProperty property)
    {
      fsMetaProperty[] properties = fsMetaType.Get(Serializer.Config, instance.GetType()).Properties;
      for (int index = 0; index < properties.Length; ++index)
      {
        if (properties[index].MemberName == _memberName)
        {
          property = properties[index];
          return fsResult.Success;
        }
      }
      property = null;
      return fsResult.Fail("No property named \"" + _memberName + "\" on " + instance.GetType().CSharpName());
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      serialized = fsData.Null;
      fsResult fsResult;
      if ((fsResult = fsResult.Success + GetProperty(instance, out fsMetaProperty property)).Failed)
        return fsResult;
      object instance1 = property.Read(instance);
      return Serializer.TrySerialize(property.StorageType, instance1, out serialized);
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      fsResult fsResult1;
      if ((fsResult1 = fsResult.Success + GetProperty(instance, out fsMetaProperty property)).Failed)
        return fsResult1;
      object result = null;
      fsResult fsResult2;
      if ((fsResult2 = fsResult1 + Serializer.TryDeserialize(data, property.StorageType, ref result)).Failed)
        return fsResult2;
      property.Write(instance, result);
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
    }
  }
}
