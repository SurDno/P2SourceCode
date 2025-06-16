namespace Engine.Impl.UI.Controls
{
  public class ProgressRemappedSettings : ProgressView
  {
    [SerializeField]
    private ProgressRemapped view;
    [SerializeField]
    private bool max = false;

    protected override void ApplyProgress()
    {
      if ((Object) view == (Object) null)
        return;
      if (max)
        view.SetMax(Progress);
      else
        view.SetMin(Progress);
    }

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }
  }
}
