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

  private void Cut() => postProcessingBehaviour.ResetTemporalEffects();

  private void Start()
  {
    postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
    Transform transform = this.transform;
    position = transform.position;
    rotation = transform.rotation;
  }

  private void OnPreCull()
  {
    Transform transform = this.transform;
    Vector3 position = transform.position;
    Quaternion rotation = transform.rotation;
    float num = maxContinuousTranslation * maxContinuousTranslation;
    if (Vector3.SqrMagnitude(position - this.position) > (double) num)
      Cut();
    else if (Quaternion.Angle(rotation, this.rotation) > (double) maxContinuousRotation)
      Cut();
    this.position = position;
    this.rotation = rotation;
  }
}
