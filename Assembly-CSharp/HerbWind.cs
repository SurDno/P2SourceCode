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
  private ParticleSystem particles;
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
      if (particles == null)
        return;
      ParticleSystem.EmissionModule emission = particles.emission;
      emission.enabled = active;
    }
  }

  private void OnEnable() => Active = false;

  private void Update()
  {
    Active = Vector3.Distance(transform.position, EngineApplication.PlayerPosition) < (double) maxPlayerDistance;
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
    transform.eulerAngles = new Vector3(currentWind.y * bendAngle, 0.0f, -currentWind.x * bendAngle);
    UpdateParticles();
  }

  private void UpdateParticles()
  {
    if (particles == null)
      return;
    float magnitude = currentWind.magnitude;
    
    ParticleSystem.EmissionModule emission = particles.emission;
    emission.rateOverTime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(emissionRateRange.x, emissionRateRange.y, magnitude));
    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particles.velocityOverLifetime with
    {
      x = currentWind.x * speed.x,
      y = magnitude * speed.y,
      z = currentWind.y * speed.x
    };
  }
}
