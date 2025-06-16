using UnityEngine;

public class LockPickingPinSpot : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField]
  private Vector2 rotationRange = new Vector2(0.0f, 45f);
  [SerializeField]
  private float sizeMapping = 10f;
  [SerializeField]
  private float border;
  [SerializeField]
  private RectTransform image;
  [Header("State")]
  [SerializeField]
  private float size = 0.1f;
  [SerializeField]
  private float position = 0.5f;

  private void OnValidate() => ApplyState();

  private void ApplyState()
  {
    float min = size * 0.5f;
    position = Mathf.Clamp(position, min, 1f - min);
    RectTransform transform = (RectTransform) this.transform;
    float z = Mathf.Lerp(rotationRange.x, rotationRange.y, position);
    transform.localEulerAngles = new Vector3(0.0f, 0.0f, z);
    if (!(image != null))
      return;
    image.sizeDelta = image.sizeDelta with
    {
      y = size * sizeMapping + border
    };
    image.localEulerAngles = new Vector3(0.0f, 0.0f, -z);
  }

  public void Setup(float position, float size)
  {
    this.position = position;
    this.size = size;
    ApplyState();
  }
}
