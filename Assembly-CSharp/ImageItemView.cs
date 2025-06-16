using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using UnityEngine;
using UnityEngine.UI;

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
