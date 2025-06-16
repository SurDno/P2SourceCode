public class LightFlicker : MonoBehaviour
{
  private float baseIntensity;
  public bool flicker = true;
  public float flickerIntensity = 0.5f;
  private Light lightComp;

  private void Awake()
  {
    lightComp = this.gameObject.GetComponent<Light>();
    baseIntensity = lightComp.intensity;
  }

  private void Update()
  {
    if (!flicker)
      return;
    lightComp.intensity = Mathf.Lerp(baseIntensity - flickerIntensity, baseIntensity, Mathf.PerlinNoise(Random.Range(0.0f, 1000f), Time.time));
  }
}
