using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressRemappedSettings : ProgressView
  {
    [SerializeField]
    private ProgressRemapped view;
    [SerializeField]
    private bool max = false;

    protected override void ApplyProgress()
    {
      if ((Object) this.view == (Object) null)
        return;
      if (this.max)
        this.view.SetMax(this.Progress);
      else
        this.view.SetMin(this.Progress);
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }
  }
}
