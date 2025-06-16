using Engine.Source.Utility;
using InputServices;
using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class StringAndHoldGameActionView : GameActionViewBase
  {
    [SerializeField]
    private GameObject holdObject;
    [SerializeField]
    private KeyCodeStringView keyCodeStringView;

    private void OnEnable()
    {
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.SetCodeView);
      this.ApplyValue(true);
    }

    private void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.SetCodeView);
    }

    protected override void ApplyValue(bool instant)
    {
      this.SetCodeView(InputService.Instance.JoystickUsed);
    }

    private void SetCodeView(bool joystick)
    {
      bool hold;
      this.keyCodeStringView.StringValue = InputUtility.GetHotKeyByAction(this.GetValue(), joystick, out hold);
      this.holdObject.SetActive(hold);
    }
  }
}
