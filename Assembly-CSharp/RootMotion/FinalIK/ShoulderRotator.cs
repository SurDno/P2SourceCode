using UnityEngine;

namespace RootMotion.FinalIK;

public class ShoulderRotator : MonoBehaviour {
	[Tooltip("Weight of shoulder rotation")]
	public float weight = 1.5f;

	[Tooltip("The greater the offset, the sooner the shoulder will start rotating")]
	public float offset = 0.2f;

	private FullBodyBipedIK ik;
	private bool skip;

	private void Start() {
		ik = GetComponent<FullBodyBipedIK>();
		var solver = ik.solver;
		solver.OnPostUpdate = solver.OnPostUpdate + RotateShoulders;
	}

	private void RotateShoulders() {
		if (ik == null || ik.solver.IKPositionWeight <= 0.0)
			return;
		if (skip)
			skip = false;
		else {
			RotateShoulder(FullBodyBipedChain.LeftArm, weight, offset);
			RotateShoulder(FullBodyBipedChain.RightArm, weight, offset);
			skip = true;
			ik.solver.Update();
		}
	}

	private void RotateShoulder(FullBodyBipedChain chain, float weight, float offset) {
		var quaternion = Quaternion.Lerp(Quaternion.identity,
			Quaternion.FromToRotation(GetParentBoneMap(chain).swingDirection,
				ik.solver.GetEndEffector(chain).position - GetParentBoneMap(chain).transform.position),
			Mathf.Clamp(
				((float)((ik.solver.GetEndEffector(chain).position - ik.solver.GetLimbMapping(chain).bone1.position)
				         .magnitude /
				         (double)(ik.solver.GetChain(chain).nodes[0].length +
				                  ik.solver.GetChain(chain).nodes[1].length) -
				         1.0) + offset) * weight, 0.0f, 1f) * ik.solver.GetEndEffector(chain).positionWeight *
			ik.solver.IKPositionWeight);
		ik.solver.GetLimbMapping(chain).parentBone.rotation =
			quaternion * ik.solver.GetLimbMapping(chain).parentBone.rotation;
	}

	private IKMapping.BoneMap GetParentBoneMap(FullBodyBipedChain chain) {
		return ik.solver.GetLimbMapping(chain).GetBoneMap(IKMappingLimb.BoneMapType.Parent);
	}

	private void OnDestroy() {
		if (!(ik != null))
			return;
		var solver = ik.solver;
		solver.OnPostUpdate = solver.OnPostUpdate - RotateShoulders;
	}
}