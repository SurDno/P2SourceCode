using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Bounds_DirectConverter : fsDirectConverter<Bounds>
  {
    protected override fsResult DoSerialize(Bounds model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "center", model.center) + SerializeMember(serialized, null, "size", model.size);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Bounds model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "center", out Vector3 center);
      model.center = center;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "size", out Vector3 size);
      model.size = size;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new Bounds();
    }
  }
}
