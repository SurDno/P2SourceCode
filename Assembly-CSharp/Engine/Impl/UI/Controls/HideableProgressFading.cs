// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableProgressFading
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class HideableProgressFading : HideableView
  {
    [SerializeField]
    private ProgressViewBase progressView;
    [SerializeField]
    private float fadeInTime = 0.5f;
    [SerializeField]
    private float fadeOutTime = 0.5f;

    private void Update()
    {
      if ((Object) this.progressView == (Object) null)
        return;
      float progress = this.progressView.Progress;
      float target = this.Visible ? 1f : 0.0f;
      if ((double) progress == (double) target)
        return;
      float num = (double) progress >= (double) target ? this.fadeOutTime : this.fadeInTime;
      if ((double) num > 0.0)
        this.progressView.Progress = Mathf.MoveTowards(progress, target, Time.deltaTime / num);
      else
        this.progressView.Progress = target;
    }

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if ((Object) this.progressView == (Object) null)
        return;
      this.progressView.Progress = this.Visible ? 1f : 0.0f;
      this.progressView.SkipAnimation();
    }

    protected override void ApplyVisibility()
    {
      if (Application.isPlaying)
        return;
      this.SkipAnimation();
    }
  }
}
