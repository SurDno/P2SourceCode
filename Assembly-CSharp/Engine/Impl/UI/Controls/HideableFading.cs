using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableFading : HideableView
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float fadeInTime = 0.5f;
    [SerializeField]
    private float fadeOutTime = 0.5f;

    private void Update()
    {
      if (canvasGroup == null)
        return;
      float alpha = canvasGroup.alpha;
      float target = Visible ? 1f : 0.0f;
      if (alpha == (double) target)
        return;
      float num = alpha >= (double) target ? fadeOutTime : fadeInTime;
      if (num > 0.0)
        canvasGroup.alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime / num);
      else
        canvasGroup.alpha = target;
    }

    private void OnDisable() => canvasGroup.alpha = Visible ? 1f : 0.0f;

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if (!(canvasGroup != null))
        return;
      canvasGroup.alpha = Visible ? 1f : 0.0f;
    }

    protected override void ApplyVisibility()
    {
      if (!Application.isPlaying)
        SkipAnimation();
      if (!(canvasGroup != null))
        return;
      canvasGroup.interactable = Visible;
      canvasGroup.blocksRaycasts = Visible;
    }
  }
}
