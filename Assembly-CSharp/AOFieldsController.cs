using Engine.Impl.UI.Controls;

public class AOFieldsController : HideableView
{
  protected override void ApplyVisibility() => AOField.IsPlayerOutdoor = this.Visible;
}
