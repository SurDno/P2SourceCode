// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class HideableFloat : HideableView
  {
    [SerializeField]
    private FloatView view;

    protected override void ApplyVisibility()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.FloatValue = this.Visible ? 1f : 0.0f;
    }
  }
}
