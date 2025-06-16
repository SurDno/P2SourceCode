using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Other
{
  [RequireComponent(typeof (ScrollRect))]
  [ExecuteInEditMode]
  public class ScrollRectPatch : MonoBehaviour
  {
    public RectTransform Content;
    public Mask ReplaceMask;
    public RectTransform Viewport;

    private void Awake()
    {
      ScrollRect component = this.GetComponent<ScrollRect>();
      component.content = this.Content;
      component.viewport = this.Viewport;
      if (!((Object) this.ReplaceMask != (Object) null))
        return;
      GameObject gameObject = this.ReplaceMask.gameObject;
      Object.Destroy((Object) gameObject.GetComponent<Graphic>());
      Object.Destroy((Object) gameObject.GetComponent<CanvasRenderer>());
      Object.Destroy((Object) this.ReplaceMask);
      gameObject.AddComponent<RectMask2D>();
    }
  }
}
