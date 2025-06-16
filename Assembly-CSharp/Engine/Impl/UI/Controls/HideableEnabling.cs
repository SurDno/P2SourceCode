namespace Engine.Impl.UI.Controls
{
  public class HideableEnabling : HideableView
  {
    [SerializeField]
    private Behaviour component;

    protected override void ApplyVisibility()
    {
      if (!((Object) component != (Object) null))
        return;
      component.enabled = Visible;
    }
  }
}
