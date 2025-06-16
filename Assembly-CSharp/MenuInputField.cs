using System;
using Engine.Impl.UI.Controls;

public class MenuInputField : MonoBehaviour
{
  [SerializeField]
  private InputField inputField;
  [SerializeField]
  private StringView messageView;
  [SerializeField]
  private StringView placeholderView;

  public event Action<string> SendEvent;

  private void Awake()
  {
    inputField.onEndEdit.AddListener(new UnityAction<string>(OnEndEdit));
  }

  public void ClearValue() => inputField.text = string.Empty;

  private void OnEnable()
  {
    ClearValue();
    SetMessage(string.Empty);
  }

  private void OnEndEdit(string value)
  {
    if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
      return;
    Action<string> sendEvent = SendEvent;
    if (sendEvent == null)
      return;
    sendEvent(value);
  }

  public void SetPlaceholder(string value)
  {
    if (!((UnityEngine.Object) placeholderView != (UnityEngine.Object) null))
      return;
    placeholderView.StringValue = value;
  }

  public void SetMessage(string value)
  {
    if (!((UnityEngine.Object) messageView != (UnityEngine.Object) null))
      return;
    messageView.StringValue = value;
  }
}
