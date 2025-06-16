using UnityEngine;

public class PlagueCloud : MonoBehaviour {
	[Header("Settings")] public PlagueCloudParticlesSettings InvisibleSettings;
	public PlagueCloudParticlesSettings VisibleSettings;
	public PlagueCloudParticlesSettings AttackSettings;
	public float MovementSpeed;
	[Header("State")] [SerializeField] private VisibilityType visibility = VisibilityType.Invisible;
	public MovementType Movement;
	public Transform TargetTransform;
	public Vector3 TargetPosition;
	private PlagueCloudParticles[] particles;

	public VisibilityType Visibility {
		get => visibility;
		set {
			if (visibility == value)
				return;
			visibility = value;
			UpdateState();
		}
	}

	private void Start() {
		UpdateState();
	}

	private void Update() {
		if (Movement == MovementType.Custom || (Movement == MovementType.TowardTransform && TargetTransform == null))
			return;
		var position = transform.position;
		var vector3 = Movement == MovementType.TowardTransform ? TargetTransform.position : TargetPosition;
		var a = Vector3.MoveTowards(position, vector3, MovementSpeed * Time.deltaTime);
		transform.position = a;
		if (Vector3.Distance(a, vector3) <= 0.5)
			return;
		transform.LookAt(vector3);
	}

	private void UpdateState() {
		if (particles == null)
			particles = GetComponentsInChildren<PlagueCloudParticles>();
		for (var index = 0; index < particles.Length; ++index)
			switch (visibility) {
				case VisibilityType.Invisible:
					ApplySettings(particles[index], InvisibleSettings);
					break;
				case VisibilityType.Visible:
					ApplySettings(particles[index], VisibleSettings);
					break;
				case VisibilityType.Attack:
					ApplySettings(particles[index], AttackSettings);
					break;
			}
	}

	private void ApplySettings(PlagueCloudParticles particles, PlagueCloudParticlesSettings settings) {
		particles.EmissionRadius = settings.EmissionRadius;
		particles.Emission = settings.Emission;
		particles.LifeTime = settings.LifeTime;
		particles.Acceleration = settings.Acceleration;
		particles.Drag = settings.Drag;
		particles.NoiseFrequency = settings.NoiseFrequency;
		particles.NoiseAmplitude = settings.NoiseAmplitude;
		particles.NoiseMotion = settings.NoiseMotion;
		particles.Gravity = settings.Gravity;
		particles.GravitySphereRadius = settings.GravitySphereRadius;
		particles.GravityFadeRadius = settings.GravityFadeRadius;
		particles.GravityMovementPrediction = settings.GravityMovementPrediction;
		particles.Pattern = settings.Pattern;
		particles.PatternSize = settings.PatternSize;
		particles.PatternPlaneForce = settings.PatternPlaneForce;
		particles.PatternOrthogonalForce = settings.PatternOrthogonalForce;
		particles.PatternRandomForce = settings.PatternRandomForce;
	}

	public enum VisibilityType {
		Invisible,
		Visible,
		Attack
	}

	public enum MovementType {
		Custom,
		TowardPoint,
		TowardTransform
	}
}