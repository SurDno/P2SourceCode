namespace Engine.Impl.UI.Controls
{
  public class ProgressGradient : ProgressView
  {
    [SerializeField]
    private Gradient endGradient;
    [SerializeField]
    private Gradient startGradient;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if ((Object) endGradient != (Object) null)
        endGradient.EndPosition = Progress;
      if (!((Object) startGradient != (Object) null))
        return;
      startGradient.StartPosition = Progress;
    }
  }
}
