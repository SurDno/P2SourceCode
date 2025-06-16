using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI
{
  [RequireComponent(typeof (RectTransform))]
  [ExecuteInEditMode]
  [AddComponentMenu("SRF/UI/Content Fit Text")]
  public class ContentFitText : UIBehaviour, ILayoutElement
  {
    public SRText CopySource;
    public Vector2 Padding;

    public float minWidth
    {
      get
      {
        return CopySource == null ? -1f : LayoutUtility.GetMinWidth(CopySource.rectTransform) + Padding.x;
      }
    }

    public float preferredWidth
    {
      get
      {
        return CopySource == null ? -1f : LayoutUtility.GetPreferredWidth(CopySource.rectTransform) + Padding.x;
      }
    }

    public float flexibleWidth
    {
      get
      {
        return CopySource == null ? -1f : LayoutUtility.GetFlexibleWidth(CopySource.rectTransform);
      }
    }

    public float minHeight
    {
      get
      {
        return CopySource == null ? -1f : LayoutUtility.GetFlexibleHeight(CopySource.rectTransform) + Padding.y;
      }
    }

    public float preferredHeight
    {
      get
      {
        return CopySource == null ? -1f : LayoutUtility.GetPreferredHeight(CopySource.rectTransform) + Padding.y;
      }
    }

    public float flexibleHeight
    {
      get
      {
        return CopySource == null ? -1f : LayoutUtility.GetFlexibleHeight(CopySource.rectTransform);
      }
    }

    public int layoutPriority => 0;

    public void CalculateLayoutInputHorizontal()
    {
      CopySource.CalculateLayoutInputHorizontal();
    }

    public void CalculateLayoutInputVertical() => CopySource.CalculateLayoutInputVertical();

    protected override void OnEnable()
    {
      SetDirty();
      CopySource.LayoutDirty += CopySourceOnLayoutDirty;
    }

    private void CopySourceOnLayoutDirty(SRText srText) => SetDirty();

    protected override void OnTransformParentChanged() => SetDirty();

    protected override void OnDisable()
    {
      CopySource.LayoutDirty -= CopySourceOnLayoutDirty;
      SetDirty();
    }

    protected override void OnDidApplyAnimationProperties() => SetDirty();

    protected override void OnBeforeTransformParentChanged() => SetDirty();

    protected void SetDirty()
    {
      if (!IsActive())
        return;
      LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
    }
  }
}
