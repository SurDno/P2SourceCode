namespace Engine.Impl.UI.Controls
{
  public abstract class FloatView : MonoBehaviour
  {
    public abstract float FloatValue { get; set; }

    public abstract void SkipAnimation();
  }
}
