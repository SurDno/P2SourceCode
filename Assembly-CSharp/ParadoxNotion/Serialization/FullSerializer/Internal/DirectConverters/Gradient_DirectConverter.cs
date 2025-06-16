using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Gradient_DirectConverter : fsDirectConverter<Gradient>
  {
    protected override fsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<GradientAlphaKey[]>(serialized, (System.Type) null, "alphaKeys", model.alphaKeys) + this.SerializeMember<GradientColorKey[]>(serialized, (System.Type) null, "colorKeys", model.colorKeys);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
    {
      fsResult success = fsResult.Success;
      GradientAlphaKey[] alphaKeys = model.alphaKeys;
      fsResult fsResult1 = success + this.DeserializeMember<GradientAlphaKey[]>(data, (System.Type) null, "alphaKeys", out alphaKeys);
      model.alphaKeys = alphaKeys;
      GradientColorKey[] colorKeys = model.colorKeys;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<GradientColorKey[]>(data, (System.Type) null, "colorKeys", out colorKeys);
      model.colorKeys = colorKeys;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new Gradient();
    }
  }
}
