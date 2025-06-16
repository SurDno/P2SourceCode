using System;

public class SaveFileItem : MonoBehaviour
{
  public Action<SaveFileItem> OnPressed;

  public string File { get; set; }

  public void SetText(string text) => this.GetComponent<Text>().text = text;

  public void OnPress()
  {
    Action<SaveFileItem> onPressed = OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }
}
