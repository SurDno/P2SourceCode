﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Long Press Button")]
  public class LongPressButton : Button
  {
    private bool _handled;
    [SerializeField]
    private ButtonClickedEvent _onLongPress = new();
    private bool _pressed;
    private float _pressedTime;
    public float LongPressDuration = 0.9f;

    public ButtonClickedEvent onLongPress
    {
      get => _onLongPress;
      set => _onLongPress = value;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
      base.OnPointerExit(eventData);
      _pressed = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      if (eventData.button != 0)
        return;
      _pressed = true;
      _handled = false;
      _pressedTime = Time.realtimeSinceStartup;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      if (!_handled)
        base.OnPointerUp(eventData);
      _pressed = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
      if (_handled)
        return;
      base.OnPointerClick(eventData);
    }

    private void Update()
    {
      if (!_pressed || Time.realtimeSinceStartup - (double) _pressedTime < LongPressDuration)
        return;
      _pressed = false;
      _handled = true;
      onLongPress.Invoke();
    }
  }
}
