namespace Engine.Impl.UI.Controls
{
  public class HideableFloat : HideableView
  {
    [SerializeField]
    private FloatView view;

    protected override void ApplyVisibility()
    {
      if (!((Object) view != (Object) null))
        return;
      view.FloatValue = Visible ? 1f : 0.0f;
    }
  }
}
