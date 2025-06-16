using System;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using InputServices;

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
    Hide();
    if (onAcceptAction != null)
      onAcceptAction();
    InputService.Instance.ChangeGameSession();
  }

  private void Awake()
  {
    acceptButton.onClick.AddListener(new UnityAction(Accept));
    cancelButton.onClick.AddListener(new UnityAction(Cancel));
  }

  private void Cancel()
  {
    Action onCancelAction = this.onCancelAction;
    Hide();
    if (onCancelAction == null)
      return;
    onCancelAction();
  }

  public void Hide()
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    service.RemoveListener(GameActionType.Submit, OnSelection);
    service.RemoveListener(GameActionType.Cancel, OnSelection);
    onAcceptAction = null;
    onCancelAction = null;
    this.gameObject.SetActive(false);
    InputService.Instance.onJoystickUsedChanged -= OnJoystick;
  }

  public void Show(string text, Action onAccept, Action onCancel)
  {
    textView.StringValue = text;
    onAcceptAction = onAccept;
    onCancelAction = onCancel;
    this.gameObject.SetActive(true);
    InputService.Instance.onJoystickUsedChanged += OnJoystick;
    OnJoystick(InputService.Instance.JoystickUsed);
  }

  private bool OnSelection(GameActionType type, bool down)
  {
    if (type == GameActionType.Submit & down)
    {
      Accept();
      return true;
    }
    if (!(type == GameActionType.Cancel & down))
      return false;
    Cancel();
    return true;
  }

  private void OnJoystick(bool joystick)
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    if (joystick)
    {
      service.AddListener(GameActionType.Submit, OnSelection, true);
      service.AddListener(GameActionType.Cancel, OnSelection, true);
      consoleAcceptButton.SetActive(true);
      consoleCancelButton.SetActive(true);
      cancelButton.gameObject.SetActive(false);
      acceptButton.gameObject.SetActive(false);
    }
    else
    {
      service.RemoveListener(GameActionType.Submit, OnSelection);
      service.RemoveListener(GameActionType.Cancel, OnSelection);
      consoleAcceptButton.SetActive(false);
      consoleCancelButton.SetActive(false);
      cancelButton.gameObject.SetActive(true);
      acceptButton.gameObject.SetActive(true);
    }
  }
}
