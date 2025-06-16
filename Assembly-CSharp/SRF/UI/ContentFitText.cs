// Decompiled with JetBrains decompiler
// Type: SRF.UI.ContentFitText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
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
        return (UnityEngine.Object) this.CopySource == (UnityEngine.Object) null ? -1f : LayoutUtility.GetMinWidth(this.CopySource.rectTransform) + this.Padding.x;
      }
    }

    public float preferredWidth
    {
      get
      {
        return (UnityEngine.Object) this.CopySource == (UnityEngine.Object) null ? -1f : LayoutUtility.GetPreferredWidth(this.CopySource.rectTransform) + this.Padding.x;
      }
    }

    public float flexibleWidth
    {
      get
      {
        return (UnityEngine.Object) this.CopySource == (UnityEngine.Object) null ? -1f : LayoutUtility.GetFlexibleWidth(this.CopySource.rectTransform);
      }
    }

    public float minHeight
    {
      get
      {
        return (UnityEngine.Object) this.CopySource == (UnityEngine.Object) null ? -1f : LayoutUtility.GetFlexibleHeight(this.CopySource.rectTransform) + this.Padding.y;
      }
    }

    public float preferredHeight
    {
      get
      {
        return (UnityEngine.Object) this.CopySource == (UnityEngine.Object) null ? -1f : LayoutUtility.GetPreferredHeight(this.CopySource.rectTransform) + this.Padding.y;
      }
    }

    public float flexibleHeight
    {
      get
      {
        return (UnityEngine.Object) this.CopySource == (UnityEngine.Object) null ? -1f : LayoutUtility.GetFlexibleHeight(this.CopySource.rectTransform);
      }
    }

    public int layoutPriority => 0;

    public void CalculateLayoutInputHorizontal()
    {
      this.CopySource.CalculateLayoutInputHorizontal();
    }

    public void CalculateLayoutInputVertical() => this.CopySource.CalculateLayoutInputVertical();

    protected override void OnEnable()
    {
      this.SetDirty();
      this.CopySource.LayoutDirty += new Action<SRText>(this.CopySourceOnLayoutDirty);
    }

    private void CopySourceOnLayoutDirty(SRText srText) => this.SetDirty();

    protected override void OnTransformParentChanged() => this.SetDirty();

    protected override void OnDisable()
    {
      this.CopySource.LayoutDirty -= new Action<SRText>(this.CopySourceOnLayoutDirty);
      this.SetDirty();
    }

    protected override void OnDidApplyAnimationProperties() => this.SetDirty();

    protected override void OnBeforeTransformParentChanged() => this.SetDirty();

    protected void SetDirty()
    {
      if (!this.IsActive())
        return;
      LayoutRebuilder.MarkLayoutForRebuild(this.transform as RectTransform);
    }
  }
}
