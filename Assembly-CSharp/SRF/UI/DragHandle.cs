using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI;

public class DragHandle :
	MonoBehaviour,
	IBeginDragHandler,
	IEventSystemHandler,
	IEndDragHandler,
	IDragHandler {
	private CanvasScaler _canvasScaler;
	private float _delta;
	private float _startValue;
	public RectTransform.Axis Axis = RectTransform.Axis.Horizontal;
	public bool Invert;
	public float MaxSize = -1f;
	public LayoutElement TargetLayoutElement;
	public RectTransform TargetRectTransform;

	private float Mult => Invert ? -1f : 1f;

	public void OnBeginDrag(PointerEventData eventData) {
		if (!Verify())
			return;
		_startValue = GetCurrentValue();
		_delta = 0.0f;
	}

	public void OnDrag(PointerEventData eventData) {
		if (!Verify())
			return;
		var num1 = 0.0f;
		var num2 = Axis != RectTransform.Axis.Horizontal ? num1 + eventData.delta.y : num1 + eventData.delta.x;
		if (_canvasScaler != null)
			num2 /= _canvasScaler.scaleFactor;
		_delta += num2 * Mult;
		SetCurrentValue(Mathf.Clamp(_startValue + _delta, GetMinSize(), GetMaxSize()));
	}

	public void OnEndDrag(PointerEventData eventData) {
		if (!Verify())
			return;
		SetCurrentValue(Mathf.Max(_startValue + _delta, GetMinSize()));
		_delta = 0.0f;
		CommitCurrentValue();
	}

	private void Start() {
		Verify();
		_canvasScaler = GetComponentInParent<CanvasScaler>();
	}

	private bool Verify() {
		if (!(TargetLayoutElement == null) || !(TargetRectTransform == null))
			return true;
		Debug.LogWarning("DragHandle: TargetLayoutElement and TargetRectTransform are both null. Disabling behaviour.");
		enabled = false;
		return false;
	}

	private float GetCurrentValue() {
		if (TargetLayoutElement != null)
			return Axis == RectTransform.Axis.Horizontal
				? TargetLayoutElement.preferredWidth
				: TargetLayoutElement.preferredHeight;
		if (TargetRectTransform != null)
			return Axis == RectTransform.Axis.Horizontal
				? TargetRectTransform.sizeDelta.x
				: TargetRectTransform.sizeDelta.y;
		throw new InvalidOperationException();
	}

	private void SetCurrentValue(float value) {
		if (TargetLayoutElement != null) {
			if (Axis == RectTransform.Axis.Horizontal)
				TargetLayoutElement.preferredWidth = value;
			else
				TargetLayoutElement.preferredHeight = value;
		} else {
			var vector2 = TargetRectTransform != null
				? TargetRectTransform.sizeDelta
				: throw new InvalidOperationException();
			if (Axis == RectTransform.Axis.Horizontal)
				vector2.x = value;
			else
				vector2.y = value;
			TargetRectTransform.sizeDelta = vector2;
		}
	}

	private void CommitCurrentValue() {
		if (!(TargetLayoutElement != null))
			return;
		if (Axis == RectTransform.Axis.Horizontal)
			TargetLayoutElement.preferredWidth = ((RectTransform)TargetLayoutElement.transform).sizeDelta.x;
		else
			TargetLayoutElement.preferredHeight = ((RectTransform)TargetLayoutElement.transform).sizeDelta.y;
	}

	private float GetMinSize() {
		return TargetLayoutElement == null ? 0.0f :
			Axis == RectTransform.Axis.Horizontal ? TargetLayoutElement.minWidth : TargetLayoutElement.minHeight;
	}

	private float GetMaxSize() {
		return MaxSize > 0.0 ? MaxSize : float.MaxValue;
	}
}