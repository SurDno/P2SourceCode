using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraCutDetector : MonoBehaviour
{
  [SerializeField]
  private float maxContinuousTranslation = 1f;
  [SerializeField]
  private float maxContinuousRotation = 45f;
  private PostProcessingBehaviour postProcessingBehaviour;
  private Vector3 position;
  private Quaternion rotation;

  private void Cut() => this.postProcessingBehaviour.ResetTemporalEffects();

  private void Start()
  {
    this.postProcessingBehaviour = this.GetComponent<PostProcessingBehaviour>();
    Transform transform = this.transform;
    this.position = transform.position;
    this.rotation = transform.rotation;
  }

  private void OnPreCull()
  {
    Transform transform = this.transform;
    Vector3 position = transform.position;
    Quaternion rotation = transform.rotation;
    float num = this.maxContinuousTranslation * this.maxContinuousTranslation;
    if ((double) Vector3.SqrMagnitude(position - this.position) > (double) num)
      this.Cut();
    else if ((double) Quaternion.Angle(rotation, this.rotation) > (double) this.maxContinuousRotation)
      this.Cut();
    this.position = position;
    this.rotation = rotation;
  }
}
