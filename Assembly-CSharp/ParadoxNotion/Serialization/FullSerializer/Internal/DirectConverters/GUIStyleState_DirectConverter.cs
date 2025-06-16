// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters.GUIStyleState_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class GUIStyleState_DirectConverter : fsDirectConverter<GUIStyleState>
  {
    protected override fsResult DoSerialize(
      GUIStyleState model,
      Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<Texture2D>(serialized, (System.Type) null, "background", model.background) + this.SerializeMember<Color>(serialized, (System.Type) null, "textColor", model.textColor);
    }

    protected override fsResult DoDeserialize(
      Dictionary<string, fsData> data,
      ref GUIStyleState model)
    {
      fsResult success = fsResult.Success;
      Texture2D background = model.background;
      fsResult fsResult1 = success + this.DeserializeMember<Texture2D>(data, (System.Type) null, "background", out background);
      model.background = background;
      Color textColor = model.textColor;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<Color>(data, (System.Type) null, "textColor", out textColor);
      model.textColor = textColor;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new GUIStyleState();
    }
  }
}
