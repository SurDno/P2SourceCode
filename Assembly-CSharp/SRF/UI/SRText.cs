// Decompiled with JetBrains decompiler
// Type: SRF.UI.SRText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRF.UI
{
  [AddComponentMenu("SRF/UI/SRText")]
  public class SRText : Text
  {
    public event Action<SRText> LayoutDirty;

    public override void SetLayoutDirty()
    {
      base.SetLayoutDirty();
      Action<SRText> layoutDirty = this.LayoutDirty;
      if (layoutDirty == null)
        return;
      layoutDirty(this);
    }
  }
}
