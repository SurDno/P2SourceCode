public class MouseKeyCodeView : KeyCodeViewBase
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private Sprite[] buttonIcons;

  protected override void ApplyValue(bool instant)
  {
    if ((Object) image == (Object) null)
      return;
    if (buttonIcons != null)
    {
      int index = (int) (GetValue() - 323);
      if (index >= 0 || index < buttonIcons.Length)
      {
        image.sprite = buttonIcons[index];
        return;
      }
    }
    image.sprite = (Sprite) null;
  }
}
