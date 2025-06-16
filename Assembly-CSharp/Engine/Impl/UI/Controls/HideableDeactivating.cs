namespace Engine.Impl.UI.Controls
{
  public class HideableDeactivating : HideableView
  {
    [SerializeField]
    private GameObject target;

    protected override void ApplyVisibility()
    {
      if ((Object) target != (Object) null)
        target.SetActive(Visible);
      else
        this.gameObject.SetActive(Visible);
    }
  }
}
