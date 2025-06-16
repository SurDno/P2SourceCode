// Decompiled with JetBrains decompiler
// Type: SRF.UI.DragHandle
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

    private float Mult => this.Invert ? -1f : 1f;

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!this.Verify())
        return;
      this._startValue = this.GetCurrentValue();
      this._delta = 0.0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (!this.Verify())
        return;
      float num1 = 0.0f;
      float num2 = this.Axis != RectTransform.Axis.Horizontal ? num1 + eventData.delta.y : num1 + eventData.delta.x;
      if ((UnityEngine.Object) this._canvasScaler != (UnityEngine.Object) null)
        num2 /= this._canvasScaler.scaleFactor;
      this._delta += num2 * this.Mult;
      this.SetCurrentValue(Mathf.Clamp(this._startValue + this._delta, this.GetMinSize(), this.GetMaxSize()));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (!this.Verify())
        return;
      this.SetCurrentValue(Mathf.Max(this._startValue + this._delta, this.GetMinSize()));
      this._delta = 0.0f;
      this.CommitCurrentValue();
    }

    private void Start()
    {
      this.Verify();
      this._canvasScaler = this.GetComponentInParent<CanvasScaler>();
    }

    private bool Verify()
    {
      if (!((UnityEngine.Object) this.TargetLayoutElement == (UnityEngine.Object) null) || !((UnityEngine.Object) this.TargetRectTransform == (UnityEngine.Object) null))
        return true;
      Debug.LogWarning((object) "DragHandle: TargetLayoutElement and TargetRectTransform are both null. Disabling behaviour.");
      this.enabled = false;
      return false;
    }

    private float GetCurrentValue()
    {
      if ((UnityEngine.Object) this.TargetLayoutElement != (UnityEngine.Object) null)
        return this.Axis == RectTransform.Axis.Horizontal ? this.TargetLayoutElement.preferredWidth : this.TargetLayoutElement.preferredHeight;
      if ((UnityEngine.Object) this.TargetRectTransform != (UnityEngine.Object) null)
        return this.Axis == RectTransform.Axis.Horizontal ? this.TargetRectTransform.sizeDelta.x : this.TargetRectTransform.sizeDelta.y;
      throw new InvalidOperationException();
    }

    private void SetCurrentValue(float value)
    {
      if ((UnityEngine.Object) this.TargetLayoutElement != (UnityEngine.Object) null)
      {
        if (this.Axis == RectTransform.Axis.Horizontal)
          this.TargetLayoutElement.preferredWidth = value;
        else
          this.TargetLayoutElement.preferredHeight = value;
      }
      else
      {
        Vector2 vector2 = (UnityEngine.Object) this.TargetRectTransform != (UnityEngine.Object) null ? this.TargetRectTransform.sizeDelta : throw new InvalidOperationException();
        if (this.Axis == RectTransform.Axis.Horizontal)
          vector2.x = value;
        else
          vector2.y = value;
        this.TargetRectTransform.sizeDelta = vector2;
      }
    }

    private void CommitCurrentValue()
    {
      if (!((UnityEngine.Object) this.TargetLayoutElement != (UnityEngine.Object) null))
        return;
      if (this.Axis == RectTransform.Axis.Horizontal)
        this.TargetLayoutElement.preferredWidth = ((RectTransform) this.TargetLayoutElement.transform).sizeDelta.x;
      else
        this.TargetLayoutElement.preferredHeight = ((RectTransform) this.TargetLayoutElement.transform).sizeDelta.y;
    }

    private float GetMinSize()
    {
      return (UnityEngine.Object) this.TargetLayoutElement == (UnityEngine.Object) null ? 0.0f : (this.Axis == RectTransform.Axis.Horizontal ? this.TargetLayoutElement.minWidth : this.TargetLayoutElement.minHeight);
    }

    private float GetMaxSize() => (double) this.MaxSize > 0.0 ? this.MaxSize : float.MaxValue;
  }
}
