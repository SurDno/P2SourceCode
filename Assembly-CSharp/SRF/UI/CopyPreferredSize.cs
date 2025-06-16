using UnityEngine;
using UnityEngine.UI;

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
        return (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredWidth(this.CopySource) + this.PaddingWidth;
      }
    }

    public override float preferredHeight
    {
      get
      {
        return (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredHeight(this.CopySource) + this.PaddingHeight;
      }
    }

    public override int layoutPriority => 2;
  }
}
