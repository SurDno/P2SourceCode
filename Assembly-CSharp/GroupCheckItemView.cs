// Decompiled with JetBrains decompiler
// Type: GroupCheckItemView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Storable;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using System.Linq;
using UnityEngine;

#nullable disable
public class GroupCheckItemView : ItemView
{
  [SerializeField]
  private HideableView hideableView;
  [SerializeField]
  private StorableGroup[] groups = new StorableGroup[0];
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      if ((Object) this.hideableView == (Object) null)
        return;
      if (this.storable != null)
      {
        foreach (StorableGroup group in this.groups)
        {
          if (this.storable.Groups.Contains<StorableGroup>(group))
          {
            this.hideableView.Visible = true;
            return;
          }
        }
      }
      this.hideableView.Visible = false;
    }
  }

  public override void SkipAnimation() => this.hideableView?.SkipAnimation();
}
