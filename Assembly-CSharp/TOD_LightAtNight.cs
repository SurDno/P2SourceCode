using UnityEngine;

[RequireComponent(typeof (Light))]
public class TOD_LightAtNight : MonoBehaviour
{
  public float fadeTime = 1f;
  private float lerpTime = 0.0f;
  private Light lightComponent;
  private float lightIntensity;

  protected void Start()
  {
    this.lightComponent = this.GetComponent<Light>();
    this.lightIntensity = this.lightComponent.intensity;
    this.lightComponent.enabled = TOD_Sky.Instance.IsNight;
  }

  protected void Update()
  {
    this.lerpTime = Mathf.Clamp01(this.lerpTime + (TOD_Sky.Instance.IsNight ? 1f : -1f) * Time.deltaTime / this.fadeTime);
    this.lightComponent.intensity = Mathf.Lerp(0.0f, this.lightIntensity, this.lerpTime);
    this.lightComponent.enabled = (double) this.lightComponent.intensity > 0.0;
  }
}
