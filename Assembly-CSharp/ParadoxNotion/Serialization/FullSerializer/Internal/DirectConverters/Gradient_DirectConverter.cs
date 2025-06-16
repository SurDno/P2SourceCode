using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Gradient_DirectConverter : fsDirectConverter<Gradient>
  {
    protected override fsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember<GradientAlphaKey[]>(serialized, null, "alphaKeys", model.alphaKeys) + SerializeMember<GradientColorKey[]>(serialized, null, "colorKeys", model.colorKeys);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
    {
      fsResult success = fsResult.Success;
      GradientAlphaKey[] alphaKeys = model.alphaKeys;
      fsResult fsResult1 = success + DeserializeMember(data, null, "alphaKeys", out alphaKeys);
      model.alphaKeys = alphaKeys;
      GradientColorKey[] colorKeys = model.colorKeys;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "colorKeys", out colorKeys);
      model.colorKeys = colorKeys;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return (object) new Gradient();
    }
  }
}
