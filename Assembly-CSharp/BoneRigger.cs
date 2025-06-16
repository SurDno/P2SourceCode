using System.Collections.Generic;
using UnityEngine;

public class BoneRigger : MonoBehaviour {
	public string RiggerName;
	public string[] poseNames;
	public string basePose;
	[HideInInspector] public Transform[] BoneList;
	[HideInInspector] public VisemeBoneDefine[] Poses;
	[HideInInspector] public string editorSelected;
	private AnnoBoneDeformer rtBoneDeformer;

	private void Start() {
		InitializeRuntimeDeformer();
	}

	private void InitializeRuntimeDeformer() {
		if (rtBoneDeformer != null || Poses.Length == 0)
			return;
		var _labelToBoneDict = new Dictionary<string, VisemeBoneDefine>();
		VisemeBoneDefine _basePose = null;
		foreach (var pose in Poses) {
			_labelToBoneDict.Add(pose.Name, pose);
			if (pose.Name == basePose)
				_basePose = pose;
		}

		if (_basePose != null && _labelToBoneDict.Count > 0)
			rtBoneDeformer = new AnnoBoneDeformer(_labelToBoneDict, _basePose);
	}

	public IAnnoDeformer BoneDeformer {
		get {
			if (rtBoneDeformer == null)
				InitializeRuntimeDeformer();
			return rtBoneDeformer;
		}
	}

	public string Selected {
		get {
			if (editorSelected == null || editorSelected.Length == 0)
				editorSelected = basePose;
			if ((editorSelected == null || editorSelected.Length == 0) && poseNames != null && poseNames.Length != 0)
				editorSelected = poseNames[0];
			return editorSelected;
		}
		set => editorSelected = value;
	}

	public int SelectedIndex {
		get {
			var selected = Selected;
			for (var selectedIndex = 0; poseNames != null && selectedIndex < poseNames.Length; ++selectedIndex)
				if (poseNames[selectedIndex] == selected)
					return selectedIndex;
			return -1;
		}
		set {
			if (value < 0 || value >= poseNames.Length)
				return;
			Selected = poseNames[value];
		}
	}

	public void RefreshPoseList() {
		var visemeBoneDefineArray = new VisemeBoneDefine[poseNames.Length];
		for (var index = 0; index < poseNames.Length; ++index) {
			var visemeBoneDefine = GetPose(poseNames[index]) ?? new VisemeBoneDefine(poseNames[index]);
			visemeBoneDefineArray[index] = visemeBoneDefine;
		}

		Poses = visemeBoneDefineArray;
	}

	public VisemeBoneDefine GetPose(string which) {
		if (Poses == null) {
			Debug.Log("GetPose problem: no poses?");
			return null;
		}

		foreach (var pose in Poses)
			if (pose != null && pose.m_visemeLabel == which)
				return pose;
		return null;
	}

	public void CommitSelected() {
		GetPose(Selected).RecordBonePositions(BoneList);
	}

	public void ShowSelected() {
		GetPose(Selected).ResetToThisPose();
	}

	public bool HasBonedPose(string which) {
		var pose = GetPose(which);
		return pose != null && pose != null && pose.HasPose;
	}

	private void Update() { }
}