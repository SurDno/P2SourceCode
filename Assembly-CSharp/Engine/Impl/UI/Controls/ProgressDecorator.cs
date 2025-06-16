// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressDecorator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class ProgressDecorator : ProgressView
  {
    [SerializeField]
    private ProgressViewBase progressView = (ProgressViewBase) null;

    protected override void ApplyProgress()
    {
      if (!((Object) this.progressView != (Object) null))
        return;
      this.progressView.Progress = this.Progress;
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.progressView != (Object) null))
        return;
      this.progressView.SkipAnimation();
    }
  }
}
