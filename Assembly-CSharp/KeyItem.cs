using System;
using Engine.Source.Services.Inputs;

public class KeyItem : MonoBehaviour
{
  [SerializeField]
  private Color baseColor;
  [SerializeField]
  private Color activeColor;
  [SerializeField]
  private Color readonlyColor;
  private bool selction;
  public Action<KeyItem> OnPressed;

  public ActionGroup Bind { get; set; }

  public void SetText(string text) => this.GetComponent<Text>().text = text;

  public void SetReadonly() => this.GetComponent<Text>().color = readonlyColor;

  public bool Selection
  {
    get => selction;
    set
    {
      selction = value;
      this.GetComponent<Text>().color = selction ? activeColor : baseColor;
    }
  }

  public void OnPress()
  {
    Action<KeyItem> onPressed = OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }
}
