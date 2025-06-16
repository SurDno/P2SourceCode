using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Bounds_DirectConverter : fsDirectConverter<Bounds>
  {
    protected override fsResult DoSerialize(Bounds model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember<Vector3>(serialized, null, "center", model.center) + SerializeMember<Vector3>(serialized, null, "size", model.size);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Bounds model)
    {
      fsResult success = fsResult.Success;
      Vector3 center = model.center;
      fsResult fsResult1 = success + DeserializeMember(data, null, "center", out center);
      model.center = center;
      Vector3 size = model.size;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "size", out size);
      model.size = size;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return (object) new Bounds();
    }
  }
}
