using UnityEngine;

public class LockPickingPinSpot : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField]
  private Vector2 rotationRange = new Vector2(0.0f, 45f);
  [SerializeField]
  private float sizeMapping = 10f;
  [SerializeField]
  private float border = 0.0f;
  [SerializeField]
  private RectTransform image = (RectTransform) null;
  [Header("State")]
  [SerializeField]
  private float size = 0.1f;
  [SerializeField]
  private float position = 0.5f;

  private void OnValidate() => this.ApplyState();

  private void ApplyState()
  {
    float min = this.size * 0.5f;
    this.position = Mathf.Clamp(this.position, min, 1f - min);
    RectTransform transform = (RectTransform) this.transform;
    float z = Mathf.Lerp(this.rotationRange.x, this.rotationRange.y, this.position);
    transform.localEulerAngles = new Vector3(0.0f, 0.0f, z);
    if (!((Object) this.image != (Object) null))
      return;
    this.image.sizeDelta = this.image.sizeDelta with
    {
      y = this.size * this.sizeMapping + this.border
    };
    this.image.localEulerAngles = new Vector3(0.0f, 0.0f, -z);
  }

  public void Setup(float position, float size)
  {
    this.position = position;
    this.size = size;
    this.ApplyState();
  }
}
