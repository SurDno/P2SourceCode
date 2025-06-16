using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SRF.UI.Layout;

[AddComponentMenu("SRF/UI/Layout/Flow Layout Group")]
public class FlowLayoutGroup : LayoutGroup {
	private readonly IList<RectTransform> _rowList = new List<RectTransform>();
	private float _layoutHeight;
	public bool ChildForceExpandHeight;
	public bool ChildForceExpandWidth;
	public float Spacing;

	protected bool IsCenterAlign => childAlignment == TextAnchor.LowerCenter ||
	                                childAlignment == TextAnchor.MiddleCenter ||
	                                childAlignment == TextAnchor.UpperCenter;

	protected bool IsRightAlign => childAlignment == TextAnchor.LowerRight ||
	                               childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.UpperRight;

	protected bool IsMiddleAlign => childAlignment == TextAnchor.MiddleLeft ||
	                                childAlignment == TextAnchor.MiddleRight ||
	                                childAlignment == TextAnchor.MiddleCenter;

	protected bool IsLowerAlign => childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerRight ||
	                               childAlignment == TextAnchor.LowerCenter;

	public override void CalculateLayoutInputHorizontal() {
		base.CalculateLayoutInputHorizontal();
		SetLayoutInputForAxis(GetGreatestMinimumChildWidth() + padding.left + padding.right, -1f, -1f, 0);
	}

	public override void SetLayoutHorizontal() {
		double num = SetLayout(rectTransform.rect.width, 0, false);
	}

	public override void SetLayoutVertical() {
		double num = SetLayout(rectTransform.rect.width, 1, false);
	}

	public override void CalculateLayoutInputVertical() {
		_layoutHeight = SetLayout(rectTransform.rect.width, 1, true);
	}

	public float SetLayout(float width, int axis, bool layoutInput) {
		var height = rectTransform.rect.height;
		var num1 = rectTransform.rect.width - padding.left - padding.right;
		var yOffset = IsLowerAlign ? padding.bottom : (float)padding.top;
		var rowWidth1 = 0.0f;
		var num2 = 0.0f;
		for (var index = 0; index < rectChildren.Count; ++index) {
			var rectChild = rectChildren[IsLowerAlign ? rectChildren.Count - 1 - index : index];
			var preferredSize1 = LayoutUtility.GetPreferredSize(rectChild, 0);
			var preferredSize2 = LayoutUtility.GetPreferredSize(rectChild, 1);
			var num3 = Mathf.Min(preferredSize1, num1);
			if (_rowList.Count > 0)
				rowWidth1 += Spacing;
			if (rowWidth1 + (double)num3 > num1) {
				var rowWidth2 = rowWidth1 - Spacing;
				if (!layoutInput) {
					var rowVerticalOffset = CalculateRowVerticalOffset(height, yOffset, num2);
					LayoutRow(_rowList, rowWidth2, num2, num1, padding.left, rowVerticalOffset, axis);
				}

				_rowList.Clear();
				yOffset = yOffset + num2 + Spacing;
				num2 = 0.0f;
				rowWidth1 = 0.0f;
			}

			rowWidth1 += num3;
			_rowList.Add(rectChild);
			if (preferredSize2 > (double)num2)
				num2 = preferredSize2;
		}

		if (!layoutInput) {
			var rowVerticalOffset = CalculateRowVerticalOffset(height, yOffset, num2);
			LayoutRow(_rowList, rowWidth1, num2, num1, padding.left, rowVerticalOffset, axis);
		}

		_rowList.Clear();
		var num4 = yOffset + num2 + (IsLowerAlign ? padding.top : (float)padding.bottom);
		if (layoutInput && axis == 1)
			SetLayoutInputForAxis(num4, num4, -1f, axis);
		return num4;
	}

	private float CalculateRowVerticalOffset(
		float groupHeight,
		float yOffset,
		float currentRowHeight) {
		return !IsLowerAlign
			? !IsMiddleAlign ? yOffset : (float)(groupHeight * 0.5 - _layoutHeight * 0.5) + yOffset
			: groupHeight - yOffset - currentRowHeight;
	}

	protected void LayoutRow(
		IList<RectTransform> contents,
		float rowWidth,
		float rowHeight,
		float maxWidth,
		float xOffset,
		float yOffset,
		int axis) {
		var pos1 = xOffset;
		if (!ChildForceExpandWidth && IsCenterAlign)
			pos1 += (float)((maxWidth - (double)rowWidth) * 0.5);
		else if (!ChildForceExpandWidth && IsRightAlign)
			pos1 += maxWidth - rowWidth;
		var num1 = 0.0f;
		if (ChildForceExpandWidth) {
			var num2 = 0;
			for (var index = 0; index < _rowList.Count; ++index)
				if (LayoutUtility.GetFlexibleWidth(_rowList[index]) > 0.0)
					++num2;
			if (num2 > 0)
				num1 = (maxWidth - rowWidth) / num2;
		}

		for (var index = 0; index < _rowList.Count; ++index) {
			var row = _rowList[IsLowerAlign ? _rowList.Count - 1 - index : index];
			var preferredSize = LayoutUtility.GetPreferredSize(row, 0);
			if (LayoutUtility.GetFlexibleWidth(row) > 0.0)
				preferredSize += num1;
			var size1 = LayoutUtility.GetPreferredSize(row, 1);
			if (ChildForceExpandHeight)
				size1 = rowHeight;
			var size2 = Mathf.Min(preferredSize, maxWidth);
			var pos2 = yOffset;
			if (IsMiddleAlign)
				pos2 += (float)((rowHeight - (double)size1) * 0.5);
			else if (IsLowerAlign)
				pos2 += rowHeight - size1;
			if (axis == 0)
				SetChildAlongAxis(row, 0, pos1, size2);
			else
				SetChildAlongAxis(row, 1, pos2, size1);
			pos1 += size2 + Spacing;
		}
	}

	public float GetGreatestMinimumChildWidth() {
		var b = 0.0f;
		for (var index = 0; index < rectChildren.Count; ++index)
			b = Mathf.Max(LayoutUtility.GetMinWidth(rectChildren[index]), b);
		return b;
	}
}