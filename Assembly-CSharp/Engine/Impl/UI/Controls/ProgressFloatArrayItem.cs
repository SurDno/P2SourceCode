namespace Engine.Impl.UI.Controls
{
  public class ProgressFloatArrayItem : ProgressView
  {
    [SerializeField]
    private FloatArrayView view = null;
    [SerializeField]
    private int index;

    protected override void ApplyProgress()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SetValue(index, Progress);
    }

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }
  }
}
