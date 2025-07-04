﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class GUIStyleState_DirectConverter : fsDirectConverter<GUIStyleState>
  {
    protected override fsResult DoSerialize(
      GUIStyleState model,
      Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "background", model.background) + SerializeMember(serialized, null, "textColor", model.textColor);
    }

    protected override fsResult DoDeserialize(
      Dictionary<string, fsData> data,
      ref GUIStyleState model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "background", out Texture2D background);
      model.background = background;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "textColor", out Color textColor);
      model.textColor = textColor;
      return fsResult2;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new GUIStyleState();
    }
  }
}
