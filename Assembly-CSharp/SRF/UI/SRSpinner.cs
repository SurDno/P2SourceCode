// Decompiled with JetBrains decompiler
// Type: SRF.UI.SRSpinner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/Spinner")]
  public class SRSpinner : Selectable, IDragHandler, IEventSystemHandler, IBeginDragHandler
  {
    private float _dragDelta;
    [SerializeField]
    private SRSpinner.SpinEvent _onSpinDecrement = new SRSpinner.SpinEvent();
    [SerializeField]
    private SRSpinner.SpinEvent _onSpinIncrement = new SRSpinner.SpinEvent();
    public float DragThreshold = 20f;

    public SRSpinner.SpinEvent OnSpinIncrement
    {
      get => this._onSpinIncrement;
      set => this._onSpinIncrement = value;
    }

    public SRSpinner.SpinEvent OnSpinDecrement
    {
      get => this._onSpinDecrement;
      set => this._onSpinDecrement = value;
    }

    public void OnBeginDrag(PointerEventData eventData) => this._dragDelta = 0.0f;

    public void OnDrag(PointerEventData eventData)
    {
      if (!this.interactable)
        return;
      this._dragDelta += eventData.delta.x;
      if ((double) Mathf.Abs(this._dragDelta) <= (double) this.DragThreshold)
        return;
      float num = Mathf.Sign(this._dragDelta);
      int amount = Mathf.FloorToInt(Mathf.Abs(this._dragDelta) / this.DragThreshold);
      if ((double) num > 0.0)
        this.OnIncrement(amount);
      else
        this.OnDecrement(amount);
      this._dragDelta -= (float) amount * this.DragThreshold * num;
    }

    private void OnIncrement(int amount)
    {
      for (int index = 0; index < amount; ++index)
        this.OnSpinIncrement.Invoke();
    }

    private void OnDecrement(int amount)
    {
      for (int index = 0; index < amount; ++index)
        this.OnSpinDecrement.Invoke();
    }

    [Serializable]
    public class SpinEvent : UnityEvent
    {
    }
  }
}
