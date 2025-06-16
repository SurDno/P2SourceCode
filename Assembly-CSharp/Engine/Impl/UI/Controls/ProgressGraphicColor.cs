// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressGraphicColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressGraphicColor : ProgressView
  {
    [SerializeField]
    private Graphic graphic;
    [SerializeField]
    private Color minColor = Color.white;
    [SerializeField]
    private Color maxColor = Color.white;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyProgress()
    {
      if (!((Object) this.graphic != (Object) null))
        return;
      this.graphic.color = Color.Lerp(this.minColor, this.maxColor, this.Progress);
    }
  }
}
