using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace JerboaAnimationInstancing;

public class RuntimeHelper {
	public static Transform[] MergeBone(SkinnedMeshRenderer[] meshRender, List<Matrix4x4> bindPose) {
		if (Profiler.enabled)
			Profiler.BeginSample("MergeBone()");
		var transformList = new List<Transform>(150);
		for (var index1 = 0; index1 != meshRender.Length; ++index1) {
			var bones = meshRender[index1].bones;
			var bindposes = meshRender[index1].sharedMesh.bindposes;
			for (var j = 0; j != bones.Length; ++j) {
				var index2 = transformList.FindIndex(q => q == bones[j]);
				if (index2 < 0) {
					transformList.Add(bones[j]);
					bindPose?.Add(bindposes[j]);
				} else
					bindPose[index2] = bindposes[j];
			}

			meshRender[index1].enabled = false;
		}

		if (Profiler.enabled)
			Profiler.EndSample();
		return transformList.ToArray();
	}

	public static Quaternion QuaternionFromMatrix(Matrix4x4 mat) {
		Vector3 forward;
		forward.x = mat.m02;
		forward.y = mat.m12;
		forward.z = mat.m22;
		Vector3 upwards;
		upwards.x = mat.m01;
		upwards.y = mat.m11;
		upwards.z = mat.m21;
		return Quaternion.LookRotation(forward, upwards);
	}
}