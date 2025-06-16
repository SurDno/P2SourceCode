namespace Engine.Impl.UI.Controls
{
  public class SumColorView : BinaryColorView
  {
    [SerializeField]
    private ColorViewHandle view;

    protected override void ApplyValues(bool instant)
    {
      view.SetValue(GetValue(0) + GetValue(1), instant);
    }
  }
}
