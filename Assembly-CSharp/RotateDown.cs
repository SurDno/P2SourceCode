using UnityEngine;

public class RotateDown : MonoBehaviour {
	[SerializeField] private float gravity = 30f;
	[SerializeField] private float drag = 1f;
	[SerializeField] private bool limited;
	[SerializeField] private float minLimit;
	[SerializeField] private float maxLimit;
	private float rotation;
	private float velocity;
	private Vector3 forwardBeforeAnimation;
	private bool wasEnabled;

	private void UpdateRotation() {
		velocity += forwardBeforeAnimation.y * gravity * Time.deltaTime;
		velocity *= Mathf.Max(0.0f, (float)(1.0 - Time.deltaTime * (double)drag));
		rotation += velocity * Time.deltaTime;
		if (limited) {
			if (rotation < (double)minLimit) {
				rotation = minLimit;
				if (velocity >= 0.0)
					return;
				velocity = 0.0f;
			} else {
				if (rotation <= (double)maxLimit)
					return;
				rotation = maxLimit;
				if (velocity > 0.0)
					velocity = 0.0f;
			}
		} else
			rotation = Mathf.Repeat(rotation, 360f);
	}

	private void LateUpdate() {
		if (wasEnabled)
			UpdateRotation();
		else {
			ResetRotation();
			wasEnabled = true;
		}

		transform.localEulerAngles = new Vector3(rotation, 0.0f, 0.0f);
	}

	private void OnDisable() {
		wasEnabled = false;
	}

	private void ResetRotation() {
		velocity = 0.0f;
		rotation = Vector3.SignedAngle(transform.parent.up, Vector3.up, transform.parent.right);
	}

	private void Update() {
		forwardBeforeAnimation = transform.forward;
	}
}