using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Rect_DirectConverter : fsDirectConverter<Rect>
  {
    protected override fsResult DoSerialize(Rect model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<float>(serialized, (System.Type) null, "xMin", model.xMin) + this.SerializeMember<float>(serialized, (System.Type) null, "yMin", model.yMin) + this.SerializeMember<float>(serialized, (System.Type) null, "xMax", model.xMax) + this.SerializeMember<float>(serialized, (System.Type) null, "yMax", model.yMax);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Rect model)
    {
      fsResult success = fsResult.Success;
      float xMin = model.xMin;
      fsResult fsResult1 = success + this.DeserializeMember<float>(data, (System.Type) null, "xMin", out xMin);
      model.xMin = xMin;
      float yMin = model.yMin;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<float>(data, (System.Type) null, "yMin", out yMin);
      model.yMin = yMin;
      float xMax = model.xMax;
      fsResult fsResult3 = fsResult2 + this.DeserializeMember<float>(data, (System.Type) null, "xMax", out xMax);
      model.xMax = xMax;
      float yMax = model.yMax;
      fsResult fsResult4 = fsResult3 + this.DeserializeMember<float>(data, (System.Type) null, "yMax", out yMax);
      model.yMax = yMax;
      return fsResult4;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new Rect();
    }
  }
}
