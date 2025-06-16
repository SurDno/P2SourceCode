// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressSplited
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressSplited : ProgressView
  {
    [SerializeField]
    private FloatView[] nestedViews = new FloatView[0];

    protected override void ApplyProgress()
    {
      foreach (FloatView nestedView in this.nestedViews)
      {
        if ((Object) nestedView != (Object) null)
          nestedView.FloatValue = this.Progress;
      }
    }

    public override void SkipAnimation()
    {
      foreach (FloatView nestedView in this.nestedViews)
      {
        if ((Object) nestedView != (Object) null)
          nestedView.SkipAnimation();
      }
    }
  }
}
