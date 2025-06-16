using Engine.Source.Commons;
using Engine.Source.Settings;

public class FireLightAnimation : MonoBehaviour
{
  [SerializeField]
  private Vector3 positionAmplitude;
  [SerializeField]
  private Vector3 positionRate;
  [SerializeField]
  private Vector3 rotationAmplitude;
  [SerializeField]
  private Vector3 rotationRate;
  private Vector3 basePosition;
  private Vector3 baseRotation;

  private void Animate()
  {
    Transform transform = this.transform;
    bool flag = InstanceByRequest<GraphicsGameSettings>.Instance.Antialiasing.Value;
    transform.localPosition = basePosition + Animate(flag ? positionAmplitude * 2.5f : positionAmplitude, positionRate);
    transform.localEulerAngles = baseRotation + Animate(flag ? rotationAmplitude * 2.5f : rotationAmplitude, rotationRate);
  }

  private float Animate(float amplitude, float rate)
  {
    return Mathf.Sin((float) ((double) Time.time * rate * 2.0 * 3.1415927410125732)) * amplitude;
  }

  private Vector3 Animate(Vector3 amplitude, Vector3 rate)
  {
    return new Vector3(this.Animate(amplitude.x, rate.x), this.Animate(amplitude.y, rate.y), this.Animate(amplitude.z, rate.z));
  }

  private void OnDisable()
  {
    Transform transform = this.transform;
    transform.localPosition = basePosition;
    transform.localEulerAngles = baseRotation;
  }

  private void OnEnable()
  {
    Transform transform = this.transform;
    basePosition = transform.localPosition;
    baseRotation = transform.localEulerAngles;
    Animate();
  }

  private void Update() => Animate();
}
