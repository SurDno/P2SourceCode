using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Inspectors;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class DragInventoryCell
  {
    [Inspected]
    public HashSet<InventoryCellUI> ActionCells = [];
    [Inspected]
    public HashSet<InventoryCellUI> BaseCells = [];
    [Inspected]
    public bool IsEnabled;
    [Inspected]
    public Vector2 MouseOffset;
    [Inspected]
    public IStorageComponent Storage;
    [Inspected]
    public IInventoryComponent Container;
    [Inspected]
    public StorableComponent Storable;
    [Inspected]
    public Cell Cell;

    public void Reset()
    {
      IsEnabled = false;
      Storable = null;
      Storage = null;
      Container = null;
      Cell = null;
      MouseOffset = Vector2.zero;
      foreach (InventoryCellUI actionCell in ActionCells)
        actionCell.State = CellState.Default;
      ActionCells.Clear();
      foreach (InventoryCellUI baseCell in BaseCells)
        baseCell.State = CellState.Default;
      BaseCells.Clear();
    }
  }
}
