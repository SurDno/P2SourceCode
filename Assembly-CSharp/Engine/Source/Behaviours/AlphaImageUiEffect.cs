using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Source.Behaviours
{
  public class AlphaImageUiEffect : ProgressViewBase
  {
    [SerializeField]
    private RawImage rawImage = (RawImage) null;
    [SerializeField]
    private Image image = (Image) null;

    public override float Progress
    {
      get
      {
        if ((Object) this.rawImage != (Object) null)
          return this.rawImage.color.a;
        return (Object) this.image != (Object) null ? this.image.color.a : 1f;
      }
      set
      {
        if ((Object) this.rawImage != (Object) null)
          this.rawImage.color = this.rawImage.color with
          {
            a = value
          };
        if (!((Object) this.image != (Object) null))
          return;
        this.image.color = this.image.color with
        {
          a = value
        };
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
