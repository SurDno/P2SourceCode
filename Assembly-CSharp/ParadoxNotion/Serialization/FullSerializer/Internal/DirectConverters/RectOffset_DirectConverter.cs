using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class RectOffset_DirectConverter : fsDirectConverter<RectOffset>
  {
    protected override fsResult DoSerialize(RectOffset model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember<int>(serialized, null, "bottom", model.bottom) + SerializeMember<int>(serialized, null, "left", model.left) + SerializeMember<int>(serialized, null, "right", model.right) + SerializeMember<int>(serialized, null, "top", model.top);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RectOffset model)
    {
      fsResult success = fsResult.Success;
      int bottom = model.bottom;
      fsResult fsResult1 = success + DeserializeMember(data, null, "bottom", out bottom);
      model.bottom = bottom;
      int left = model.left;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "left", out left);
      model.left = left;
      int right = model.right;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "right", out right);
      model.right = right;
      int top = model.top;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "top", out top);
      model.top = top;
      return fsResult4;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return (object) new RectOffset();
    }
  }
}
