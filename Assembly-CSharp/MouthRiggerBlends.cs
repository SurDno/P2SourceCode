using System;
using System.Collections.Generic;
using UnityEngine;

public class MouthRiggerBlends : MonoBehaviour {
	public SkinnedMeshRenderer skinnedMesh;
	public Visemes_Count_t VisemeSet = Visemes_Count_t._Please_Set_;
	private string VisemeSelector;
	public VisemeBlendDefine[] VisemeBlends;
	public PhonemeVisemeMapping phnMap;
	private bool bExpandedList;
	private bool bExpandedBoneList;
	private AnnoBlendDeformer visemeBlendDeformer;
	public BonePose[] BasePoses;

	public IAnnoDeformer MouthDeformer {
		get {
			if (visemeBlendDeformer == null)
				MakeRuntimeDeformer();
			return visemeBlendDeformer;
		}
	}

	private void MakeRuntimeDeformer() {
		if (visemeBlendDeformer != null || phnMap == null || VisemeBlends == null)
			return;
		if (skinnedMesh == null)
			skinnedMesh = GetComponent<SkinnedMeshRenderer>();
		UpdateBlendIndices(skinnedMesh, VisemeBlends);
		visemeBlendDeformer =
			new AnnoBlendDeformer(VisemeBlends, GetActiveBlendIndices(skinnedMesh, VisemeBlends), skinnedMesh);
	}

	private void Start() {
		MakeRuntimeDeformer();
	}

	public bool VisemesExpanded {
		get => bExpandedList;
		set => bExpandedList = value;
	}

	public bool BoneListExpanded {
		get => bExpandedBoneList;
		set => bExpandedBoneList = value;
	}

	private void Update() { }

	public Visemes_Count_t VisemeConfig {
		get => VisemeSet;
		set {
			if (value == VisemeSet)
				return;
			VisemeSet = value;
			InstantiatePhnToVis();
		}
	}

	public void InstantiatePhnToVis() {
		switch (VisemeSet) {
			case Visemes_Count_t._Please_Set_:
				phnMap = null;
				break;
			case Visemes_Count_t._9_Visemes:
				var visemes9 = new Visemes9();
				phnMap = new PhonemeVisemeMapping(visemes9.visNames, visemes9.mapping);
				break;
			case Visemes_Count_t._12_Visemes:
				var visemes12 = new Visemes12();
				phnMap = new PhonemeVisemeMapping(visemes12.visNames, visemes12.mapping);
				break;
			case Visemes_Count_t._17_Visemes:
				var visemes17 = new Visemes17();
				phnMap = new PhonemeVisemeMapping(visemes17.visNames, visemes17.mapping);
				break;
		}
	}

	public string CurrentViseme {
		get => VisemeSelector;
		set => VisemeSelector = value;
	}

	public int GetPopupInfo(out string[] list) {
		list = null;
		if (phnMap == null)
			InstantiatePhnToVis();
		if (phnMap == null)
			return -1;
		list = phnMap.GetVisemeNames();
		if (list == null)
			return -1;
		for (var popupInfo = 0; popupInfo < list.Length; ++popupInfo)
			if (list[popupInfo] == VisemeSelector)
				return popupInfo;
		return 0;
	}

	public VisemeBlendDefine GetViseme(string which) {
		if (VisemeBlends == null)
			VisemeBlends = new VisemeBlendDefine[0];
		foreach (var visemeBlend in VisemeBlends) {
			foreach (var thePhone in visemeBlend.thePhones)
				if (string.Compare(thePhone, which, StringComparison.OrdinalIgnoreCase) == 0)
					return visemeBlend;
		}

		var length = VisemeBlends.Length;
		var visemeBlendDefineArray = new VisemeBlendDefine[length + 1];
		for (var index = 0; index < length; ++index)
			visemeBlendDefineArray[index] = VisemeBlends[index];
		var phonemes = phnMap.GetPhonemes(which);
		visemeBlendDefineArray[length] = new VisemeBlendDefine(phonemes.phns);
		VisemeBlends = visemeBlendDefineArray;
		return visemeBlendDefineArray[length];
	}

	public static void SaveGUIToBlendDefine(SkinnedMeshRenderer obj, VisemeBlendDefine bn) {
		var weightedBlendShapeList = new List<WeightedBlendShape>();
		var blendShapeCount = obj.sharedMesh.blendShapeCount;
		for (var index = 0; index < blendShapeCount; ++index) {
			var blendShapeWeight = obj.GetBlendShapeWeight(index);
			if (blendShapeWeight > 0.001) {
				var blendShapeName = obj.sharedMesh.GetBlendShapeName(index);
				if (blendShapeName != null)
					weightedBlendShapeList.Add(new WeightedBlendShape(blendShapeName, blendShapeWeight, index));
			}
		}

		bn.theBlends = new WeightedBlendShape[weightedBlendShapeList.Count];
		for (var index = 0; index < weightedBlendShapeList.Count; ++index)
			bn.theBlends[index] = weightedBlendShapeList[index];
	}

	public void CommitBonesForCurrentViseme() {
		SaveGUIToBlendDefine(skinnedMesh, GetViseme(CurrentViseme));
	}

	public void DebugPrint() {
		var message = "";
		if (VisemeBlends == null)
			return;
		foreach (var visemeBlend in VisemeBlends) {
			foreach (var thePhone in visemeBlend.thePhones)
				message = message + thePhone + " ";
			foreach (var theBlend in visemeBlend.theBlends)
				message = message + theBlend.blendName + "," + theBlend.weight;
			message += "\n";
		}

		Debug.Log(message);
	}

	private static int BlendNameToBlendIndex(SkinnedMeshRenderer obj, string blendName) {
		var blendShapeCount = obj.sharedMesh.blendShapeCount;
		for (var shapeIndex = 0; shapeIndex < blendShapeCount; ++shapeIndex)
			if (obj.sharedMesh.GetBlendShapeName(shapeIndex) == blendName)
				return shapeIndex;
		Debug.Log("Missing Blend Shape for blend " + blendName);
		return -1;
	}

	public static void UpdateBlendIndices(SkinnedMeshRenderer obj, VisemeBlendDefine[] blends) {
		foreach (var blend in blends) {
			foreach (var theBlend in blend.theBlends) {
				var blendIndex = BlendNameToBlendIndex(obj, theBlend.blendName);
				theBlend.blendIdx = blendIndex;
			}
		}
	}

	public static Dictionary<int, float> GetActiveBlendIndices(
		SkinnedMeshRenderer obj,
		VisemeBlendDefine[] blends) {
		UpdateBlendIndices(obj, blends);
		var activeBlendIndices = new Dictionary<int, float>();
		foreach (var blend in blends) {
			foreach (var theBlend in blend.theBlends)
				if (!activeBlendIndices.ContainsKey(theBlend.blendIdx))
					activeBlendIndices.Add(theBlend.blendIdx, 0.0f);
		}

		return activeBlendIndices;
	}

	public void ShowViseme(string which) {
		var viseme = GetViseme(which);
		var activeBlendIndices = GetActiveBlendIndices(skinnedMesh, VisemeBlends);
		foreach (var theBlend in viseme.theBlends)
			activeBlendIndices[theBlend.blendIdx] += theBlend.weight;
		foreach (var keyValuePair in activeBlendIndices)
			skinnedMesh.SetBlendShapeWeight(keyValuePair.Key, keyValuePair.Value);
	}
}