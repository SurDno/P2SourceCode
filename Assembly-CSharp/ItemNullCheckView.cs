// Decompiled with JetBrains decompiler
// Type: ItemNullCheckView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
public class ItemNullCheckView : ItemView
{
  [SerializeField]
  private HideableView hideableView;
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if (!((Object) this.hideableView != (Object) null))
        return;
      this.hideableView.Visible = this.storable != null;
    }
  }

  public override void SkipAnimation() => this.hideableView?.SkipAnimation();
}
