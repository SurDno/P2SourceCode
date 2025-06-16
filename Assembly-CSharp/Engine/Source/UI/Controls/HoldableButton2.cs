using System;
using Engine.Impl.UI.Controls;

namespace Engine.Source.UI.Controls
{
  public class HoldableButton2 : Selectable
  {
    [SerializeField]
    private ProgressViewBase progressView = null;
    [SerializeField]
    private HideableView holdView = null;
    [SerializeField]
    private EventView cancelView = null;
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
        if (!((UnityEngine.Object) progressView != (UnityEngine.Object) null))
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
      if ((UnityEngine.Object) holdView != (UnityEngine.Object) null)
        holdView.Visible = false;
      if (!((UnityEngine.Object) cancelView != (UnityEngine.Object) null))
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
      if (!((UnityEngine.Object) holdView != (UnityEngine.Object) null))
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
      if (!this.IsActive() || !this.IsInteractable() || eventData.button != 0)
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
      if (!((UnityEngine.Object) holdView != (UnityEngine.Object) null))
        return;
      holdView.Visible = true;
    }

    private void Update()
    {
      if (!IsTimerRunning())
        return;
      if (!this.IsInteractable())
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
