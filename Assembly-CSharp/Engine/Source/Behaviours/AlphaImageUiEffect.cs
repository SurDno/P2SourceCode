// Decompiled with JetBrains decompiler
// Type: Engine.Source.Behaviours.AlphaImageUiEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Source.Behaviours
{
  public class AlphaImageUiEffect : ProgressViewBase
  {
    [SerializeField]
    private RawImage rawImage = (RawImage) null;
    [SerializeField]
    private Image image = (Image) null;

    public override float Progress
    {
      get
      {
        if ((Object) this.rawImage != (Object) null)
          return this.rawImage.color.a;
        return (Object) this.image != (Object) null ? this.image.color.a : 1f;
      }
      set
      {
        if ((Object) this.rawImage != (Object) null)
          this.rawImage.color = this.rawImage.color with
          {
            a = value
          };
        if (!((Object) this.image != (Object) null))
          return;
        this.image.color = this.image.color with
        {
          a = value
        };
      }
    }

    public override void SkipAnimation()
    {
    }
  }
}
