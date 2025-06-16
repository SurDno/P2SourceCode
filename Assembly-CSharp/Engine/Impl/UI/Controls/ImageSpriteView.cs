// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ImageSpriteView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ImageSpriteView : SpriteViewBase
  {
    [SerializeField]
    private Image image;

    protected override void ApplyValue(bool instant)
    {
      if (!((Object) this.image != (Object) null))
        return;
      Sprite sprite = this.GetValue();
      this.image.sprite = sprite;
      this.image.enabled = (bool) (Object) sprite;
    }
  }
}
