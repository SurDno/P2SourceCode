using System;
using UnityEngine;

namespace RootMotion.FinalIK;

public class HitReactionVRIK : OffsetModifierVRIK {
	public AnimationCurve[] offsetCurves;

	[Tooltip("Hit points for the FBBIK effectors")]
	public PositionOffset[] positionOffsets;

	[Tooltip(" Hit points for bones without an effector, such as the head")]
	public RotationOffset[] rotationOffsets;

	protected override void OnModifyOffset() {
		foreach (Offset positionOffset in positionOffsets)
			positionOffset.Apply(ik, offsetCurves, weight);
		foreach (Offset rotationOffset in rotationOffsets)
			rotationOffset.Apply(ik, offsetCurves, weight);
	}

	public void Hit(Collider collider, Vector3 force, Vector3 point) {
		if (ik == null)
			Debug.LogError("No IK assigned in HitReaction");
		else {
			foreach (var positionOffset in positionOffsets)
				if (positionOffset.collider == collider)
					positionOffset.Hit(force, offsetCurves, point);
			foreach (var rotationOffset in rotationOffsets)
				if (rotationOffset.collider == collider)
					rotationOffset.Hit(force, offsetCurves, point);
		}
	}

	[Serializable]
	public abstract class Offset {
		[Tooltip("Just for visual clarity, not used at all")]
		public string name;

		[Tooltip("Linking this hit point to a collider")]
		public Collider collider;

		[Tooltip("Only used if this hit point gets hit when already processing another hit")] [SerializeField]
		private float crossFadeTime = 0.1f;

		private float length;
		private float crossFadeSpeed;
		private float lastTime;

		protected float crossFader { get; private set; }

		protected float timer { get; private set; }

		protected Vector3 force { get; private set; }

		protected Vector3 point { get; private set; }

		public void Hit(Vector3 force, AnimationCurve[] curves, Vector3 point) {
			if (length == 0.0)
				length = GetLength(curves);
			if (length <= 0.0)
				Debug.LogError("Hit Point WeightCurve length is zero.");
			else {
				if (timer < 1.0)
					crossFader = 0.0f;
				crossFadeSpeed = crossFadeTime > 0.0 ? 1f / crossFadeTime : 0.0f;
				CrossFadeStart();
				timer = 0.0f;
				this.force = force;
				this.point = point;
			}
		}

		public void Apply(VRIK ik, AnimationCurve[] curves, float weight) {
			var num = Time.time - lastTime;
			lastTime = Time.time;
			if (timer >= (double)length)
				return;
			timer = Mathf.Clamp(timer + num, 0.0f, length);
			crossFader = crossFadeSpeed <= 0.0 ? 1f : Mathf.Clamp(crossFader + num * crossFadeSpeed, 0.0f, 1f);
			OnApply(ik, curves, weight);
		}

		protected abstract float GetLength(AnimationCurve[] curves);

		protected abstract void CrossFadeStart();

		protected abstract void OnApply(VRIK ik, AnimationCurve[] curves, float weight);
	}

	[Serializable]
	public class PositionOffset : Offset {
		[Tooltip("Offset magnitude in the direction of the hit force")]
		public int forceDirCurveIndex;

		[Tooltip("Offset magnitude in the direction of character.up")]
		public int upDirCurveIndex = 1;

		[Tooltip("Linking this offset to the VRIK position offsets")]
		public PositionOffsetLink[] offsetLinks;

		protected override float GetLength(AnimationCurve[] curves) {
			var max = curves[forceDirCurveIndex].keys.Length != 0
				? curves[forceDirCurveIndex].keys[curves[forceDirCurveIndex].length - 1].time
				: 0.0f;
			var min = curves[upDirCurveIndex].keys.Length != 0
				? curves[upDirCurveIndex].keys[curves[upDirCurveIndex].length - 1].time
				: 0.0f;
			return Mathf.Clamp(max, min, max);
		}

		protected override void CrossFadeStart() {
			foreach (var offsetLink in offsetLinks)
				offsetLink.CrossFadeStart();
		}

		protected override void OnApply(VRIK ik, AnimationCurve[] curves, float weight) {
			var vector3 = ik.transform.up * force.magnitude;
			var offset = (curves[forceDirCurveIndex].Evaluate(timer) * force +
			              curves[upDirCurveIndex].Evaluate(timer) * vector3) * weight;
			foreach (var offsetLink in offsetLinks)
				offsetLink.Apply(ik, offset, crossFader);
		}

		[Serializable]
		public class PositionOffsetLink {
			[Tooltip("The FBBIK effector type")] public IKSolverVR.PositionOffset positionOffset;

			[Tooltip("The weight of this effector (could also be negative)")]
			public float weight;

			private Vector3 lastValue;
			private Vector3 current;

			public void Apply(VRIK ik, Vector3 offset, float crossFader) {
				current = Vector3.Lerp(lastValue, offset * weight, crossFader);
				ik.solver.AddPositionOffset(positionOffset, current);
			}

			public void CrossFadeStart() {
				lastValue = current;
			}
		}
	}

	[Serializable]
	public class RotationOffset : Offset {
		[Tooltip("The angle to rotate the bone around it's rigidbody's world center of mass")]
		public int curveIndex;

		[Tooltip("Linking this hit point to bone(s)")]
		public RotationOffsetLink[] offsetLinks;

		private Rigidbody rigidbody;

		protected override float GetLength(AnimationCurve[] curves) {
			return curves[curveIndex].keys.Length != 0
				? curves[curveIndex].keys[curves[curveIndex].length - 1].time
				: 0.0f;
		}

		protected override void CrossFadeStart() {
			foreach (var offsetLink in offsetLinks)
				offsetLink.CrossFadeStart();
		}

		protected override void OnApply(VRIK ik, AnimationCurve[] curves, float weight) {
			if (collider == null)
				Debug.LogError("No collider assigned for a HitPointBone in the HitReaction component.");
			else {
				if (rigidbody == null)
					rigidbody = collider.GetComponent<Rigidbody>();
				if (!(rigidbody != null))
					return;
				var axis = Vector3.Cross(force, point - rigidbody.worldCenterOfMass);
				var offset = Quaternion.AngleAxis(curves[curveIndex].Evaluate(timer) * weight, axis);
				foreach (var offsetLink in offsetLinks)
					offsetLink.Apply(ik, offset, crossFader);
			}
		}

		[Serializable]
		public class RotationOffsetLink {
			[Tooltip("Reference to the bone that this hit point rotates")]
			public IKSolverVR.RotationOffset rotationOffset;

			[Tooltip("Weight of rotating the bone")] [Range(0.0f, 1f)]
			public float weight;

			private Quaternion lastValue = Quaternion.identity;
			private Quaternion current = Quaternion.identity;

			public void Apply(VRIK ik, Quaternion offset, float crossFader) {
				current = Quaternion.Lerp(lastValue, Quaternion.Lerp(Quaternion.identity, offset, weight), crossFader);
				ik.solver.AddRotationOffset(rotationOffset, current);
			}

			public void CrossFadeStart() {
				lastValue = current;
			}
		}
	}
}