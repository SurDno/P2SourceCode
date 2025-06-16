using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

public class FireLightAnimation : MonoBehaviour {
	[SerializeField] private Vector3 positionAmplitude;
	[SerializeField] private Vector3 positionRate;
	[SerializeField] private Vector3 rotationAmplitude;
	[SerializeField] private Vector3 rotationRate;
	private Vector3 basePosition;
	private Vector3 baseRotation;

	private void Animate() {
		var transform = this.transform;
		var flag = InstanceByRequest<GraphicsGameSettings>.Instance.Antialiasing.Value;
		transform.localPosition =
			basePosition + Animate(flag ? positionAmplitude * 2.5f : positionAmplitude, positionRate);
		transform.localEulerAngles =
			baseRotation + Animate(flag ? rotationAmplitude * 2.5f : rotationAmplitude, rotationRate);
	}

	private float Animate(float amplitude, float rate) {
		return Mathf.Sin((float)(Time.time * (double)rate * 2.0 * 3.1415927410125732)) * amplitude;
	}

	private Vector3 Animate(Vector3 amplitude, Vector3 rate) {
		return new Vector3(Animate(amplitude.x, rate.x), Animate(amplitude.y, rate.y), Animate(amplitude.z, rate.z));
	}

	private void OnDisable() {
		var transform = this.transform;
		transform.localPosition = basePosition;
		transform.localEulerAngles = baseRotation;
	}

	private void OnEnable() {
		var transform = this.transform;
		basePosition = transform.localPosition;
		baseRotation = transform.localEulerAngles;
		Animate();
	}

	private void Update() {
		Animate();
	}
}