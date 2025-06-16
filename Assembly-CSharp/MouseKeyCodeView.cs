// Decompiled with JetBrains decompiler
// Type: MouseKeyCodeView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class MouseKeyCodeView : KeyCodeViewBase
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private Sprite[] buttonIcons;

  protected override void ApplyValue(bool instant)
  {
    if ((Object) this.image == (Object) null)
      return;
    if (this.buttonIcons != null)
    {
      int index = (int) (this.GetValue() - 323);
      if (index >= 0 || index < this.buttonIcons.Length)
      {
        this.image.sprite = this.buttonIcons[index];
        return;
      }
    }
    this.image.sprite = (Sprite) null;
  }
}
