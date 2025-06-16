namespace Engine.Impl.UI.Controls
{
  public class ProgressImage : ProgressView
  {
    [SerializeField]
    private Image image;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) image != (Object) null))
        return;
      image.fillAmount = Progress;
    }
  }
}
