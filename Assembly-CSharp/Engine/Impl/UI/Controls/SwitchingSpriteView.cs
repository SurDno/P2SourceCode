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
      if ((Object) currentProgressView == (Object) null)
        instant = true;
      if (instant)
      {
        backView?.SetValue((Sprite) null, true);
        frontView?.SetValue(GetValue(), true);
        if ((Object) currentProgressView != (Object) null)
        {
          currentProgressView.Progress = 1f;
          currentProgressView.SkipAnimation();
        }
        this.enabled = false;
      }
      else
      {
        if (this.enabled)
          return;
        TransitionStart();
        this.enabled = true;
      }
    }

    private void TransitionStart()
    {
      backView.SetValue(frontView.GetValue(), true);
      frontView.SetValue(GetValue(), true);
      currentProgressView.Progress = 0.0f;
      currentProgressView.SkipAnimation();
    }

    private void Update()
    {
      if (currentProgressView.Progress < 1.0)
        return;
      if ((Object) GetValue() == (Object) frontView.GetValue())
        this.enabled = false;
      else
        TransitionStart();
    }
  }
}
