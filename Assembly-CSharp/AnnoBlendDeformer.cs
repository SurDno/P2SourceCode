using System.Collections.Generic;
using UnityEngine;

public class AnnoBlendDeformer : IAnnoDeformer {
	private SkinnedMeshRenderer skinnedMesh;
	private Dictionary<int, float> BlendIdxToWeight;
	private int[] activeBlendIndices;
	private Dictionary<string, VisemeBlendDefine> _P2V;

	public AnnoBlendDeformer(
		VisemeBlendDefine[] _blendPoses,
		Dictionary<int, float> blendIndices,
		SkinnedMeshRenderer _skinnedMesh) {
		skinnedMesh = _skinnedMesh;
		BlendIdxToWeight = blendIndices;
		activeBlendIndices = new int[blendIndices.Keys.Count];
		var index = 0;
		foreach (var key in blendIndices.Keys) {
			activeBlendIndices[index] = key;
			++index;
		}

		_P2V = new Dictionary<string, VisemeBlendDefine>();
		foreach (var blendPose in _blendPoses) {
			foreach (var thePhone in blendPose.thePhones)
				_P2V.Add(thePhone, blendPose);
		}
	}

	public void Start() {
		foreach (var activeBlendIndex in activeBlendIndices)
			BlendIdxToWeight[activeBlendIndex] = 0.0f;
	}

	public void Blend(string sLabel, float weight) {
		VisemeBlendDefine visemeBlendDefine;
		if (!_P2V.TryGetValue(sLabel, out visemeBlendDefine))
			return;
		foreach (var theBlend in visemeBlendDefine.theBlends)
			BlendIdxToWeight[theBlend.blendIdx] += weight * theBlend.weight;
	}

	public void End() {
		foreach (var keyValuePair in BlendIdxToWeight)
			skinnedMesh.SetBlendShapeWeight(keyValuePair.Key, keyValuePair.Value);
	}
}