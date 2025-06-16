namespace Engine.Impl.UI.Controls
{
  public class HideableColor : HideableView, IValueView<Color>
  {
    [SerializeField]
    private ColorViewHandle view;
    [SerializeField]
    private Color falseColor;
    [SerializeField]
    private Color trueColor;

    public override void SkipAnimation() => ApplyVisibility(true);

    protected override void ApplyVisibility() => ApplyVisibility(false);

    private void ApplyVisibility(bool instant)
    {
      view.SetValue(Visible ? trueColor : falseColor, instant);
    }

    Color IValueView<Color>.GetValue(int id) => id <= 0 ? falseColor : trueColor;

    void IValueView<Color>.SetValue(int id, Color value, bool instant)
    {
      if (id <= 0)
      {
        if (!instant && falseColor == value)
          return;
        falseColor = value;
        ApplyVisibility(instant);
      }
      else
      {
        if (!instant && trueColor == value)
          return;
        trueColor = value;
        ApplyVisibility(instant);
      }
    }
  }
}
