// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableCouple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class HideableCouple : HideableView
  {
    [SerializeField]
    private HideableView positiveView = (HideableView) null;
    [SerializeField]
    private HideableView negativeView = (HideableView) null;

    public override void SkipAnimation()
    {
      base.SkipAnimation();
      if ((Object) this.positiveView != (Object) null)
        this.positiveView.SkipAnimation();
      if (!((Object) this.negativeView != (Object) null))
        return;
      this.negativeView.SkipAnimation();
    }

    protected override void ApplyVisibility()
    {
      if ((Object) this.positiveView != (Object) null)
        this.positiveView.Visible = this.Visible;
      if (!((Object) this.negativeView != (Object) null))
        return;
      this.negativeView.Visible = !this.Visible;
    }
  }
}
