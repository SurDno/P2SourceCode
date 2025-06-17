using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Gradient_DirectConverter : fsDirectConverter<Gradient>
  {
    protected override fsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "alphaKeys", model.alphaKeys) + SerializeMember(serialized, null, "colorKeys", model.colorKeys);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "alphaKeys", out GradientAlphaKey[] alphaKeys);
      model.alphaKeys = alphaKeys;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "colorKeys", out GradientColorKey[] colorKeys);
      model.colorKeys = colorKeys;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new Gradient();
    }
  }
}
