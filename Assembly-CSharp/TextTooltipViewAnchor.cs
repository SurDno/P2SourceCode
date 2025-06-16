using UnityEngine;

public class TextTooltipViewAnchor : TextTooltipView
{
  [SerializeField]
  private TextTooltipView prefab;
  private TextTooltipView view;

  public override void Hide() => this.view?.Hide();

  private void OnEnable()
  {
    if (!((Object) TextTooltipView.Current == (Object) null))
      return;
    TextTooltipView.Current = (TextTooltipView) this;
  }

  private void OnDisable()
  {
    if (!((Object) TextTooltipView.Current == (Object) this))
      return;
    TextTooltipView.Current = (TextTooltipView) null;
  }

  public override void Show(Vector2 screenPosition, string text)
  {
    if ((Object) this.view == (Object) null)
      this.view = Object.Instantiate<TextTooltipView>(this.prefab, this.transform, false);
    this.view.Show(screenPosition, text);
  }
}
