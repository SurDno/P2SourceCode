using System;

namespace Engine.Impl.UI.Controls
{
  public abstract class HideableView : FloatView
  {
    [SerializeField]
    private bool visible = true;

    public event Action OnChangeEvent;

    public event Action OnSkipAnimationEvent;

    public event VisibilityChanged OnVisibilityChanged;

    public bool Visible
    {
      get => visible;
      set
      {
        if (visible == value)
          return;
        visible = value;
        ApplyVisibility();
        Action onChangeEvent = OnChangeEvent;
        if (onChangeEvent != null)
          onChangeEvent();
        VisibilityChanged visibilityChanged = OnVisibilityChanged;
        if (visibilityChanged == null)
          return;
        visibilityChanged(visible, this);
      }
    }

    public override void SkipAnimation()
    {
      Action skipAnimationEvent = OnSkipAnimationEvent;
      if (skipAnimationEvent == null)
        return;
      skipAnimationEvent();
    }

    public override float FloatValue
    {
      get => Convert.ToSingle(Visible);
      set => Visible = Convert.ToBoolean(value);
    }

    protected virtual void OnValidate() => ApplyVisibility();

    protected abstract void ApplyVisibility();

    public delegate void VisibilityChanged(bool newValue, HideableView view);
  }
}
