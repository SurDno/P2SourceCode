// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters.RectOffset_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class RectOffset_DirectConverter : fsDirectConverter<RectOffset>
  {
    protected override fsResult DoSerialize(RectOffset model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<int>(serialized, (System.Type) null, "bottom", model.bottom) + this.SerializeMember<int>(serialized, (System.Type) null, "left", model.left) + this.SerializeMember<int>(serialized, (System.Type) null, "right", model.right) + this.SerializeMember<int>(serialized, (System.Type) null, "top", model.top);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RectOffset model)
    {
      fsResult success = fsResult.Success;
      int bottom = model.bottom;
      fsResult fsResult1 = success + this.DeserializeMember<int>(data, (System.Type) null, "bottom", out bottom);
      model.bottom = bottom;
      int left = model.left;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<int>(data, (System.Type) null, "left", out left);
      model.left = left;
      int right = model.right;
      fsResult fsResult3 = fsResult2 + this.DeserializeMember<int>(data, (System.Type) null, "right", out right);
      model.right = right;
      int top = model.top;
      fsResult fsResult4 = fsResult3 + this.DeserializeMember<int>(data, (System.Type) null, "top", out top);
      model.top = top;
      return fsResult4;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new RectOffset();
    }
  }
}
