// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressGradient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressGradient : ProgressView
  {
    [SerializeField]
    private Gradient endGradient;
    [SerializeField]
    private Gradient startGradient;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if ((Object) this.endGradient != (Object) null)
        this.endGradient.EndPosition = this.Progress;
      if (!((Object) this.startGradient != (Object) null))
        return;
      this.startGradient.StartPosition = this.Progress;
    }
  }
}
