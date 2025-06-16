// Decompiled with JetBrains decompiler
// Type: ImageItemView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class ImageItemView : ItemView
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private InventoryCellSizeEnum style;
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      InventoryPlaceholder placeholder = this.storable?.Placeholder;
      if (placeholder == null)
      {
        this.image.sprite = (Sprite) null;
        this.image.enabled = false;
      }
      else
      {
        this.image.sprite = InventoryUtility.GetSpriteByStyle(placeholder, this.style);
        this.image.enabled = true;
      }
    }
  }
}
