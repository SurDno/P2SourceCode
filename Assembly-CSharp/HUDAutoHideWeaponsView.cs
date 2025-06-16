public class HUDAutoHideWeaponsView : HUDWeaponsView
{
  public override void AssignCurrentItem()
  {
    base.AssignCurrentItem();
    this.changeEventView?.Invoke();
  }
}
