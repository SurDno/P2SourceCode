using Engine.Source.Commons;

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
  private float timeToChange;
  private Vector2 targetWind = Vector2.zero;
  private Vector2 currentWind = Vector2.zero;
  private Vector2 windChangeVelocity = Vector2.zero;
  private bool active = true;

  private bool Active
  {
    get => active;
    set
    {
      if (active == value)
        return;
      active = value;
      if ((Object) particles == (Object) null)
        return;
      particles.emission.enabled = active;
    }
  }

  private void OnEnable() => Active = false;

  private void Update()
  {
    Active = (double) Vector3.Distance(this.transform.position, EngineApplication.PlayerPosition) < maxPlayerDistance;
    if (!Active)
      return;
    timeToChange -= Time.deltaTime;
    if (timeToChange <= 0.0)
    {
      timeToChange += changeTime;
      while (timeToChange <= 0.0)
        timeToChange += changeTime;
      targetWind = Random.insideUnitCircle;
    }
    currentWind = Vector2.SmoothDamp(currentWind, targetWind, ref windChangeVelocity, smoothness, float.MaxValue, Time.deltaTime);
    this.transform.eulerAngles = new Vector3(currentWind.y * bendAngle, 0.0f, -currentWind.x * bendAngle);
    UpdateParticles();
  }

  private void UpdateParticles()
  {
    if ((Object) particles == (Object) null)
      return;
    float magnitude = currentWind.magnitude;
    particles.emission.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(emissionRateRange.x, emissionRateRange.y, magnitude));
    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particles.velocityOverLifetime with
    {
      x = (ParticleSystem.MinMaxCurve) (currentWind.x * speed.x),
      y = (ParticleSystem.MinMaxCurve) (magnitude * speed.y),
      z = (ParticleSystem.MinMaxCurve) (currentWind.y * speed.x)
    };
  }
}
