// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableFading
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
      if ((Object) this.canvasGroup == (Object) null)
        return;
      float alpha = this.canvasGroup.alpha;
      float target = this.Visible ? 1f : 0.0f;
      if ((double) alpha == (double) target)
        return;
      float num = (double) alpha >= (double) target ? this.fadeOutTime : this.fadeInTime;
      if ((double) num > 0.0)
        this.canvasGroup.alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime / num);
      else
        this.canvasGroup.alpha = target;
    }

    private void OnDisable() => this.canvasGroup.alpha = this.Visible ? 1f : 0.0f;

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if (!((Object) this.canvasGroup != (Object) null))
        return;
      this.canvasGroup.alpha = this.Visible ? 1f : 0.0f;
    }

    protected override void ApplyVisibility()
    {
      if (!Application.isPlaying)
        this.SkipAnimation();
      if (!((Object) this.canvasGroup != (Object) null))
        return;
      this.canvasGroup.interactable = this.Visible;
      this.canvasGroup.blocksRaycasts = this.Visible;
    }
  }
}
