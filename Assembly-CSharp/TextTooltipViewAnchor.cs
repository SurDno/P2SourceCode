public class TextTooltipViewAnchor : TextTooltipView
{
  [SerializeField]
  private TextTooltipView prefab;
  private TextTooltipView view;

  public override void Hide() => view?.Hide();

  private void OnEnable()
  {
    if (!((Object) Current == (Object) null))
      return;
    Current = this;
  }

  private void OnDisable()
  {
    if (!((Object) Current == (Object) this))
      return;
    Current = null;
  }

  public override void Show(Vector2 screenPosition, string text)
  {
    if ((Object) view == (Object) null)
      view = Object.Instantiate<TextTooltipView>(prefab, this.transform, false);
    view.Show(screenPosition, text);
  }
}
