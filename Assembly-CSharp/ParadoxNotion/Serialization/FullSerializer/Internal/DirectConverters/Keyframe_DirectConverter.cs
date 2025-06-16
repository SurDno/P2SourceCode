using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class Keyframe_DirectConverter : fsDirectConverter<Keyframe>
  {
    protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<float>(serialized, (System.Type) null, "time", model.time) + this.SerializeMember<float>(serialized, (System.Type) null, "value", model.value) + this.SerializeMember<int>(serialized, (System.Type) null, "tangentMode", model.tangentMode) + this.SerializeMember<float>(serialized, (System.Type) null, "inTangent", model.inTangent) + this.SerializeMember<float>(serialized, (System.Type) null, "outTangent", model.outTangent);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model)
    {
      fsResult success = fsResult.Success;
      float time = model.time;
      fsResult fsResult1 = success + this.DeserializeMember<float>(data, (System.Type) null, "time", out time);
      model.time = time;
      float num = model.value;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<float>(data, (System.Type) null, "value", out num);
      model.value = num;
      int tangentMode = model.tangentMode;
      fsResult fsResult3 = fsResult2 + this.DeserializeMember<int>(data, (System.Type) null, "tangentMode", out tangentMode);
      model.tangentMode = tangentMode;
      float inTangent = model.inTangent;
      fsResult fsResult4 = fsResult3 + this.DeserializeMember<float>(data, (System.Type) null, "inTangent", out inTangent);
      model.inTangent = inTangent;
      float outTangent = model.outTangent;
      fsResult fsResult5 = fsResult4 + this.DeserializeMember<float>(data, (System.Type) null, "outTangent", out outTangent);
      model.outTangent = outTangent;
      return fsResult5;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new Keyframe();
    }
  }
}
