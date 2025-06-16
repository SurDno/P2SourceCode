using Engine.Impl.UI.Controls;
using InputServices;

public class KeyCodeStringView : StringView
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private GameObject textObject;
  [SerializeField]
  private Text text;
  [SerializeField]
  private StringSpriteMap mouseMap;
  [SerializeField]
  private StringSpriteMap xboxMap;
  [SerializeField]
  private StringSpriteMap psMap;
  [SerializeField]
  private StringMap keyMap;

  public override void SkipAnimation()
  {
  }

  private void OnEnable() => Apply(InputService.Instance.JoystickUsed);

  private void OnDisable()
  {
  }

  protected override void ApplyStringValue() => Apply(InputService.Instance.JoystickUsed);

  private void Apply(bool joystick)
  {
    Sprite sprite = joystick ? xboxMap?.GetValue(StringValue) : mouseMap?.GetValue(StringValue);
    if ((Object) sprite != (Object) null)
    {
      textObject.gameObject.SetActive(false);
      image.sprite = sprite;
      image.gameObject.SetActive(true);
      ((RectTransform) image.transform).sizeDelta = image.sprite.rect.size;
    }
    else
    {
      string str = keyMap?.GetValue(StringValue) ?? StringValue;
      image.gameObject.SetActive(false);
      text.text = str;
      textObject.gameObject.SetActive(true);
    }
  }
}
