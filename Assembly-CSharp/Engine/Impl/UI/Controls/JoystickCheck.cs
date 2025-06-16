// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.JoystickCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using InputServices;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class JoystickCheck : MonoBehaviour
  {
    [SerializeField]
    private HideableView view;
    private Action<bool> onJoystickAction;

    private void OnDisable()
    {
      InputService.Instance.onJoystickUsedChanged -= this.onJoystickAction;
    }

    private void OnEnable()
    {
      InputService instance = InputService.Instance;
      this.view.Visible = instance.JoystickUsed;
      if (this.onJoystickAction == null)
        this.onJoystickAction = new Action<bool>(this.OnJoystick);
      instance.onJoystickUsedChanged += this.onJoystickAction;
    }

    private void OnJoystick(bool joystick) => this.view.Visible = joystick;
  }
}
