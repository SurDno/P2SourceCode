namespace Engine.Impl.UI.Controls
{
  public class GraphicColorView : SingleColorView
  {
    [SerializeField]
    private Graphic graphic;

    protected override void ApplyValue(bool instant)
    {
      if (!((Object) graphic != (Object) null))
        return;
      graphic.color = GetValue();
    }
  }
}
