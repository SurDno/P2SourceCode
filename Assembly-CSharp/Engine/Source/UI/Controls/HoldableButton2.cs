using System;
using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Source.UI.Controls
{
  public class HoldableButton2 : Selectable
  {
    [SerializeField]
    private ProgressViewBase progressView;
    [SerializeField]
    private HideableView holdView;
    [SerializeField]
    private EventView cancelView;
    private float currentHoldTime = -1f;

    public float HoldTime { get; set; } = 2f;

    public event Action OpenBeginEvent;

    public event Action<bool> OpenEndEvent;

    public event Action SelectEvent;

    public event Action DeselectEvent;

    private float CurrentHoldTime
    {
      get => currentHoldTime;
      set
      {
        currentHoldTime = value;
        if (!(progressView != null))
          return;
        progressView.Progress = currentHoldTime >= 0.0 && currentHoldTime <= (double) HoldTime ? currentHoldTime / HoldTime : 0.0f;
      }
    }

    private void Cancel()
    {
      CurrentHoldTime = -1f;
      Action<bool> openEndEvent = OpenEndEvent;
      if (openEndEvent != null)
        openEndEvent(false);
      if (holdView != null)
        holdView.Visible = false;
      if (!(cancelView != null))
        return;
      cancelView.Invoke();
    }

    private void EndHold()
    {
      if (!IsTimerRunning())
        return;
      Cancel();
    }

    private void Finish()
    {
      CurrentHoldTime = HoldTime + 1f;
      Action<bool> openEndEvent = OpenEndEvent;
      if (openEndEvent != null)
        openEndEvent(true);
      if (!(holdView != null))
        return;
      holdView.Visible = false;
    }

    private bool IsTimerRunning()
    {
      return CurrentHoldTime >= 0.0 && CurrentHoldTime <= (double) HoldTime;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
      base.OnPointerExit(eventData);
      Action deselectEvent = DeselectEvent;
      if (deselectEvent == null)
        return;
      deselectEvent();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      base.OnPointerDown(eventData);
      if (!IsActive() || !IsInteractable() || eventData.button != 0)
        return;
      StartHold();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      base.OnPointerUp(eventData);
      if (eventData.button != 0)
        return;
      EndHold();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
      base.OnPointerEnter(eventData);
      Action selectEvent = SelectEvent;
      if (selectEvent == null)
        return;
      selectEvent();
    }

    private void StartHold()
    {
      if (IsTimerRunning())
        return;
      CurrentHoldTime = Time.deltaTime;
      Action openBeginEvent = OpenBeginEvent;
      if (openBeginEvent != null)
        openBeginEvent();
      if (!(holdView != null))
        return;
      holdView.Visible = true;
    }

    private void Update()
    {
      if (!IsTimerRunning())
        return;
      if (!IsInteractable())
      {
        Cancel();
      }
      else
      {
        float num = CurrentHoldTime + Time.deltaTime;
        if (num >= (double) HoldTime)
          Finish();
        else
          CurrentHoldTime = num;
      }
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      EndHold();
    }

    public void GamepadStartHold() => StartHold();

    public void GamepadEndHold() => EndHold();
  }
}
