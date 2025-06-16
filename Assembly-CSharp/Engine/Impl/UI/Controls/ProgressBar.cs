namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class ProgressBar : ProgressViewBase
  {
    [SerializeField]
    [FormerlySerializedAs("_Background")]
    private RawImage background = (RawImage) null;
    [SerializeField]
    [FormerlySerializedAs("_Progress")]
    private float progress;
    [SerializeField]
    [FormerlySerializedAs("_ProgressSlider")]
    private RawImage progressSlider = (RawImage) null;

    public override float Progress
    {
      get => progress;
      set
      {
        progress = value;
        if (progress > 1.0)
          progress = 1f;
        else if (progress < 0.0)
          progress = 0.0f;
        if ((Object) background == (Object) null)
          return;
        RectTransform component = background.gameObject.GetComponent<RectTransform>();
        if ((Object) progressSlider == (Object) null)
          return;
        progressSlider.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (component.rect.width - 2f) * progress);
      }
    }

    public Color Background
    {
      get => (Object) background == (Object) null ? Color.black : background.color;
      set
      {
        if ((Object) background == (Object) null)
          return;
        background.color = value;
      }
    }

    public Color Foreground
    {
      get => (Object) progressSlider == (Object) null ? Color.red : progressSlider.color;
      set
      {
        if ((Object) progressSlider == (Object) null)
          return;
        progressSlider.color = value;
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
