using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve>
  {
    protected override fsResult DoSerialize(
      AnimationCurve model,
      Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember<Keyframe[]>(serialized, null, "keys", model.keys) + SerializeMember<WrapMode>(serialized, null, "preWrapMode", model.preWrapMode) + SerializeMember<WrapMode>(serialized, null, "postWrapMode", model.postWrapMode);
    }

    protected override fsResult DoDeserialize(
      Dictionary<string, fsData> data,
      ref AnimationCurve model)
    {
      fsResult success = fsResult.Success;
      Keyframe[] keys = model.keys;
      fsResult fsResult1 = success + DeserializeMember(data, null, "keys", out keys);
      model.keys = keys;
      WrapMode preWrapMode = model.preWrapMode;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "preWrapMode", out preWrapMode);
      model.preWrapMode = preWrapMode;
      WrapMode postWrapMode = model.postWrapMode;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "postWrapMode", out postWrapMode);
      model.postWrapMode = postWrapMode;
      return fsResult3;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return (object) new AnimationCurve();
    }
  }
}
