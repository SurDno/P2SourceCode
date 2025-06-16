namespace Engine.Impl.UI
{
  [RequireComponent(typeof (RectTransform))]
  public class UIControl : MonoBehaviour
  {
    private RectTransform transform;

    public RectTransform Transform
    {
      get
      {
        if ((Object) transform == (Object) null)
          transform = this.gameObject.GetComponent<RectTransform>();
        return transform;
      }
    }

    public Vector2 PivotedPosition
    {
      get
      {
        return (Vector2) Transform.position - Vector2.Scale(Transform.pivot, Vector2.Scale(Transform.sizeDelta, (Vector2) Transform.lossyScale));
      }
      set
      {
        Transform.position = (Vector3) (value + Vector2.Scale(Transform.pivot, Vector2.Scale(Transform.sizeDelta, (Vector2) Transform.lossyScale)));
      }
    }

    public bool IsEnabled
    {
      get => this.gameObject.activeSelf;
      set => this.gameObject.SetActive(value);
    }

    protected virtual void Awake()
    {
    }
  }
}
