namespace Engine.Impl.UI.Controls
{
  public class ImageSpriteView : SpriteViewBase
  {
    [SerializeField]
    private Image image;

    protected override void ApplyValue(bool instant)
    {
      if (!((Object) image != (Object) null))
        return;
      Sprite sprite = GetValue();
      image.sprite = sprite;
      image.enabled = (bool) (Object) sprite;
    }
  }
}
