// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SplitFloatView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SplitFloatView : FloatViewBase
  {
    [SerializeField]
    private FloatView[] views = (FloatView[]) null;

    public override void SkipAnimation()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
      {
        FloatView view = this.views[index];
        if ((Object) view != (Object) null)
          view.SkipAnimation();
      }
    }

    protected override void ApplyFloatValue()
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
      {
        FloatView view = this.views[index];
        if ((Object) view != (Object) null)
          view.FloatValue = this.FloatValue;
      }
    }
  }
}
