using Engine.Common.Services;
using Engine.Impl.Services;

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
    canvas = (RectTransform) this.GetComponentInParent<Canvas>().transform;
    content.Position = -edgePositions;
  }

  private void Update()
  {
    float position = content.Position;
    float b = Mathf.Max(content.Size + canvas.sizeDelta.y + edgePositions, position);
    float num = Mathf.Min(position + Time.deltaTime * speed, b);
    content.Position = num;
    if (num != (double) b)
      return;
    ServiceLocator.GetService<UIService>()?.Pop();
  }
}
