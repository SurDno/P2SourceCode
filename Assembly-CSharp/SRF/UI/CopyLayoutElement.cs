using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI
{
  [RequireComponent(typeof (RectTransform))]
  [ExecuteInEditMode]
  [AddComponentMenu("SRF/UI/Copy Layout Element")]
  public class CopyLayoutElement : UIBehaviour, ILayoutElement
  {
    public bool CopyMinHeight;
    public bool CopyMinWidth;
    public bool CopyPreferredHeight;
    public bool CopyPreferredWidth;
    public RectTransform CopySource;
    public float PaddingMinHeight;
    public float PaddingMinWidth;
    public float PaddingPreferredHeight;
    public float PaddingPreferredWidth;

    public float preferredWidth
    {
      get
      {
        return !this.CopyPreferredWidth || (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredWidth(this.CopySource) + this.PaddingPreferredWidth;
      }
    }

    public float preferredHeight
    {
      get
      {
        return !this.CopyPreferredHeight || (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetPreferredHeight(this.CopySource) + this.PaddingPreferredHeight;
      }
    }

    public float minWidth
    {
      get
      {
        return !this.CopyMinWidth || (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetMinWidth(this.CopySource) + this.PaddingMinWidth;
      }
    }

    public float minHeight
    {
      get
      {
        return !this.CopyMinHeight || (Object) this.CopySource == (Object) null || !this.IsActive() ? -1f : LayoutUtility.GetMinHeight(this.CopySource) + this.PaddingMinHeight;
      }
    }

    public int layoutPriority => 2;

    public float flexibleHeight => -1f;

    public float flexibleWidth => -1f;

    public void CalculateLayoutInputHorizontal()
    {
    }

    public void CalculateLayoutInputVertical()
    {
    }
  }
}
