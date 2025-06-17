using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Keyframe_DirectConverter : fsDirectConverter<Keyframe>
  {
    protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "time", model.time) + SerializeMember(serialized, null, "value", model.value) + SerializeMember(serialized, null, "tangentMode", model.tangentMode) + SerializeMember(serialized, null, "inTangent", model.inTangent) + SerializeMember(serialized, null, "outTangent", model.outTangent);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "time", out float time);
      model.time = time;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "value", out float num);
      model.value = num;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "tangentMode", out int tangentMode);
      model.tangentMode = tangentMode;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "inTangent", out float inTangent);
      model.inTangent = inTangent;
      fsResult fsResult5 = fsResult4 + DeserializeMember(data, null, "outTangent", out float outTangent);
      model.outTangent = outTangent;
      return fsResult5;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new Keyframe();
    }
  }
}
