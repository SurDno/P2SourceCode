// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.StringAndHoldGameActionView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Utility;
using InputServices;
using System;
using UnityEngine;

#nullable disable
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
