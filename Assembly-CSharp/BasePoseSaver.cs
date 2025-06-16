using System.Collections.Generic;
using UnityEngine;

public class BasePoseSaver : MonoBehaviour {
	public BonePose[] PoseList;

	public void CommitBones() {
		var componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
		var bonePoseList = new List<BonePose>();
		foreach (var bx in componentsInChildren)
			if (bx != transform) {
				var bonePose = new BonePose();
				bonePose.InitializeBone(bx);
				bonePoseList.Add(bonePose);
			}

		PoseList = new BonePose[bonePoseList.Count];
		var index = 0;
		foreach (var bonePose in bonePoseList) {
			PoseList[index] = bonePose;
			++index;
		}
	}

	public void ShowBones() {
		if (PoseList == null)
			return;
		foreach (var pose in PoseList)
			pose.ResetToThisTransform();
	}
}