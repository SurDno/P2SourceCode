// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters.Gradient_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
