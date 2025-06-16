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
    postProcessingBehaviour = this.GetComponent<PostProcessingBehaviour>();
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
    if ((double) Vector3.SqrMagnitude(position - this.position) > num)
      Cut();
    else if ((double) Quaternion.Angle(rotation, this.rotation) > maxContinuousRotation)
      Cut();
    this.position = position;
    this.rotation = rotation;
  }
}
