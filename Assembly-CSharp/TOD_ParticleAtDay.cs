using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TOD_ParticleAtDay : MonoBehaviour {
	public float fadeTime = 1f;
	private float lerpTime;
	private ParticleSystem particleComponent;
	private float particleEmission;

	protected void Start() {
		particleComponent = GetComponent<ParticleSystem>();
		particleEmission = particleComponent.emissionRate;
		particleComponent.emissionRate = TOD_Sky.Instance.IsDay ? particleEmission : 0.0f;
	}

	protected void Update() {
		lerpTime = Mathf.Clamp01(lerpTime + (TOD_Sky.Instance.IsDay ? 1f : -1f) * Time.deltaTime / fadeTime);
		particleComponent.emissionRate = Mathf.Lerp(0.0f, particleEmission, lerpTime);
	}
}