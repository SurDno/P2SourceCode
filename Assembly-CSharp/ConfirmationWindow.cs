// Decompiled with JetBrains decompiler
// Type: ConfirmationWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using InputServices;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
public class ConfirmationWindow : MonoBehaviour
{
  [SerializeField]
  private StringView textView;
  [SerializeField]
  private Button acceptButton;
  [SerializeField]
  private Button cancelButton;
  private Action onAcceptAction;
  private Action onCancelAction;
  [SerializeField]
  private GameObject consoleAcceptButton;
  [SerializeField]
  private GameObject consoleCancelButton;

  private void Accept()
  {
    Action onAcceptAction = this.onAcceptAction;
    this.Hide();
    if (onAcceptAction != null)
      onAcceptAction();
    InputService.Instance.ChangeGameSession();
  }

  private void Awake()
  {
    this.acceptButton.onClick.AddListener(new UnityAction(this.Accept));
    this.cancelButton.onClick.AddListener(new UnityAction(this.Cancel));
  }

  private void Cancel()
  {
    Action onCancelAction = this.onCancelAction;
    this.Hide();
    if (onCancelAction == null)
      return;
    onCancelAction();
  }

  public void Hide()
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.OnSelection));
    service.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.OnSelection));
    this.onAcceptAction = (Action) null;
    this.onCancelAction = (Action) null;
    this.gameObject.SetActive(false);
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
  }

  public void Show(string text, Action onAccept, Action onCancel)
  {
    this.textView.StringValue = text;
    this.onAcceptAction = onAccept;
    this.onCancelAction = onCancel;
    this.gameObject.SetActive(true);
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    this.OnJoystick(InputService.Instance.JoystickUsed);
  }

  private bool OnSelection(GameActionType type, bool down)
  {
    if (type == GameActionType.Submit & down)
    {
      this.Accept();
      return true;
    }
    if (!(type == GameActionType.Cancel & down))
      return false;
    this.Cancel();
    return true;
  }

  private void OnJoystick(bool joystick)
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    if (joystick)
    {
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.OnSelection), true);
      service.AddListener(GameActionType.Cancel, new GameActionHandle(this.OnSelection), true);
      this.consoleAcceptButton.SetActive(true);
      this.consoleCancelButton.SetActive(true);
      this.cancelButton.gameObject.SetActive(false);
      this.acceptButton.gameObject.SetActive(false);
    }
    else
    {
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.OnSelection));
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.OnSelection));
      this.consoleAcceptButton.SetActive(false);
      this.consoleCancelButton.SetActive(false);
      this.cancelButton.gameObject.SetActive(true);
      this.acceptButton.gameObject.SetActive(true);
    }
  }
}
