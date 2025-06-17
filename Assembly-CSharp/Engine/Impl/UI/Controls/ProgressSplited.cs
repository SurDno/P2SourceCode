using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressSplited : ProgressView
  {
    [SerializeField]
    private FloatView[] nestedViews = [];

    protected override void ApplyProgress()
    {
      foreach (FloatView nestedView in nestedViews)
      {
        if (nestedView != null)
          nestedView.FloatValue = Progress;
      }
    }

    public override void SkipAnimation()
    {
      foreach (FloatView nestedView in nestedViews)
      {
        if (nestedView != null)
          nestedView.SkipAnimation();
      }
    }
  }
}
