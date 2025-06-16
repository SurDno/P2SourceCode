using UnityEngine;

public class PlagueCloud : MonoBehaviour
{
  [Header("Settings")]
  public PlagueCloudParticlesSettings InvisibleSettings;
  public PlagueCloudParticlesSettings VisibleSettings;
  public PlagueCloudParticlesSettings AttackSettings;
  public float MovementSpeed;
  [Header("State")]
  [SerializeField]
  private PlagueCloud.VisibilityType visibility = PlagueCloud.VisibilityType.Invisible;
  public PlagueCloud.MovementType Movement;
  public Transform TargetTransform;
  public Vector3 TargetPosition;
  private PlagueCloudParticles[] particles;

  public PlagueCloud.VisibilityType Visibility
  {
    get => this.visibility;
    set
    {
      if (this.visibility == value)
        return;
      this.visibility = value;
      this.UpdateState();
    }
  }

  private void Start() => this.UpdateState();

  private void Update()
  {
    if (this.Movement == PlagueCloud.MovementType.Custom || this.Movement == PlagueCloud.MovementType.TowardTransform && (Object) this.TargetTransform == (Object) null)
      return;
    Vector3 position = this.transform.position;
    Vector3 vector3 = this.Movement == PlagueCloud.MovementType.TowardTransform ? this.TargetTransform.position : this.TargetPosition;
    Vector3 a = Vector3.MoveTowards(position, vector3, this.MovementSpeed * Time.deltaTime);
    this.transform.position = a;
    if ((double) Vector3.Distance(a, vector3) <= 0.5)
      return;
    this.transform.LookAt(vector3);
  }

  private void UpdateState()
  {
    if (this.particles == null)
      this.particles = this.GetComponentsInChildren<PlagueCloudParticles>();
    for (int index = 0; index < this.particles.Length; ++index)
    {
      switch (this.visibility)
      {
        case PlagueCloud.VisibilityType.Invisible:
          this.ApplySettings(this.particles[index], this.InvisibleSettings);
          break;
        case PlagueCloud.VisibilityType.Visible:
          this.ApplySettings(this.particles[index], this.VisibleSettings);
          break;
        case PlagueCloud.VisibilityType.Attack:
          this.ApplySettings(this.particles[index], this.AttackSettings);
          break;
      }
    }
  }

  private void ApplySettings(PlagueCloudParticles particles, PlagueCloudParticlesSettings settings)
  {
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

  public enum VisibilityType
  {
    Invisible,
    Visible,
    Attack,
  }

  public enum MovementType
  {
    Custom,
    TowardPoint,
    TowardTransform,
  }
}
