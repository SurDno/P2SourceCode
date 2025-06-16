using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SwitchingSpriteView : SpriteViewBase
  {
    [SerializeField]
    private SpriteView backView;
    [SerializeField]
    private SpriteView frontView;
    [SerializeField]
    private ProgressView currentProgressView;

    protected override void ApplyValue(bool instant)
    {
      if ((Object) this.currentProgressView == (Object) null)
        instant = true;
      if (instant)
      {
        this.backView?.SetValue((Sprite) null, true);
        this.frontView?.SetValue(this.GetValue(), true);
        if ((Object) this.currentProgressView != (Object) null)
        {
          this.currentProgressView.Progress = 1f;
          this.currentProgressView.SkipAnimation();
        }
        this.enabled = false;
      }
      else
      {
        if (this.enabled)
          return;
        this.TransitionStart();
        this.enabled = true;
      }
    }

    private void TransitionStart()
    {
      this.backView.SetValue(this.frontView.GetValue(), true);
      this.frontView.SetValue(this.GetValue(), true);
      this.currentProgressView.Progress = 0.0f;
      this.currentProgressView.SkipAnimation();
    }

    private void Update()
    {
      if ((double) this.currentProgressView.Progress < 1.0)
        return;
      if ((Object) this.GetValue() == (Object) this.frontView.GetValue())
        this.enabled = false;
      else
        this.TransitionStart();
    }
  }
}
