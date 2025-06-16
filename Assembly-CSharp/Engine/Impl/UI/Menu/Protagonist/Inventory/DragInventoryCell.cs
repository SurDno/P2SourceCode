// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.DragInventoryCell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class DragInventoryCell
  {
    [Inspected]
    public HashSet<InventoryCellUI> ActionCells = new HashSet<InventoryCellUI>();
    [Inspected]
    public HashSet<InventoryCellUI> BaseCells = new HashSet<InventoryCellUI>();
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
      this.IsEnabled = false;
      this.Storable = (StorableComponent) null;
      this.Storage = (IStorageComponent) null;
      this.Container = (IInventoryComponent) null;
      this.Cell = (Cell) null;
      this.MouseOffset = Vector2.zero;
      foreach (InventoryCellUI actionCell in this.ActionCells)
        actionCell.State = CellState.Default;
      this.ActionCells.Clear();
      foreach (InventoryCellUI baseCell in this.BaseCells)
        baseCell.State = CellState.Default;
      this.BaseCells.Clear();
    }
  }
}
