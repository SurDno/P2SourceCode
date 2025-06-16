using UnityEngine;

namespace RootMotion.FinalIK;

public class BodyTilt : OffsetModifier {
	[Tooltip("Speed of tilting")] public float tiltSpeed = 6f;
	[Tooltip("Sensitivity of tilting")] public float tiltSensitivity = 0.07f;
	[Tooltip("The OffsetPose components")] public OffsetPose poseLeft;
	[Tooltip("The OffsetPose components")] public OffsetPose poseRight;
	private float tiltAngle;
	private Vector3 lastForward;

	protected override void Start() {
		base.Start();
		lastForward = transform.forward;
	}

	protected override void OnModifyOffset() {
		var rotation = Quaternion.FromToRotation(lastForward, transform.forward);
		var angle = 0.0f;
		var axis = Vector3.zero;
		rotation.ToAngleAxis(out angle, out axis);
		if (axis.y > 0.0)
			angle = -angle;
		tiltAngle = Mathf.Lerp(tiltAngle, Mathf.Clamp(angle * (tiltSensitivity * 0.01f) / deltaTime, -1f, 1f),
			deltaTime * tiltSpeed);
		var weight = Mathf.Abs(tiltAngle) / 1f;
		if (tiltAngle < 0.0)
			poseRight.Apply(ik.solver, weight);
		else
			poseLeft.Apply(ik.solver, weight);
		lastForward = transform.forward;
	}
}