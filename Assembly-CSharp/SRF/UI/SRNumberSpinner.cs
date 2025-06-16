using System;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/SRNumberSpinner")]
  public class SRNumberSpinner : InputField
  {
    private double _currentValue;
    private double _dragStartAmount;
    private double _dragStep;
    public float DragSensitivity = 0.01f;
    public double MaxValue = double.MaxValue;
    public double MinValue = double.MinValue;

    protected override void Awake()
    {
      base.Awake();
      if (this.contentType == InputField.ContentType.IntegerNumber || this.contentType == InputField.ContentType.DecimalNumber)
        return;
      Debug.LogError((object) "[SRNumberSpinner] contentType must be integer or decimal. Defaulting to integer");
      this.contentType = InputField.ContentType.DecimalNumber;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (!this.interactable || eventData.dragging)
        return;
      EventSystem.current.SetSelectedGameObject(this.gameObject, (BaseEventData) eventData);
      base.OnPointerClick(eventData);
      if (this.m_Keyboard == null || !this.m_Keyboard.active)
      {
        this.OnSelect((BaseEventData) eventData);
      }
      else
      {
        this.UpdateLabel();
        eventData.Use();
      }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
      if (!this.interactable)
        return;
      if ((double) Mathf.Abs(eventData.delta.y) > (double) Mathf.Abs(eventData.delta.x))
      {
        Transform parent = this.transform.parent;
        if (!((UnityEngine.Object) parent != (UnityEngine.Object) null))
          return;
        eventData.pointerDrag = ExecuteEvents.GetEventHandler<IBeginDragHandler>(parent.gameObject);
        if ((UnityEngine.Object) eventData.pointerDrag != (UnityEngine.Object) null)
          ExecuteEvents.Execute<IBeginDragHandler>(eventData.pointerDrag, (BaseEventData) eventData, ExecuteEvents.beginDragHandler);
      }
      else
      {
        eventData.Use();
        _dragStartAmount = double.Parse(this.text);
        _currentValue = _dragStartAmount;
        float val1 = 1f;
        if (this.contentType == InputField.ContentType.IntegerNumber)
          val1 *= 10f;
        _dragStep = Math.Max(val1, _dragStartAmount * 0.05000000074505806);
        if (!this.isFocused)
          return;
        this.DeactivateInputField();
      }
    }

    public override void OnDrag(PointerEventData eventData)
    {
      if (!this.interactable)
        return;
      float x = eventData.delta.x;
      _currentValue += Math.Abs(_dragStep) * x * DragSensitivity;
      _currentValue = Math.Round(_currentValue, 2);
      if (_currentValue > MaxValue)
        _currentValue = MaxValue;
      if (_currentValue < MinValue)
        _currentValue = MinValue;
      if (this.contentType == InputField.ContentType.IntegerNumber)
        this.text = ((int) Math.Round(_currentValue)).ToString();
      else
        this.text = _currentValue.ToString();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
      if (!this.interactable)
        return;
      eventData.Use();
      if (_dragStartAmount == _currentValue)
        return;
      this.DeactivateInputField();
      this.SendOnSubmit();
    }
  }
}
