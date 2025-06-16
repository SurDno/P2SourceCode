[RequireComponent(typeof (Light))]
public class TOD_LightAtNight : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime;
  private Light lightComponent;
  private float lightIntensity;

  protected void Start()
  {
    lightComponent = this.GetComponent<Light>();
    lightIntensity = lightComponent.intensity;
    lightComponent.enabled = TOD_Sky.Instance.IsNight;
  }

  protected void Update()
  {
    lerpTime = Mathf.Clamp01(lerpTime + (TOD_Sky.Instance.IsNight ? 1f : -1f) * Time.deltaTime / fadeTime);
    lightComponent.intensity = Mathf.Lerp(0.0f, lightIntensity, lerpTime);
    lightComponent.enabled = (double) lightComponent.intensity > 0.0;
  }
}
