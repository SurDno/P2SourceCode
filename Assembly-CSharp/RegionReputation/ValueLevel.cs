using System;

namespace RegionReputation;

[Serializable]
public struct ValueLevel {
	public string Signature;
	public float Threshold;

	public static string GetSignature(ValueLevel[] levels, float value, bool greater = false) {
		for (var index = 0; index < levels.Length; ++index)
			if (greater ? value > (double)levels[index].Threshold : value < (double)levels[index].Threshold)
				return levels[index].Signature;
		return null;
	}
}