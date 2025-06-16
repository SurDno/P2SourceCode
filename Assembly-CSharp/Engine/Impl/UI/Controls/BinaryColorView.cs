using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class BinaryColorView : MonoBehaviour, IValueView<Color>
  {
    [SerializeField]
    private Color valueA = Color.white;
    [SerializeField]
    private Color valueB = Color.white;

    public Color GetValue(int id) => id <= 0 ? valueA : valueB;

    public void SetValue(int id, Color value, bool instant)
    {
      if (id <= 0)
      {
        if (!instant && valueA == value)
          return;
        valueA = value;
        ApplyValues(instant);
      }
      else
      {
        if (!instant && valueB == value)
          return;
        valueB = value;
        ApplyValues(instant);
      }
    }

    protected abstract void ApplyValues(bool instant);
  }
}
