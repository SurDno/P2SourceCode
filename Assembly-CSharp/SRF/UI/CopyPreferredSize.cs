namespace SRF.UI
{
  [RequireComponent(typeof (RectTransform))]
  [ExecuteInEditMode]
  [AddComponentMenu("SRF/UI/Copy Preferred Size")]
  public class CopyPreferredSize : LayoutElement
  {
    public RectTransform CopySource;
    public float PaddingHeight;
    public float PaddingWidth;

    public override float preferredWidth
    {
      get
      {
        return (Object) CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredWidth(CopySource) + PaddingWidth;
      }
    }

    public override float preferredHeight
    {
      get
      {
        return (Object) CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredHeight(CopySource) + PaddingHeight;
      }
    }

    public override int layoutPriority => 2;
  }
}
