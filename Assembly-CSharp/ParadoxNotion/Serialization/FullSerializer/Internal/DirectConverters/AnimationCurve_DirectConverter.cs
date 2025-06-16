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
      return fsResult.Success + this.SerializeMember<Keyframe[]>(serialized, (System.Type) null, "keys", model.keys) + this.SerializeMember<WrapMode>(serialized, (System.Type) null, "preWrapMode", model.preWrapMode) + this.SerializeMember<WrapMode>(serialized, (System.Type) null, "postWrapMode", model.postWrapMode);
    }

    protected override fsResult DoDeserialize(
      Dictionary<string, fsData> data,
      ref AnimationCurve model)
    {
      fsResult success = fsResult.Success;
      Keyframe[] keys = model.keys;
      fsResult fsResult1 = success + this.DeserializeMember<Keyframe[]>(data, (System.Type) null, "keys", out keys);
      model.keys = keys;
      WrapMode preWrapMode = model.preWrapMode;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<WrapMode>(data, (System.Type) null, "preWrapMode", out preWrapMode);
      model.preWrapMode = preWrapMode;
      WrapMode postWrapMode = model.postWrapMode;
      fsResult fsResult3 = fsResult2 + this.DeserializeMember<WrapMode>(data, (System.Type) null, "postWrapMode", out postWrapMode);
      model.postWrapMode = postWrapMode;
      return fsResult3;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new AnimationCurve();
    }
  }
}
