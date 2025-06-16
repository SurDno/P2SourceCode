using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class ImageSpriteView : SpriteViewBase
  {
    [SerializeField]
    private Image image;

    protected override void ApplyValue(bool instant)
    {
      if (!((Object) this.image != (Object) null))
        return;
      Sprite sprite = this.GetValue();
      this.image.sprite = sprite;
      this.image.enabled = (bool) (Object) sprite;
    }
  }
}
