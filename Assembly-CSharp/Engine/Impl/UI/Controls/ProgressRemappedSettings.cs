// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressRemappedSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressRemappedSettings : ProgressView
  {
    [SerializeField]
    private ProgressRemapped view;
    [SerializeField]
    private bool max = false;

    protected override void ApplyProgress()
    {
      if ((Object) this.view == (Object) null)
        return;
      if (this.max)
        this.view.SetMax(this.Progress);
      else
        this.view.SetMin(this.Progress);
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }
  }
}
