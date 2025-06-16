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
    private Image circleBase;
    [SerializeField]
    [FormerlySerializedAs("CircleFill")]
    private Image circleFill;
    private float progress;

    [Inspected]
    public override float Progress
    {
      get => progress;
      set
      {
        if (value <= 0.0)
        {
          gameObject.SetActive(false);
        }
        else
        {
          if (progress <= 0.0)
            gameObject.SetActive(true);
          if (circleFill != null)
            circleFill.fillAmount = value;
          if (circleBase != null)
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
