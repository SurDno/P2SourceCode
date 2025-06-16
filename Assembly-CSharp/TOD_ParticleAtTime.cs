using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TOD_ParticleAtTime : MonoBehaviour {
	public AnimationCurve Emission = new() {
		keys = new Keyframe[3] {
			new(0.0f, 0.0f),
			new(12f, 1f),
			new(24f, 0.0f)
		}
	};

	private ParticleSystem particleComponent;

	protected void Start() {
		particleComponent = GetComponent<ParticleSystem>();
	}

	protected void Update() {
		particleComponent.emissionRate = Emission.Evaluate(TOD_Sky.Instance.Cycle.Hour);
	}
}