using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container
{
  [DisallowMultipleComponent]
  public class ComplexInventoryContainerUI : InventoryContainerUI
  {
    protected Dictionary<Cell, StorableComponent> items = new Dictionary<Cell, StorableComponent>();

    public RectTransform Mask { get; private set; }

    public IEnumerable<IStorableComponent> Items
    {
      get => items.Values;
    }

    public static ComplexInventoryContainerUI Instantiate(
      InventoryCellStyle style,
      GameObject prefab,
      List<StorableComponent> items,
      RectTransform mask)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(prefab);
      gameObject.name = "[Container] Complex";
      ComplexInventoryContainerUI component1 = gameObject.GetComponent<ComplexInventoryContainerUI>();
      component1.content = component1;
      component1.Mask = mask;
      int num = 0;
      foreach (StorableComponent storableComponent in items)
      {
        InventoryCellUI inventoryCellUi = InventoryCellUI.Instantiate(new Vector2(0.0f, (float) (items.Count - 1 - num)), style);
        inventoryCellUi.Transform.SetParent(component1.grid.transform, false);
        inventoryCellUi.Transform.localPosition = inventoryCellUi.Transform.localPosition + (Vector3) style.OutlineOffset;
        component1.cells.Add(storableComponent.Cell, inventoryCellUi);
        component1.items.Add(storableComponent.Cell, storableComponent);
        ++num;
      }
      Vector2 outerSize = InventoryUtility.CalculateOuterSize(new Vector2(1f, (float) items.Count), style);
      component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float) ((double) outerSize.x + (double) style.OutlineOffset.x * 2.0 + 2.0));
      component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float) ((double) outerSize.y + (double) style.OutlineOffset.y * 2.0 + 2.0));
      if ((Object) style.OutlinePrefab != (Object) null)
      {
        RectTransform component2 = Object.Instantiate<GameObject>(style.OutlinePrefab).GetComponent<RectTransform>();
        component2.sizeDelta = new Vector2(outerSize.x + style.OutlineOffset.x * 2f, outerSize.y + style.OutlineOffset.y * 2f);
        InventoryUtility.GetCenter(component1);
        component2.SetParent(component1.grid.transform, false);
      }
      if ((Object) component1.button != (Object) null)
        component1.Button.gameObject.SetActive(false);
      return component1;
    }

    public InventoryCellUI GetCellUi(Cell cell)
    {
      return !cells.ContainsKey(cell) ? null : cells[cell];
    }

    public IStorableComponent GetCellItem(Cell cell)
    {
      return !items.ContainsKey(cell) || items[cell].Cell != cell ? null : (IStorableComponent) items[cell];
    }
  }
}
