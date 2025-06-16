using Engine.Impl.UI.Controls;

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
        if ((Object) rawImage != (Object) null)
          return rawImage.color.a;
        return (Object) image != (Object) null ? image.color.a : 1f;
      }
      set
      {
        if ((Object) rawImage != (Object) null)
          rawImage.color = rawImage.color with
          {
            a = value
          };
        if (!((Object) image != (Object) null))
          return;
        image.color = image.color with
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
