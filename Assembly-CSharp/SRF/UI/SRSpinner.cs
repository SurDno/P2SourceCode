using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Spinner")]
  public class SRSpinner : Selectable, IDragHandler, IEventSystemHandler, IBeginDragHandler
  {
    private float _dragDelta;
    [SerializeField]
    private SpinEvent _onSpinDecrement = new();
    [SerializeField]
    private SpinEvent _onSpinIncrement = new();
    public float DragThreshold = 20f;

    public SpinEvent OnSpinIncrement
    {
      get => _onSpinIncrement;
      set => _onSpinIncrement = value;
    }

    public SpinEvent OnSpinDecrement
    {
      get => _onSpinDecrement;
      set => _onSpinDecrement = value;
    }

    public void OnBeginDrag(PointerEventData eventData) => _dragDelta = 0.0f;

    public void OnDrag(PointerEventData eventData)
    {
      if (!interactable)
        return;
      _dragDelta += eventData.delta.x;
      if (Mathf.Abs(_dragDelta) <= (double) DragThreshold)
        return;
      float num = Mathf.Sign(_dragDelta);
      int amount = Mathf.FloorToInt(Mathf.Abs(_dragDelta) / DragThreshold);
      if (num > 0.0)
        OnIncrement(amount);
      else
        OnDecrement(amount);
      _dragDelta -= amount * DragThreshold * num;
    }

    private void OnIncrement(int amount)
    {
      for (int index = 0; index < amount; ++index)
        OnSpinIncrement.Invoke();
    }

    private void OnDecrement(int amount)
    {
      for (int index = 0; index < amount; ++index)
        OnSpinDecrement.Invoke();
    }

    [Serializable]
    public class SpinEvent : UnityEvent
    {
    }
  }
}
