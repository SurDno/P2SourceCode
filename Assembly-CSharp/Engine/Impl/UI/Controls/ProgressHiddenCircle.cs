using Inspectors;

namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class ProgressHiddenCircle : ProgressViewBase
  {
    [SerializeField]
    [FormerlySerializedAs("CircleBase")]
    private Image circleBase = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("CircleFill")]
    private Image circleFill = (Image) null;
    private float progress;

    [Inspected]
    public override float Progress
    {
      get => progress;
      set
      {
        if (value <= 0.0)
        {
          this.gameObject.SetActive(false);
        }
        else
        {
          if (progress <= 0.0)
            this.gameObject.SetActive(true);
          if ((Object) circleFill != (Object) null)
            circleFill.fillAmount = value;
          if ((Object) circleBase != (Object) null)
            circleBase.fillAmount = 1f - value;
        }
        progress = value;
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
