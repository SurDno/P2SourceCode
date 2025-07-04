﻿using UnityEngine;
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

    public float preferredWidth => !CopyPreferredWidth || CopySource == null || !IsActive() ? -1f : LayoutUtility.GetPreferredWidth(CopySource) + PaddingPreferredWidth;

    public float preferredHeight => !CopyPreferredHeight || CopySource == null || !IsActive() ? -1f : LayoutUtility.GetPreferredHeight(CopySource) + PaddingPreferredHeight;

    public float minWidth => !CopyMinWidth || CopySource == null || !IsActive() ? -1f : LayoutUtility.GetMinWidth(CopySource) + PaddingMinWidth;

    public float minHeight => !CopyMinHeight || CopySource == null || !IsActive() ? -1f : LayoutUtility.GetMinHeight(CopySource) + PaddingMinHeight;

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
