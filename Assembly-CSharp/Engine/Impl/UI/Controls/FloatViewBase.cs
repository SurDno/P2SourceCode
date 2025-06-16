using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class FloatViewBase : FloatView
  {
    [SerializeField]
    private float floatValue = 0.0f;

    public override float FloatValue
    {
      get => this.floatValue;
      set
      {
        if ((double) this.floatValue == (double) value)
          return;
        this.floatValue = value;
        this.ApplyFloatValue();
      }
    }

    protected virtual void OnValidate() => this.ApplyFloatValue();

    protected abstract void ApplyFloatValue();
  }
}
