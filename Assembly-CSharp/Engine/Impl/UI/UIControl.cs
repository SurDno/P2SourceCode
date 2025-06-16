using UnityEngine;

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
        if ((Object) this.transform == (Object) null)
          this.transform = this.gameObject.GetComponent<RectTransform>();
        return this.transform;
      }
    }

    public Vector2 PivotedPosition
    {
      get
      {
        return (Vector2) this.Transform.position - Vector2.Scale(this.Transform.pivot, Vector2.Scale(this.Transform.sizeDelta, (Vector2) this.Transform.lossyScale));
      }
      set
      {
        this.Transform.position = (Vector3) (value + Vector2.Scale(this.Transform.pivot, Vector2.Scale(this.Transform.sizeDelta, (Vector2) this.Transform.lossyScale)));
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
