using System;

namespace SRF.UI
{
  public class DragHandle : 
    MonoBehaviour,
    IBeginDragHandler,
    IEventSystemHandler,
    IEndDragHandler,
    IDragHandler
  {
    private CanvasScaler _canvasScaler;
    private float _delta;
    private float _startValue;
    public RectTransform.Axis Axis = RectTransform.Axis.Horizontal;
    public bool Invert = false;
    public float MaxSize = -1f;
    public LayoutElement TargetLayoutElement;
    public RectTransform TargetRectTransform;

    private float Mult => Invert ? -1f : 1f;

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!Verify())
        return;
      _startValue = GetCurrentValue();
      _delta = 0.0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (!Verify())
        return;
      float num1 = 0.0f;
      float num2 = Axis != RectTransform.Axis.Horizontal ? num1 + eventData.delta.y : num1 + eventData.delta.x;
      if ((UnityEngine.Object) _canvasScaler != (UnityEngine.Object) null)
        num2 /= _canvasScaler.scaleFactor;
      _delta += num2 * Mult;
      SetCurrentValue(Mathf.Clamp(_startValue + _delta, GetMinSize(), GetMaxSize()));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (!Verify())
        return;
      SetCurrentValue(Mathf.Max(_startValue + _delta, GetMinSize()));
      _delta = 0.0f;
      CommitCurrentValue();
    }

    private void Start()
    {
      Verify();
      _canvasScaler = this.GetComponentInParent<CanvasScaler>();
    }

    private bool Verify()
    {
      if (!((UnityEngine.Object) TargetLayoutElement == (UnityEngine.Object) null) || !((UnityEngine.Object) TargetRectTransform == (UnityEngine.Object) null))
        return true;
      Debug.LogWarning((object) "DragHandle: TargetLayoutElement and TargetRectTransform are both null. Disabling behaviour.");
      this.enabled = false;
      return false;
    }

    private float GetCurrentValue()
    {
      if ((UnityEngine.Object) TargetLayoutElement != (UnityEngine.Object) null)
        return Axis == RectTransform.Axis.Horizontal ? TargetLayoutElement.preferredWidth : TargetLayoutElement.preferredHeight;
      if ((UnityEngine.Object) TargetRectTransform != (UnityEngine.Object) null)
        return Axis == RectTransform.Axis.Horizontal ? TargetRectTransform.sizeDelta.x : TargetRectTransform.sizeDelta.y;
      throw new InvalidOperationException();
    }

    private void SetCurrentValue(float value)
    {
      if ((UnityEngine.Object) TargetLayoutElement != (UnityEngine.Object) null)
      {
        if (Axis == RectTransform.Axis.Horizontal)
          TargetLayoutElement.preferredWidth = value;
        else
          TargetLayoutElement.preferredHeight = value;
      }
      else
      {
        Vector2 vector2 = (UnityEngine.Object) TargetRectTransform != (UnityEngine.Object) null ? TargetRectTransform.sizeDelta : throw new InvalidOperationException();
        if (Axis == RectTransform.Axis.Horizontal)
          vector2.x = value;
        else
          vector2.y = value;
        TargetRectTransform.sizeDelta = vector2;
      }
    }

    private void CommitCurrentValue()
    {
      if (!((UnityEngine.Object) TargetLayoutElement != (UnityEngine.Object) null))
        return;
      if (Axis == RectTransform.Axis.Horizontal)
        TargetLayoutElement.preferredWidth = ((RectTransform) TargetLayoutElement.transform).sizeDelta.x;
      else
        TargetLayoutElement.preferredHeight = ((RectTransform) TargetLayoutElement.transform).sizeDelta.y;
    }

    private float GetMinSize()
    {
      return (UnityEngine.Object) TargetLayoutElement == (UnityEngine.Object) null ? 0.0f : (Axis == RectTransform.Axis.Horizontal ? TargetLayoutElement.minWidth : TargetLayoutElement.minHeight);
    }

    private float GetMaxSize() => MaxSize > 0.0 ? MaxSize : float.MaxValue;
  }
}
