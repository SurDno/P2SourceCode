using UnityEngine;
using UnityEngine.UI;

public class MouseKeyCodeView : KeyCodeViewBase
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private Sprite[] buttonIcons;

  protected override void ApplyValue(bool instant)
  {
    if ((Object) this.image == (Object) null)
      return;
    if (this.buttonIcons != null)
    {
      int index = (int) (this.GetValue() - 323);
      if (index >= 0 || index < this.buttonIcons.Length)
      {
        this.image.sprite = this.buttonIcons[index];
        return;
      }
    }
    this.image.sprite = (Sprite) null;
  }
}
