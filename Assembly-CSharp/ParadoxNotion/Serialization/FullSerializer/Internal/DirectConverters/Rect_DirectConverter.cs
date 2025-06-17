using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Rect_DirectConverter : fsDirectConverter<Rect>
  {
    protected override fsResult DoSerialize(Rect model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "xMin", model.xMin) + SerializeMember(serialized, null, "yMin", model.yMin) + SerializeMember(serialized, null, "xMax", model.xMax) + SerializeMember(serialized, null, "yMax", model.yMax);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Rect model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "xMin", out float xMin);
      model.xMin = xMin;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "yMin", out float yMin);
      model.yMin = yMin;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "xMax", out float xMax);
      model.xMax = xMax;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "yMax", out float yMax);
      model.yMax = yMax;
      return fsResult4;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new Rect();
    }
  }
}
