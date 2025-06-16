using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressFloatArrayItem : ProgressView
  {
    [SerializeField]
    private FloatArrayView view;
    [SerializeField]
    private int index;

    protected override void ApplyProgress()
    {
      if (!(view != null))
        return;
      view.SetValue(index, Progress);
    }

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }
  }
}
