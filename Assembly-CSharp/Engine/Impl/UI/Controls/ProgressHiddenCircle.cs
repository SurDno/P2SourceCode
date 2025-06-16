using Inspectors;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private float progress = 0.0f;

    [Inspected]
    public override float Progress
    {
      get => this.progress;
      set
      {
        if ((double) value <= 0.0)
        {
          this.gameObject.SetActive(false);
        }
        else
        {
          if ((double) this.progress <= 0.0)
            this.gameObject.SetActive(true);
          if ((Object) this.circleFill != (Object) null)
            this.circleFill.fillAmount = value;
          if ((Object) this.circleBase != (Object) null)
            this.circleBase.fillAmount = 1f - value;
        }
        this.progress = value;
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
