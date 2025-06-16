namespace Engine.Impl.UI.Controls
{
  public abstract class FloatViewBase : FloatView
  {
    [SerializeField]
    private float floatValue;

    public override float FloatValue
    {
      get => floatValue;
      set
      {
        if (floatValue == (double) value)
          return;
        floatValue = value;
        ApplyFloatValue();
      }
    }

    protected virtual void OnValidate() => ApplyFloatValue();

    protected abstract void ApplyFloatValue();
  }
}
