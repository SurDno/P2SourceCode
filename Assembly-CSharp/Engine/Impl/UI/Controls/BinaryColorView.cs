using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class BinaryColorView : MonoBehaviour, IValueView<Color>
  {
    [SerializeField]
    private Color valueA = Color.white;
    [SerializeField]
    private Color valueB = Color.white;

    public Color GetValue(int id) => id <= 0 ? this.valueA : this.valueB;

    public void SetValue(int id, Color value, bool instant)
    {
      if (id <= 0)
      {
        if (!instant && this.valueA == value)
          return;
        this.valueA = value;
        this.ApplyValues(instant);
      }
      else
      {
        if (!instant && this.valueB == value)
          return;
        this.valueB = value;
        this.ApplyValues(instant);
      }
    }

    protected abstract void ApplyValues(bool instant);
  }
}
