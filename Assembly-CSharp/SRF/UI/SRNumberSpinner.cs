// Decompiled with JetBrains decompiler
// Type: SRF.UI.SRNumberSpinner
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
        this._dragStartAmount = double.Parse(this.text);
        this._currentValue = this._dragStartAmount;
        float val1 = 1f;
        if (this.contentType == InputField.ContentType.IntegerNumber)
          val1 *= 10f;
        this._dragStep = Math.Max((double) val1, this._dragStartAmount * 0.05000000074505806);
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
      this._currentValue += Math.Abs(this._dragStep) * (double) x * (double) this.DragSensitivity;
      this._currentValue = Math.Round(this._currentValue, 2);
      if (this._currentValue > this.MaxValue)
        this._currentValue = this.MaxValue;
      if (this._currentValue < this.MinValue)
        this._currentValue = this.MinValue;
      if (this.contentType == InputField.ContentType.IntegerNumber)
        this.text = ((int) Math.Round(this._currentValue)).ToString();
      else
        this.text = this._currentValue.ToString();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
      if (!this.interactable)
        return;
      eventData.Use();
      if (this._dragStartAmount == this._currentValue)
        return;
      this.DeactivateInputField();
      this.SendOnSubmit();
    }
  }
}
