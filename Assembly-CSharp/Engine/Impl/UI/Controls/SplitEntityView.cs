// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SplitEntityView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SplitEntityView : EntityViewBase
  {
    [SerializeField]
    private EntityView[] views;

    protected override void ApplyValue()
    {
      if (this.views == null)
        return;
      foreach (EntityView view in this.views)
      {
        if ((Object) view != (Object) null)
          view.Value = this.Value;
      }
    }

    public override void SkipAnimation()
    {
      if (this.views == null)
        return;
      foreach (EntityView view in this.views)
        view?.SkipAnimation();
    }
  }
}
