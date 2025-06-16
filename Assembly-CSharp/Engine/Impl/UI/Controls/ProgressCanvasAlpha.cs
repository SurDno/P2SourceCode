using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressCanvasAlpha : ProgressView
  {
    [SerializeField]
    private CanvasGroup canvasGroup;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.canvasGroup != (Object) null))
        return;
      this.canvasGroup.alpha = this.Progress;
      bool flag = (double) this.Progress > 0.0;
      this.canvasGroup.interactable = flag;
      this.canvasGroup.blocksRaycasts = flag;
    }
  }
}
