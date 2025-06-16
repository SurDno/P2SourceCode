using Engine.Impl.UI;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;

namespace Engine.Source.UI.Menu.Protagonist.Inventory
{
  public static class InventoryUtility
  {
    public static Vector2 CalculateOuterSize(IInventoryGridBase grid, InventoryCellStyle style)
    {
      Vector2 outerSize = new Vector2 {
        x = (float) grid.Columns * (style.Size.x + style.Offset.x),
        y = (float) grid.Rows * (style.Size.y + style.Offset.y)
      };
      if (grid.Columns > 0)
        outerSize.x -= style.Offset.x;
      if (grid.Rows > 0)
        outerSize.y -= style.Offset.y;
      return outerSize;
    }

    public static Vector2 CalculateOuterSize(Vector2 grid, InventoryCellStyle style)
    {
      Vector2 outerSize = new Vector2 {
        x = grid.x * (style.Size.x + style.Offset.x),
        y = grid.y * (style.Size.y + style.Offset.y)
      };
      if ((double) grid.x > 0.0)
        outerSize.x -= style.Offset.x;
      if ((double) grid.y > 0.0)
        outerSize.y -= style.Offset.y;
      return outerSize;
    }

    public static Vector2 CalculateInnerSize(IInventoryGridBase grid, InventoryCellStyle style)
    {
      return new Vector2 {
        x = (float) grid.Columns * (style.Size.x + style.Offset.x) + style.Offset.y,
        y = (float) grid.Rows * (style.Size.y + style.Offset.y) + style.Offset.y
      };
    }

    public static Vector2 CalculateStorablePosition(Cell cell, InventoryCellStyle style)
    {
      if (cell == null)
        return Vector2.zero;
      return new Vector2 {
        x = (float) cell.Column * (style.Size.x + style.Offset.x) - style.Offset.x,
        y = (float) cell.Row * (style.Size.y + style.Offset.y) - style.Offset.y
      };
    }

    public static Vector2 CalculateGridPosition(int column, int row, InventoryCellStyle style)
    {
      return new Vector2 {
        x = (float) column * (style.Size.x + style.Offset.x),
        y = (float) row * (style.Size.y + style.Offset.y)
      };
    }

    public static Vector2 CalculateGridPosition(Vector2 position, InventoryCellStyle style)
    {
      return new Vector2 {
        x = position.x * (style.Size.x + style.Offset.x),
        y = position.y * (style.Size.y + style.Offset.y)
      };
    }

    public static Vector2 CalculateGridPosition(Cell cell, InventoryCellStyle style)
    {
      return CalculateGridPosition(cell.Column, cell.Row, style);
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
      Vector3 vector3 = point - pivot;
      point = Quaternion.Euler(angles) * vector3 + pivot;
      return point;
    }

    public static Vector2 GetCenter(UIControl base2)
    {
      Vector3 lossyScale = base2.Transform.lossyScale;
      Vector2 position = base2.Transform.rect.position;
      Vector2 size = base2.Transform.rect.size;
      position.Scale((Vector2) lossyScale);
      size.Scale((Vector2) lossyScale);
      return (Vector2) (base2.Transform.rotation * (Vector3) -(position + size / 2f));
    }

    public static Rect GetScaledCoordinates(RectTransform transform)
    {
      Vector2 lossyScale = (Vector2) transform.lossyScale;
      Vector2 position = transform.rect.position;
      Vector2 size = transform.rect.size;
      position.Scale(lossyScale);
      size.Scale(lossyScale);
      return new Rect(position, size);
    }

    public static Sprite GetSpriteByStyle(
      InventoryPlaceholder placeholder,
      InventoryCellSizeEnum size)
    {
      switch (size)
      {
        case InventoryCellSizeEnum.Cell50:
          return placeholder.ImageInventoryCell.Value;
        case InventoryCellSizeEnum.Slot80:
          return placeholder.ImageInventorySlot.Value;
        case InventoryCellSizeEnum.Slot200:
          return placeholder.ImageInventorySlotBig.Value;
        case InventoryCellSizeEnum.Info800:
          return placeholder.ImageInformation.Value;
        case InventoryCellSizeEnum.InfoSpecial:
          return placeholder.ImageInformationSpecial.Value;
        default:
          return (Sprite) null;
      }
    }
  }
}
