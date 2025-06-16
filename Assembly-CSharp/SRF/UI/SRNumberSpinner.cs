using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI;

[AddComponentMenu("SRF/UI/SRNumberSpinner")]
public class SRNumberSpinner : InputField {
	private double _currentValue;
	private double _dragStartAmount;
	private double _dragStep;
	public float DragSensitivity = 0.01f;
	public double MaxValue = double.MaxValue;
	public double MinValue = double.MinValue;

	protected override void Awake() {
		base.Awake();
		if (contentType == ContentType.IntegerNumber || contentType == ContentType.DecimalNumber)
			return;
		Debug.LogError("[SRNumberSpinner] contentType must be integer or decimal. Defaulting to integer");
		contentType = ContentType.DecimalNumber;
	}

	public override void OnPointerClick(PointerEventData eventData) {
		if (!interactable || eventData.dragging)
			return;
		EventSystem.current.SetSelectedGameObject(gameObject, eventData);
		base.OnPointerClick(eventData);
		if (m_Keyboard == null || !m_Keyboard.active)
			OnSelect(eventData);
		else {
			UpdateLabel();
			eventData.Use();
		}
	}

	public override void OnPointerDown(PointerEventData eventData) { }

	public override void OnPointerUp(PointerEventData eventData) { }

	public override void OnBeginDrag(PointerEventData eventData) {
		if (!interactable)
			return;
		if (Mathf.Abs(eventData.delta.y) > (double)Mathf.Abs(eventData.delta.x)) {
			var parent = transform.parent;
			if (!(parent != null))
				return;
			eventData.pointerDrag = ExecuteEvents.GetEventHandler<IBeginDragHandler>(parent.gameObject);
			if (eventData.pointerDrag != null)
				ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
		} else {
			eventData.Use();
			_dragStartAmount = double.Parse(text);
			_currentValue = _dragStartAmount;
			var val1 = 1f;
			if (contentType == ContentType.IntegerNumber)
				val1 *= 10f;
			_dragStep = Math.Max(val1, _dragStartAmount * 0.05000000074505806);
			if (!isFocused)
				return;
			DeactivateInputField();
		}
	}

	public override void OnDrag(PointerEventData eventData) {
		if (!interactable)
			return;
		var x = eventData.delta.x;
		_currentValue += Math.Abs(_dragStep) * x * DragSensitivity;
		_currentValue = Math.Round(_currentValue, 2);
		if (_currentValue > MaxValue)
			_currentValue = MaxValue;
		if (_currentValue < MinValue)
			_currentValue = MinValue;
		if (contentType == ContentType.IntegerNumber)
			text = ((int)Math.Round(_currentValue)).ToString();
		else
			text = _currentValue.ToString();
	}

	public override void OnEndDrag(PointerEventData eventData) {
		if (!interactable)
			return;
		eventData.Use();
		if (_dragStartAmount == _currentValue)
			return;
		DeactivateInputField();
		SendOnSubmit();
	}
}