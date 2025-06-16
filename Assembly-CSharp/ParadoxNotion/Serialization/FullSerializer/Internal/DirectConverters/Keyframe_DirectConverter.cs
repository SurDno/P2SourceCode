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
      float time = model.time;
      fsResult fsResult1 = success + DeserializeMember(data, null, "time", out time);
      model.time = time;
      float num = model.value;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "value", out num);
      model.value = num;
      int tangentMode = model.tangentMode;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "tangentMode", out tangentMode);
      model.tangentMode = tangentMode;
      float inTangent = model.inTangent;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "inTangent", out inTangent);
      model.inTangent = inTangent;
      float outTangent = model.outTangent;
      fsResult fsResult5 = fsResult4 + DeserializeMember(data, null, "outTangent", out outTangent);
      model.outTangent = outTangent;
      return fsResult5;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new Keyframe();
    }
  }
}
