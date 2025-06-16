using System.Collections;
using UnityEngine;

namespace RootMotion.FinalIK;

public abstract class OffsetModifierVRIK : MonoBehaviour {
	[Tooltip("The master weight")] public float weight = 1f;

	[Tooltip("Reference to the VRIK component")]
	public VRIK ik;

	private float lastTime;

	protected float deltaTime => Time.time - lastTime;

	protected abstract void OnModifyOffset();

	protected virtual void Start() {
		StartCoroutine(Initiate());
	}

	private IEnumerator Initiate() {
		while (ik == null)
			yield return null;
		var solver = ik.solver;
		solver.OnPreUpdate = solver.OnPreUpdate + ModifyOffset;
		lastTime = Time.time;
	}

	private void ModifyOffset() {
		if (!enabled || weight <= 0.0 || deltaTime <= 0.0 || ik == null)
			return;
		weight = Mathf.Clamp(weight, 0.0f, 1f);
		OnModifyOffset();
		lastTime = Time.time;
	}

	protected virtual void OnDestroy() {
		if (!(ik != null))
			return;
		var solver = ik.solver;
		solver.OnPreUpdate = solver.OnPreUpdate - ModifyOffset;
	}
}