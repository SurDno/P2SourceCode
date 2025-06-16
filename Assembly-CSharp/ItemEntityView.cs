// Decompiled with JetBrains decompiler
// Type: ItemEntityView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
public class ItemEntityView : EntityView
{
  [SerializeField]
  private ItemView view;
  private IEntity entity;

  public override IEntity Value
  {
    get => this.entity;
    set
    {
      if (this.entity == value)
        return;
      this.entity = value;
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Storable = this.entity?.GetComponent<StorableComponent>();
    }
  }
}
