namespace Engine.Impl.UI.Controls
{
  public abstract class SingleColorView : MonoBehaviour, IValueView<Color>
  {
    [SerializeField]
    private Color value = Color.white;

    protected Color GetValue() => value;

    public Color GetValue(int id) => value;

    public void SetValue(int id, Color value, bool instant)
    {
      if (!instant && this.value == value)
        return;
      this.value = value;
      ApplyValue(instant);
    }

    protected abstract void ApplyValue(bool instant);
  }
}
