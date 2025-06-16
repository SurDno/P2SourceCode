using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressFloatArrayItem : ProgressView
  {
    [SerializeField]
    private FloatArrayView view = (FloatArrayView) null;
    [SerializeField]
    private int index;

    protected override void ApplyProgress()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SetValue(this.index, this.Progress);
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }
  }
}
