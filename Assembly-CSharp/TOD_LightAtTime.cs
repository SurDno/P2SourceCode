using UnityEngine;

[RequireComponent(typeof (Light))]
public class TOD_LightAtTime : MonoBehaviour
{
  public AnimationCurve Intensity = new() {
    keys = [
      new(0.0f, 0.0f),
      new(12f, 1f),
      new(24f, 0.0f)
    ]
  };
  private Light lightComponent;

  protected void Start() => lightComponent = GetComponent<Light>();

  protected void Update()
  {
    lightComponent.intensity = Intensity.Evaluate(TOD_Sky.Instance.Cycle.Hour);
    lightComponent.enabled = lightComponent.intensity > 0.0;
  }
}
