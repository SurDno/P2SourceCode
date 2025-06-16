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
      component.content = Content;
      component.viewport = Viewport;
      if (!((Object) ReplaceMask != (Object) null))
        return;
      GameObject gameObject = ReplaceMask.gameObject;
      Object.Destroy((Object) gameObject.GetComponent<Graphic>());
      Object.Destroy((Object) gameObject.GetComponent<CanvasRenderer>());
      Object.Destroy((Object) ReplaceMask);
      gameObject.AddComponent<RectMask2D>();
    }
  }
}
