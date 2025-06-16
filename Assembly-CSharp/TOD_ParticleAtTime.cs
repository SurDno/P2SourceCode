using UnityEngine;

[RequireComponent(typeof (ParticleSystem))]
public class TOD_ParticleAtTime : MonoBehaviour
{
  public AnimationCurve Emission = new AnimationCurve {
    keys = new Keyframe[3]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(12f, 1f),
      new Keyframe(24f, 0.0f)
    }
  };
  private ParticleSystem particleComponent;

  protected void Start() => particleComponent = GetComponent<ParticleSystem>();

  protected void Update()
  {
    particleComponent.emissionRate = Emission.Evaluate(TOD_Sky.Instance.Cycle.Hour);
  }
}
