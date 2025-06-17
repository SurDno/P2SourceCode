using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve>
  {
    protected override fsResult DoSerialize(
      AnimationCurve model,
      Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "keys", model.keys) + SerializeMember(serialized, null, "preWrapMode", model.preWrapMode) + SerializeMember(serialized, null, "postWrapMode", model.postWrapMode);
    }

    protected override fsResult DoDeserialize(
      Dictionary<string, fsData> data,
      ref AnimationCurve model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "keys", out Keyframe[] keys);
      model.keys = keys;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "preWrapMode", out WrapMode preWrapMode);
      model.preWrapMode = preWrapMode;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "postWrapMode", out WrapMode postWrapMode);
      model.postWrapMode = postWrapMode;
      return fsResult3;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new AnimationCurve();
    }
  }
}
