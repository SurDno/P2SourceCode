[RequireComponent(typeof (ParticleSystem))]
public class TOD_ParticleAtNight : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime;
  private ParticleSystem particleComponent;
  private float particleEmission;

  protected void Start()
  {
    particleComponent = this.GetComponent<ParticleSystem>();
    particleEmission = particleComponent.emissionRate;
    particleComponent.emissionRate = TOD_Sky.Instance.IsNight ? particleEmission : 0.0f;
  }

  protected void Update()
  {
    lerpTime = Mathf.Clamp01(lerpTime + (TOD_Sky.Instance.IsNight ? 1f : -1f) * Time.deltaTime / fadeTime);
    particleComponent.emissionRate = Mathf.Lerp(0.0f, particleEmission, lerpTime);
  }
}
