// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressCurve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressCurve : ProgressView
  {
    [SerializeField]
    private FloatView view = (FloatView) null;
    [SerializeField]
    private AnimationCurve curve = new AnimationCurve();

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.FloatValue = this.curve.Evaluate(this.FloatValue);
    }
  }
}
