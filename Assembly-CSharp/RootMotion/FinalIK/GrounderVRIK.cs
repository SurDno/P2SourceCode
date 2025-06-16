using UnityEngine;

namespace RootMotion.FinalIK;

[AddComponentMenu("Scripts/RootMotion.FinalIK/Grounder/Grounder VRIK")]
public class GrounderVRIK : Grounder {
	[Tooltip("Reference to the VRIK componet.")]
	public VRIK ik;

	private Transform[] feet = new Transform[2];

	[ContextMenu("TUTORIAL VIDEO")]
	private void OpenTutorial() { }

	[ContextMenu("User Manual")]
	protected override void OpenUserManual() { }

	[ContextMenu("Scrpt Reference")]
	protected override void OpenScriptReference() { }

	public override void ResetPosition() {
		solver.Reset();
	}

	private bool IsReadyToInitiate() {
		return !(ik == null) && ik.solver.initiated;
	}

	private void Update() {
		weight = Mathf.Clamp(weight, 0.0f, 1f);
		if (weight <= 0.0 || initiated || !IsReadyToInitiate())
			return;
		Initiate();
	}

	private void Initiate() {
		feet = new Transform[2];
		feet[0] = ik.references.leftFoot;
		feet[1] = ik.references.rightFoot;
		var solver1 = ik.solver;
		solver1.OnPreUpdate = solver1.OnPreUpdate + OnSolverUpdate;
		var solver2 = ik.solver;
		solver2.OnPostUpdate = solver2.OnPostUpdate + OnPostSolverUpdate;
		solver.Initiate(ik.references.root, feet);
		initiated = true;
	}

	private void OnSolverUpdate() {
		if (!enabled || weight <= 0.0)
			return;
		if (OnPreGrounder != null)
			OnPreGrounder();
		solver.Update();
		ik.references.pelvis.position += solver.pelvis.IKOffset * weight;
		ik.solver.AddPositionOffset(IKSolverVR.PositionOffset.LeftFoot,
			(solver.legs[0].IKPosition - ik.references.leftFoot.position) * weight);
		ik.solver.AddPositionOffset(IKSolverVR.PositionOffset.RightFoot,
			(solver.legs[1].IKPosition - ik.references.rightFoot.position) * weight);
		if (OnPostGrounder == null)
			return;
		OnPostGrounder();
	}

	private void SetLegIK(
		IKSolverVR.PositionOffset positionOffset,
		Transform bone,
		Grounding.Leg leg) {
		ik.solver.AddPositionOffset(positionOffset, (leg.IKPosition - bone.position) * weight);
	}

	private void OnPostSolverUpdate() {
		ik.references.leftFoot.rotation = Quaternion.Slerp(Quaternion.identity, solver.legs[0].rotationOffset, weight) *
		                                  ik.references.leftFoot.rotation;
		ik.references.rightFoot.rotation =
			Quaternion.Slerp(Quaternion.identity, solver.legs[1].rotationOffset, weight) *
			ik.references.rightFoot.rotation;
	}

	private void OnDrawGizmosSelected() {
		if (ik == null)
			ik = GetComponent<VRIK>();
		if (ik == null)
			ik = GetComponentInParent<VRIK>();
		if (!(ik == null))
			return;
		ik = GetComponentInChildren<VRIK>();
	}

	private void OnDestroy() {
		if (!initiated || !(ik != null))
			return;
		var solver1 = ik.solver;
		solver1.OnPreUpdate = solver1.OnPreUpdate - OnSolverUpdate;
		var solver2 = ik.solver;
		solver2.OnPostUpdate = solver2.OnPostUpdate - OnPostSolverUpdate;
	}
}