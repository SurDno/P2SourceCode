using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters;

public class Keyframe_DirectConverter : fsDirectConverter<Keyframe> {
	protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized) {
		return fsResult.Success + SerializeMember(serialized, null, "time", model.time) +
		       SerializeMember(serialized, null, "value", model.value) +
		       SerializeMember(serialized, null, "tangentMode", model.tangentMode) +
		       SerializeMember(serialized, null, "inTangent", model.inTangent) +
		       SerializeMember(serialized, null, "outTangent", model.outTangent);
	}

	protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model) {
		var success = fsResult.Success;
		var time = model.time;
		var fsResult1 = success + DeserializeMember(data, null, "time", out time);
		model.time = time;
		var num = model.value;
		var fsResult2 = fsResult1 + DeserializeMember(data, null, "value", out num);
		model.value = num;
		var tangentMode = model.tangentMode;
		var fsResult3 = fsResult2 + DeserializeMember(data, null, "tangentMode", out tangentMode);
		model.tangentMode = tangentMode;
		var inTangent = model.inTangent;
		var fsResult4 = fsResult3 + DeserializeMember(data, null, "inTangent", out inTangent);
		model.inTangent = inTangent;
		var outTangent = model.outTangent;
		var fsResult5 = fsResult4 + DeserializeMember(data, null, "outTangent", out outTangent);
		model.outTangent = outTangent;
		return fsResult5;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return new Keyframe();
	}
}