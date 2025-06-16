using UnityEngine;

namespace Rain
{
  [RequireComponent(typeof (ParticleSystem))]
  public class Bokeh : MonoBehaviour
  {
    public float maxRate = 10f;
    private ParticleSystem _system;

    private ParticleSystem system
    {
      get
      {
        if (_system == null)
          _system = GetComponent<ParticleSystem>();
        return _system;
      }
    }

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      float constant;
      if (instance != null)
      {
        float actualRainIntensity = instance.actualRainIntensity;
        Vector3 normalized = new Vector3(-instance.actualWindVector.x, 1f, -instance.actualWindVector.y).normalized;
        float num = Mathf.Clamp01((float) (Vector3.Dot(transform.forward, normalized) * 0.89999997615814209 + 0.10000000149011612));
        constant = num > 0.0 && !Physics.Raycast(transform.position, normalized, 50f) ? actualRainIntensity * (maxRate * num) : 0.0f;
      }
      else
        constant = 0.0f;
      ParticleSystem.EmissionModule emission = system.emission;
      emission.rateOverTime = new ParticleSystem.MinMaxCurve(constant);
    }
  }
}
