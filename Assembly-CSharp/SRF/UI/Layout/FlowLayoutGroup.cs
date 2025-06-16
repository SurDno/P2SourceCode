using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SRF.UI.Layout
{
  [AddComponentMenu("SRF/UI/Layout/Flow Layout Group")]
  public class FlowLayoutGroup : LayoutGroup
  {
    private readonly IList<RectTransform> _rowList = (IList<RectTransform>) new List<RectTransform>();
    private float _layoutHeight;
    public bool ChildForceExpandHeight = false;
    public bool ChildForceExpandWidth = false;
    public float Spacing = 0.0f;

    protected bool IsCenterAlign
    {
      get
      {
        return this.childAlignment == TextAnchor.LowerCenter || this.childAlignment == TextAnchor.MiddleCenter || this.childAlignment == TextAnchor.UpperCenter;
      }
    }

    protected bool IsRightAlign
    {
      get
      {
        return this.childAlignment == TextAnchor.LowerRight || this.childAlignment == TextAnchor.MiddleRight || this.childAlignment == TextAnchor.UpperRight;
      }
    }

    protected bool IsMiddleAlign
    {
      get
      {
        return this.childAlignment == TextAnchor.MiddleLeft || this.childAlignment == TextAnchor.MiddleRight || this.childAlignment == TextAnchor.MiddleCenter;
      }
    }

    protected bool IsLowerAlign
    {
      get
      {
        return this.childAlignment == TextAnchor.LowerLeft || this.childAlignment == TextAnchor.LowerRight || this.childAlignment == TextAnchor.LowerCenter;
      }
    }

    public override void CalculateLayoutInputHorizontal()
    {
      base.CalculateLayoutInputHorizontal();
      this.SetLayoutInputForAxis(this.GetGreatestMinimumChildWidth() + (float) this.padding.left + (float) this.padding.right, -1f, -1f, 0);
    }

    public override void SetLayoutHorizontal()
    {
      double num = (double) this.SetLayout(this.rectTransform.rect.width, 0, false);
    }

    public override void SetLayoutVertical()
    {
      double num = (double) this.SetLayout(this.rectTransform.rect.width, 1, false);
    }

    public override void CalculateLayoutInputVertical()
    {
      this._layoutHeight = this.SetLayout(this.rectTransform.rect.width, 1, true);
    }

    public float SetLayout(float width, int axis, bool layoutInput)
    {
      float height = this.rectTransform.rect.height;
      float num1 = this.rectTransform.rect.width - (float) this.padding.left - (float) this.padding.right;
      float yOffset = this.IsLowerAlign ? (float) this.padding.bottom : (float) this.padding.top;
      float rowWidth1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < this.rectChildren.Count; ++index)
      {
        RectTransform rectChild = this.rectChildren[this.IsLowerAlign ? this.rectChildren.Count - 1 - index : index];
        float preferredSize1 = LayoutUtility.GetPreferredSize(rectChild, 0);
        float preferredSize2 = LayoutUtility.GetPreferredSize(rectChild, 1);
        float num3 = Mathf.Min(preferredSize1, num1);
        if (this._rowList.Count > 0)
          rowWidth1 += this.Spacing;
        if ((double) rowWidth1 + (double) num3 > (double) num1)
        {
          float rowWidth2 = rowWidth1 - this.Spacing;
          if (!layoutInput)
          {
            float rowVerticalOffset = this.CalculateRowVerticalOffset(height, yOffset, num2);
            this.LayoutRow(this._rowList, rowWidth2, num2, num1, (float) this.padding.left, rowVerticalOffset, axis);
          }
          this._rowList.Clear();
          yOffset = yOffset + num2 + this.Spacing;
          num2 = 0.0f;
          rowWidth1 = 0.0f;
        }
        rowWidth1 += num3;
        this._rowList.Add(rectChild);
        if ((double) preferredSize2 > (double) num2)
          num2 = preferredSize2;
      }
      if (!layoutInput)
      {
        float rowVerticalOffset = this.CalculateRowVerticalOffset(height, yOffset, num2);
        this.LayoutRow(this._rowList, rowWidth1, num2, num1, (float) this.padding.left, rowVerticalOffset, axis);
      }
      this._rowList.Clear();
      float num4 = yOffset + num2 + (this.IsLowerAlign ? (float) this.padding.top : (float) this.padding.bottom);
      if (layoutInput && axis == 1)
        this.SetLayoutInputForAxis(num4, num4, -1f, axis);
      return num4;
    }

    private float CalculateRowVerticalOffset(
      float groupHeight,
      float yOffset,
      float currentRowHeight)
    {
      return !this.IsLowerAlign ? (!this.IsMiddleAlign ? yOffset : (float) ((double) groupHeight * 0.5 - (double) this._layoutHeight * 0.5) + yOffset) : groupHeight - yOffset - currentRowHeight;
    }

    protected void LayoutRow(
      IList<RectTransform> contents,
      float rowWidth,
      float rowHeight,
      float maxWidth,
      float xOffset,
      float yOffset,
      int axis)
    {
      float pos1 = xOffset;
      if (!this.ChildForceExpandWidth && this.IsCenterAlign)
        pos1 += (float) (((double) maxWidth - (double) rowWidth) * 0.5);
      else if (!this.ChildForceExpandWidth && this.IsRightAlign)
        pos1 += maxWidth - rowWidth;
      float num1 = 0.0f;
      if (this.ChildForceExpandWidth)
      {
        int num2 = 0;
        for (int index = 0; index < this._rowList.Count; ++index)
        {
          if ((double) LayoutUtility.GetFlexibleWidth(this._rowList[index]) > 0.0)
            ++num2;
        }
        if (num2 > 0)
          num1 = (maxWidth - rowWidth) / (float) num2;
      }
      for (int index = 0; index < this._rowList.Count; ++index)
      {
        RectTransform row = this._rowList[this.IsLowerAlign ? this._rowList.Count - 1 - index : index];
        float preferredSize = LayoutUtility.GetPreferredSize(row, 0);
        if ((double) LayoutUtility.GetFlexibleWidth(row) > 0.0)
          preferredSize += num1;
        float size1 = LayoutUtility.GetPreferredSize(row, 1);
        if (this.ChildForceExpandHeight)
          size1 = rowHeight;
        float size2 = Mathf.Min(preferredSize, maxWidth);
        float pos2 = yOffset;
        if (this.IsMiddleAlign)
          pos2 += (float) (((double) rowHeight - (double) size1) * 0.5);
        else if (this.IsLowerAlign)
          pos2 += rowHeight - size1;
        if (axis == 0)
          this.SetChildAlongAxis(row, 0, pos1, size2);
        else
          this.SetChildAlongAxis(row, 1, pos2, size1);
        pos1 += size2 + this.Spacing;
      }
    }

    public float GetGreatestMinimumChildWidth()
    {
      float b = 0.0f;
      for (int index = 0; index < this.rectChildren.Count; ++index)
        b = Mathf.Max(LayoutUtility.GetMinWidth(this.rectChildren[index]), b);
      return b;
    }
  }
}
