using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideablePulse : HideableView
  {
    [SerializeField]
    private ProgressViewBase view;
    [SerializeField]
    private float upTime = 0.5f;
    [SerializeField]
    private float downTime = 0.5f;
    private bool goesUp = false;

    private void Update()
    {
      if ((Object) this.view == (Object) null)
        return;
      float progress = this.view.Progress;
      float num;
      if (this.goesUp)
      {
        num = Mathf.MoveTowards(progress, 1f, (double) this.upTime != 0.0 ? Time.deltaTime / this.upTime : 1f);
        if ((double) num == 1.0)
          this.goesUp = false;
      }
      else
      {
        num = Mathf.MoveTowards(progress, 0.0f, (double) this.downTime != 0.0 ? Time.deltaTime / this.downTime : 1f);
        if ((double) num == 0.0 && this.Visible)
          this.goesUp = true;
      }
      this.view.Progress = num;
    }

    protected override void ApplyVisibility()
    {
      if (!Application.isPlaying)
        this.SkipAnimation();
      this.goesUp = this.goesUp && this.Visible;
    }

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      this.goesUp = this.Visible;
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Progress = 0.0f;
    }
  }
}
