using System;

[Serializable]
public class WeightedBlendShape {
	public string blendName;
	public float weight;
	public int blendIdx = -1;

	public WeightedBlendShape() {
		blendName = "";
		weight = 0.0f;
	}

	public WeightedBlendShape(string _blendName, float _weight) {
		blendName = _blendName;
		weight = _weight;
	}

	public WeightedBlendShape(string _blendName, float _weight, int _blendIdx) {
		blendName = _blendName;
		weight = _weight;
		blendIdx = _blendIdx;
	}
}