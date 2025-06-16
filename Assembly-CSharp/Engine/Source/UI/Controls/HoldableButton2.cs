using Engine.Impl.UI.Controls;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Source.UI.Controls
{
  public class HoldableButton2 : Selectable
  {
    [SerializeField]
    private ProgressViewBase progressView = (ProgressViewBase) null;
    [SerializeField]
    private HideableView holdView = (HideableView) null;
    [SerializeField]
    private EventView cancelView = (EventView) null;
    private float currentHoldTime = -1f;

    public float HoldTime { get; set; } = 2f;

    public event Action OpenBeginEvent;

    public event Action<bool> OpenEndEvent;

    public event Action SelectEvent;

    public event Action DeselectEvent;

    private float CurrentHoldTime
    {
      get => this.currentHoldTime;
      set
      {
        this.currentHoldTime = value;
        if (!((UnityEngine.Object) this.progressView != (UnityEngine.Object) null))
          return;
        this.progressView.Progress = (double) this.currentHoldTime >= 0.0 && (double) this.currentHoldTime <= (double) this.HoldTime ? this.currentHoldTime / this.HoldTime : 0.0f;
      }
    }

    private void Cancel()
    {
      this.CurrentHoldTime = -1f;
      Action<bool> openEndEvent = this.OpenEndEvent;
      if (openEndEvent != null)
        openEndEvent(false);
      if ((UnityEngine.Object) this.holdView != (UnityEngine.Object) null)
        this.holdView.Visible = false;
      if (!((UnityEngine.Object) this.cancelView != (UnityEngine.Object) null))
        return;
      this.cancelView.Invoke();
    }

    private void EndHold()
    {
      if (!this.IsTimerRunning())
        return;
      this.Cancel();
    }

    private void Finish()
    {
      this.CurrentHoldTime = this.HoldTime + 1f;
      Action<bool> openEndEvent = this.OpenEndEvent;
      if (openEndEvent != null)
        openEndEvent(true);
      if (!((UnityEngine.Object) this.holdView != (UnityEngine.Object) null))
        return;
      this.holdView.Visible = false;
    }

    private bool IsTimerRunning()
    {
      return (double) this.CurrentHoldTime >= 0.0 && (double) this.CurrentHoldTime <= (double) this.HoldTime;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
      base.OnPointerExit(eventData);
      Action deselectEvent = this.DeselectEvent;
      if (deselectEvent == null)
        return;
      deselectEvent();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      if (!this.IsActive() || !this.IsInteractable() || eventData.button != 0)
        return;
      this.StartHold();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      if (eventData.button != 0)
        return;
      this.EndHold();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
      base.OnPointerEnter(eventData);
      Action selectEvent = this.SelectEvent;
      if (selectEvent == null)
        return;
      selectEvent();
    }

    private void StartHold()
    {
      if (this.IsTimerRunning())
        return;
      this.CurrentHoldTime = Time.deltaTime;
      Action openBeginEvent = this.OpenBeginEvent;
      if (openBeginEvent != null)
        openBeginEvent();
      if (!((UnityEngine.Object) this.holdView != (UnityEngine.Object) null))
        return;
      this.holdView.Visible = true;
    }

    private void Update()
    {
      if (!this.IsTimerRunning())
        return;
      if (!this.IsInteractable())
      {
        this.Cancel();
      }
      else
      {
        float num = this.CurrentHoldTime + Time.deltaTime;
        if ((double) num >= (double) this.HoldTime)
          this.Finish();
        else
          this.CurrentHoldTime = num;
      }
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      this.EndHold();
    }

    public void GamepadStartHold() => this.StartHold();

    public void GamepadEndHold() => this.EndHold();
  }
}
