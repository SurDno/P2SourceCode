[RequireComponent(typeof (Light))]
public class TOD_LightAtTime : MonoBehaviour
{
  public AnimationCurve Intensity = new AnimationCurve {
    keys = new Keyframe[3]
    {
      new Keyframe(0.0f, 0.0f),
      new Keyframe(12f, 1f),
      new Keyframe(24f, 0.0f)
    }
  };
  private Light lightComponent;

  protected void Start() => lightComponent = this.GetComponent<Light>();

  protected void Update()
  {
    lightComponent.intensity = Intensity.Evaluate(TOD_Sky.Instance.Cycle.Hour);
    lightComponent.enabled = (double) lightComponent.intensity > 0.0;
  }
}
