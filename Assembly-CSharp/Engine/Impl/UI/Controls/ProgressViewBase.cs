using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressViewBase : FloatView
  {
    public abstract float Progress { get; set; }

    public override float FloatValue
    {
      get => Progress;
      set => Progress = Mathf.Clamp01(value);
    }
  }
}
