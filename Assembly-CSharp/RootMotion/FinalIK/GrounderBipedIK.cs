using UnityEngine;

namespace RootMotion.FinalIK;

[HelpURL("http://www.root-motion.com/finalikdox/html/page11.html")]
[AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder Biped")]
public class GrounderBipedIK : Grounder {
	[Tooltip("The BipedIK componet.")] public BipedIK ik;

	[Tooltip("The amount of spine bending towards upward slopes.")]
	public float spineBend = 7f;

	[Tooltip("The interpolation speed of spine bending.")]
	public float spineSpeed = 3f;

	private Transform[] feet = new Transform[2];
	private Quaternion[] footRotations = new Quaternion[2];
	private Vector3 animatedPelvisLocalPosition;
	private Vector3 solvedPelvisLocalPosition;
	private Vector3 spineOffset;
	private float lastWeight;

	[ContextMenu("User Manual")]
	protected override void OpenUserManual() {
		Application.OpenURL("http://www.root-motion.com/finalikdox/html/page11.html");
	}

	[ContextMenu("Scrpt Reference")]
	protected override void OpenScriptReference() {
		Application.OpenURL(
			"http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_grounder_biped_i_k.html");
	}

	public override void ResetPosition() {
		solver.Reset();
		spineOffset = Vector3.zero;
	}

	private bool IsReadyToInitiate() {
		return !(ik == null) && ik.solvers.leftFoot.initiated && ik.solvers.rightFoot.initiated;
	}

	private void Update() {
		weight = Mathf.Clamp(weight, 0.0f, 1f);
		if (weight <= 0.0 || initiated || !IsReadyToInitiate())
			return;
		Initiate();
	}

	private void Initiate() {
		feet = new Transform[2];
		footRotations = new Quaternion[2];
		feet[0] = ik.references.leftFoot;
		feet[1] = ik.references.rightFoot;
		footRotations[0] = Quaternion.identity;
		footRotations[1] = Quaternion.identity;
		var spine = ik.solvers.spine;
		spine.OnPreUpdate = spine.OnPreUpdate + OnSolverUpdate;
		var rightFoot = ik.solvers.rightFoot;
		rightFoot.OnPostUpdate = rightFoot.OnPostUpdate + OnPostSolverUpdate;
		animatedPelvisLocalPosition = ik.references.pelvis.localPosition;
		solver.Initiate(ik.references.root, feet);
		initiated = true;
	}

	private void OnDisable() {
		if (!initiated)
			return;
		ik.solvers.leftFoot.IKPositionWeight = 0.0f;
		ik.solvers.rightFoot.IKPositionWeight = 0.0f;
	}

	private void OnSolverUpdate() {
		if (!enabled)
			return;
		if (weight <= 0.0) {
			if (lastWeight <= 0.0)
				return;
			OnDisable();
		}

		lastWeight = weight;
		if (OnPreGrounder != null)
			OnPreGrounder();
		if (ik.references.pelvis.localPosition != solvedPelvisLocalPosition)
			animatedPelvisLocalPosition = ik.references.pelvis.localPosition;
		else
			ik.references.pelvis.localPosition = animatedPelvisLocalPosition;
		solver.Update();
		ik.references.pelvis.position += solver.pelvis.IKOffset * weight;
		SetLegIK(ik.solvers.leftFoot, 0);
		SetLegIK(ik.solvers.rightFoot, 1);
		if (spineBend != 0.0 && ik.references.spine.Length != 0) {
			spineSpeed = Mathf.Clamp(spineSpeed, 0.0f, spineSpeed);
			spineOffset = Vector3.Lerp(spineOffset, GetSpineOffsetTarget() * weight * spineBend,
				Time.deltaTime * spineSpeed);
			var rotation1 = ik.references.leftUpperArm.rotation;
			var rotation2 = ik.references.rightUpperArm.rotation;
			var up = solver.up;
			ik.references.spine[0].rotation =
				Quaternion.FromToRotation(up, up + spineOffset) * ik.references.spine[0].rotation;
			ik.references.leftUpperArm.rotation = rotation1;
			ik.references.rightUpperArm.rotation = rotation2;
		}

		if (OnPostGrounder == null)
			return;
		OnPostGrounder();
	}

	private void SetLegIK(IKSolverLimb limb, int index) {
		footRotations[index] = feet[index].rotation;
		limb.IKPosition = solver.legs[index].IKPosition;
		limb.IKPositionWeight = weight;
	}

	private void OnPostSolverUpdate() {
		if (weight <= 0.0 || !enabled)
			return;
		for (var index = 0; index < feet.Length; ++index)
			feet[index].rotation = Quaternion.Slerp(Quaternion.identity, solver.legs[index].rotationOffset, weight) *
			                       footRotations[index];
		solvedPelvisLocalPosition = ik.references.pelvis.localPosition;
	}

	private void OnDestroy() {
		if (!initiated || !(ik != null))
			return;
		var spine = ik.solvers.spine;
		spine.OnPreUpdate = spine.OnPreUpdate - OnSolverUpdate;
		var rightFoot = ik.solvers.rightFoot;
		rightFoot.OnPostUpdate = rightFoot.OnPostUpdate - OnPostSolverUpdate;
	}
}