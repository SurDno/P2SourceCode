using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

public class CreditsScrolling : MonoBehaviour
{
  [SerializeField]
  private CreditsGenerator content;
  [SerializeField]
  private float speed = 1f;
  [SerializeField]
  private float edgePositions = 50f;
  private RectTransform canvas;

  private void Start()
  {
    this.canvas = (RectTransform) this.GetComponentInParent<Canvas>().transform;
    this.content.Position = -this.edgePositions;
  }

  private void Update()
  {
    float position = this.content.Position;
    float b = Mathf.Max(this.content.Size + this.canvas.sizeDelta.y + this.edgePositions, position);
    float num = Mathf.Min(position + Time.deltaTime * this.speed, b);
    this.content.Position = num;
    if ((double) num != (double) b)
      return;
    ServiceLocator.GetService<UIService>()?.Pop();
  }
}
