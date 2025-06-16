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
      if (!((Object) canvasGroup != (Object) null))
        return;
      canvasGroup.alpha = Progress;
      bool flag = Progress > 0.0;
      canvasGroup.interactable = flag;
      canvasGroup.blocksRaycasts = flag;
    }
  }
}
