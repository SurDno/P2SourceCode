using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters;

public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve> {
	protected override fsResult DoSerialize(
		AnimationCurve model,
		Dictionary<string, fsData> serialized) {
		return fsResult.Success + SerializeMember(serialized, null, "keys", model.keys) +
		       SerializeMember(serialized, null, "preWrapMode", model.preWrapMode) +
		       SerializeMember(serialized, null, "postWrapMode", model.postWrapMode);
	}

	protected override fsResult DoDeserialize(
		Dictionary<string, fsData> data,
		ref AnimationCurve model) {
		var success = fsResult.Success;
		var keys = model.keys;
		var fsResult1 = success + DeserializeMember(data, null, "keys", out keys);
		model.keys = keys;
		var preWrapMode = model.preWrapMode;
		var fsResult2 = fsResult1 + DeserializeMember(data, null, "preWrapMode", out preWrapMode);
		model.preWrapMode = preWrapMode;
		var postWrapMode = model.postWrapMode;
		var fsResult3 = fsResult2 + DeserializeMember(data, null, "postWrapMode", out postWrapMode);
		model.postWrapMode = postWrapMode;
		return fsResult3;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return new AnimationCurve();
	}
}