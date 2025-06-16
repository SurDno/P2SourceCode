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
      float xMin = model.xMin;
      fsResult fsResult1 = success + DeserializeMember(data, null, "xMin", out xMin);
      model.xMin = xMin;
      float yMin = model.yMin;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "yMin", out yMin);
      model.yMin = yMin;
      float xMax = model.xMax;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "xMax", out xMax);
      model.xMax = xMax;
      float yMax = model.yMax;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "yMax", out yMax);
      model.yMax = yMax;
      return fsResult4;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new Rect();
    }
  }
}
