using Engine.Source.Commons;
using UnityEngine;

public class HerbWind : MonoBehaviour
{
  [SerializeField]
  private float maxPlayerDistance = 25f;
  [Space]
  [SerializeField]
  private float changeTime = 3f;
  [SerializeField]
  private float smoothness = 1f;
  [SerializeField]
  private float bendAngle = 5f;
  [Space]
  [SerializeField]
  private ParticleSystem particles = (ParticleSystem) null;
  [SerializeField]
  private Vector2 speed = Vector2.one;
  [SerializeField]
  private Vector2 emissionRateRange = new Vector2(1f, 2f);
  private float timeToChange = 0.0f;
  private Vector2 targetWind = Vector2.zero;
  private Vector2 currentWind = Vector2.zero;
  private Vector2 windChangeVelocity = Vector2.zero;
  private bool active = true;

  private bool Active
  {
    get => this.active;
    set
    {
      if (this.active == value)
        return;
      this.active = value;
      if ((Object) this.particles == (Object) null)
        return;
      this.particles.emission.enabled = this.active;
    }
  }

  private void OnEnable() => this.Active = false;

  private void Update()
  {
    this.Active = (double) Vector3.Distance(this.transform.position, EngineApplication.PlayerPosition) < (double) this.maxPlayerDistance;
    if (!this.Active)
      return;
    this.timeToChange -= Time.deltaTime;
    if ((double) this.timeToChange <= 0.0)
    {
      this.timeToChange += this.changeTime;
      while ((double) this.timeToChange <= 0.0)
        this.timeToChange += this.changeTime;
      this.targetWind = Random.insideUnitCircle;
    }
    this.currentWind = Vector2.SmoothDamp(this.currentWind, this.targetWind, ref this.windChangeVelocity, this.smoothness, float.MaxValue, Time.deltaTime);
    this.transform.eulerAngles = new Vector3(this.currentWind.y * this.bendAngle, 0.0f, -this.currentWind.x * this.bendAngle);
    this.UpdateParticles();
  }

  private void UpdateParticles()
  {
    if ((Object) this.particles == (Object) null)
      return;
    float magnitude = this.currentWind.magnitude;
    this.particles.emission.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(this.emissionRateRange.x, this.emissionRateRange.y, magnitude));
    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.particles.velocityOverLifetime with
    {
      x = (ParticleSystem.MinMaxCurve) (this.currentWind.x * this.speed.x),
      y = (ParticleSystem.MinMaxCurve) (magnitude * this.speed.y),
      z = (ParticleSystem.MinMaxCurve) (this.currentWind.y * this.speed.x)
    };
  }
}
