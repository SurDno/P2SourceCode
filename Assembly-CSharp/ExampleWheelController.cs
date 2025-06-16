using UnityEngine;

public class ExampleWheelController : MonoBehaviour {
	public float acceleration;
	public Renderer motionVectorRenderer;
	private Rigidbody m_Rigidbody;

	private void Start() {
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Rigidbody.maxAngularVelocity = 100f;
	}

	private void Update() {
		if (Input.GetKey(KeyCode.UpArrow))
			m_Rigidbody.AddRelativeTorque(new Vector3(-1f * acceleration, 0.0f, 0.0f), ForceMode.Acceleration);
		else if (Input.GetKey(KeyCode.DownArrow))
			m_Rigidbody.AddRelativeTorque(new Vector3(1f * acceleration, 0.0f, 0.0f), ForceMode.Acceleration);
		var num = (float)(-(double)m_Rigidbody.angularVelocity.x / 100.0);
		if (!(bool)(Object)motionVectorRenderer)
			return;
		motionVectorRenderer.material.SetFloat(Uniforms._MotionAmount, Mathf.Clamp(num, -0.25f, 0.25f));
	}

	private static class Uniforms {
		internal static readonly int _MotionAmount = Shader.PropertyToID(nameof(_MotionAmount));
	}
}