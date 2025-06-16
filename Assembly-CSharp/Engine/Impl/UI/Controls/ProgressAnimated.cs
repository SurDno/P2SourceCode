// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressAnimated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressAnimated : ProgressView
  {
    [SerializeField]
    private ProgressViewBase progressView = (ProgressViewBase) null;
    [SerializeField]
    private HideableView increasingEffect = (HideableView) null;
    [SerializeField]
    private HideableView decreasingEffect = (HideableView) null;
    [SerializeField]
    private float smoothTime = 1f;
    [SerializeField]
    private float effectThreshold = 0.0f;
    private float velocity = 0.0f;

    private void Update()
    {
      if ((Object) this.progressView == (Object) null)
        return;
      float progress = this.progressView.Progress;
      if ((double) progress == (double) this.Progress)
        return;
      float num = Mathf.MoveTowards(Mathf.SmoothDamp(progress, this.Progress, ref this.velocity, this.smoothTime), this.Progress, Time.deltaTime * (1f / 1000f));
      this.progressView.Progress = num;
      if ((Object) this.increasingEffect == (Object) this.decreasingEffect)
      {
        if (!((Object) this.increasingEffect != (Object) null))
          return;
        this.increasingEffect.Visible = (double) this.Progress - (double) num > (double) this.effectThreshold || (double) num - (double) this.Progress > (double) this.effectThreshold;
      }
      else
      {
        if ((Object) this.increasingEffect != (Object) null)
          this.increasingEffect.Visible = (double) this.Progress - (double) num > (double) this.effectThreshold;
        if ((Object) this.decreasingEffect != (Object) null)
          this.decreasingEffect.Visible = (double) num - (double) this.Progress > (double) this.effectThreshold;
      }
    }

    public override void SkipAnimation()
    {
      if ((Object) this.progressView != (Object) null)
        this.progressView.Progress = this.Progress;
      this.velocity = 0.0f;
      if ((Object) this.increasingEffect != (Object) null)
        this.increasingEffect.Visible = false;
      if (!((Object) this.decreasingEffect != (Object) null))
        return;
      this.decreasingEffect.Visible = false;
    }

    protected override void ApplyProgress()
    {
      if (Application.isPlaying)
        return;
      this.SkipAnimation();
    }
  }
}
