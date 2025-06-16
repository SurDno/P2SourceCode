using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class GUIStyleState_DirectConverter : fsDirectConverter<GUIStyleState>
  {
    protected override fsResult DoSerialize(
      GUIStyleState model,
      Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember<Texture2D>(serialized, null, "background", model.background) + SerializeMember<Color>(serialized, null, "textColor", model.textColor);
    }

    protected override fsResult DoDeserialize(
      Dictionary<string, fsData> data,
      ref GUIStyleState model)
    {
      fsResult success = fsResult.Success;
      Texture2D background = model.background;
      fsResult fsResult1 = success + DeserializeMember(data, null, "background", out background);
      model.background = background;
      Color textColor = model.textColor;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "textColor", out textColor);
      model.textColor = textColor;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return (object) new GUIStyleState();
    }
  }
}
