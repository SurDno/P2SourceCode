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
      if (currentProgressView == null)
        instant = true;
      if (instant)
      {
        backView?.SetValue(null, true);
        frontView?.SetValue(GetValue(), true);
        if (currentProgressView != null)
        {
          currentProgressView.Progress = 1f;
          currentProgressView.SkipAnimation();
        }
        enabled = false;
      }
      else
      {
        if (enabled)
          return;
        TransitionStart();
        enabled = true;
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
      if (GetValue() == frontView.GetValue())
        enabled = false;
      else
        TransitionStart();
    }
  }
}
