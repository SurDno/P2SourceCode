using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

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
    transform.localPosition = this.basePosition + this.Animate(flag ? this.positionAmplitude * 2.5f : this.positionAmplitude, this.positionRate);
    transform.localEulerAngles = this.baseRotation + this.Animate(flag ? this.rotationAmplitude * 2.5f : this.rotationAmplitude, this.rotationRate);
  }

  private float Animate(float amplitude, float rate)
  {
    return Mathf.Sin((float) ((double) Time.time * (double) rate * 2.0 * 3.1415927410125732)) * amplitude;
  }

  private Vector3 Animate(Vector3 amplitude, Vector3 rate)
  {
    return new Vector3(this.Animate(amplitude.x, rate.x), this.Animate(amplitude.y, rate.y), this.Animate(amplitude.z, rate.z));
  }

  private void OnDisable()
  {
    Transform transform = this.transform;
    transform.localPosition = this.basePosition;
    transform.localEulerAngles = this.baseRotation;
  }

  private void OnEnable()
  {
    Transform transform = this.transform;
    this.basePosition = transform.localPosition;
    this.baseRotation = transform.localEulerAngles;
    this.Animate();
  }

  private void Update() => this.Animate();
}
