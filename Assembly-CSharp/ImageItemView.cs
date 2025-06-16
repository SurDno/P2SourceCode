using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;

public class ImageItemView : ItemView
{
  [SerializeField]
  private Image image;
  [SerializeField]
  private InventoryCellSizeEnum style;
  private StorableComponent storable;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      InventoryPlaceholder placeholder = storable?.Placeholder;
      if (placeholder == null)
      {
        image.sprite = (Sprite) null;
        image.enabled = false;
      }
      else
      {
        image.sprite = InventoryUtility.GetSpriteByStyle(placeholder, style);
        image.enabled = true;
      }
    }
  }
}
