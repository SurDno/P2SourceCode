using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  public class ProgressBar : ProgressViewBase
  {
    [SerializeField]
    [FormerlySerializedAs("_Background")]
    private RawImage background;
    [SerializeField]
    [FormerlySerializedAs("_Progress")]
    private float progress;
    [SerializeField]
    [FormerlySerializedAs("_ProgressSlider")]
    private RawImage progressSlider;

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
        if (background == null)
          return;
        RectTransform component = background.gameObject.GetComponent<RectTransform>();
        if (progressSlider == null)
          return;
        progressSlider.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (component.rect.width - 2f) * progress);
      }
    }

    public Color Background
    {
      get => background == null ? Color.black : background.color;
      set
      {
        if (background == null)
          return;
        background.color = value;
      }
    }

    public Color Foreground
    {
      get => progressSlider == null ? Color.red : progressSlider.color;
      set
      {
        if (progressSlider == null)
          return;
        progressSlider.color = value;
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
