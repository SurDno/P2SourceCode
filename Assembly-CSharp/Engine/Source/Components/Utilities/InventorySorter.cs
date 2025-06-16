using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Source.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Source.Components.Utilities
{
  public static class InventorySorter
  {
    public static void Sort(IStorageComponent storage)
    {
      InventorySorter.ConcatSameItems(storage);
      InventorySorter.MoveItems(storage);
    }

    private static void ConcatSameItems(IStorageComponent storage)
    {
      List<IStorableComponent> storableComponentList = new List<IStorableComponent>(storage.Items);
      for (int index1 = storableComponentList.Count - 1; index1 > 0; --index1)
      {
        IStorableComponent storableComponent1 = storableComponentList[index1];
        for (int index2 = index1 - 1; index2 >= 0; --index2)
        {
          IStorableComponent storableComponent2 = storableComponentList[index2];
          if (storableComponent1 != storableComponent2 && storableComponent1 != null && storableComponent1.Owner != null && storableComponent2 != null && storableComponent2.Owner != null && storableComponent1.Owner.TemplateId == storableComponent2.Owner.TemplateId && storableComponent2.Count > 0)
          {
            int num1 = storableComponent1.Max - storableComponent1.Count;
            int num2 = storableComponent2.Count > num1 ? num1 : storableComponent2.Count;
            storableComponent1.Count += num2;
            storableComponent2.Count -= num2;
            if (storableComponent2.Count == 0)
              storableComponent2.Owner.Dispose();
          }
        }
      }
    }

    private static void MoveItems(IStorageComponent storage)
    {
      IOrderedEnumerable<IInventoryComponent> orderedEnumerable = storage.Containers.Where<IInventoryComponent>((Func<IInventoryComponent, bool>) (c => c.Enabled.Value && c.GetGroup() == InventoryGroup.Backpack)).OrderBy<IInventoryComponent, int>((Func<IInventoryComponent, int>) (c => c.GetGrid().Rows * c.GetGrid().Columns));
      List<StorableComponent> itemsLeft = new List<IStorableComponent>(storage.Items.Where<IStorableComponent>((Func<IStorableComponent, bool>) (item => item.Container.GetGroup() == InventoryGroup.Backpack))).ConvertAll<StorableComponent>((Converter<IStorableComponent, StorableComponent>) (item => item as StorableComponent));
      foreach (IInventoryComponent inventoryComponent in (IEnumerable<IInventoryComponent>) orderedEnumerable)
      {
        if (itemsLeft.Count == 0)
          break;
        int[][] greedMatrix = InventorySorter.CreateGreedMatrix(inventoryComponent.GetGrid());
        foreach (StorableComponent storableComponent in InventorySorter.GetItemsToFillContainer(inventoryComponent, greedMatrix, itemsLeft))
          itemsLeft.Remove(storableComponent);
      }
    }

    private static List<StorableComponent> GetItemsToFillContainer(
      IInventoryComponent backpack,
      int[][] backpackMatrix,
      List<StorableComponent> itemsLeft)
    {
      IEnumerable<StorableComponent> storableComponents = itemsLeft.Where<StorableComponent>((Func<StorableComponent, bool>) (item => item.Placeholder.Grid.Rows <= backpack.GetGrid().Rows && item.Placeholder.Grid.Columns <= backpack.GetGrid().Columns)).OrderBy<StorableComponent, int>((Func<StorableComponent, int>) (item => item.Placeholder.Grid.Columns * item.Placeholder.Grid.Rows)).Reverse<StorableComponent>();
      List<StorableComponent> itemsToFillContainer = new List<StorableComponent>();
      foreach (StorableComponent storableComponent in storableComponents)
      {
        Cell cellToPlace = InventorySorter.FindCellToPlace(backpackMatrix, storableComponent);
        if (cellToPlace != null)
        {
          InventorySorter.Place(backpack, backpackMatrix, storableComponent, cellToPlace);
          itemsToFillContainer.Add(storableComponent);
        }
      }
      return itemsToFillContainer;
    }

    private static void Place(
      IInventoryComponent backpack,
      int[][] backpackMatrix,
      StorableComponent item,
      Cell cell)
    {
      item.Container = backpack;
      item.Cell.Column = cell.Column;
      item.Cell.Row = cell.Row;
      int[][] greedMatrix = InventorySorter.CreateGreedMatrix((IInventoryGridBase) item.Placeholder.Grid);
      int row = cell.Row;
      int column = cell.Column;
      for (int index1 = 0; index1 < greedMatrix.Length; ++index1)
      {
        for (int index2 = 0; index2 < greedMatrix[index1].Length; ++index2)
          backpackMatrix[row + index1][column + index2] = 1;
      }
    }

    private static Cell FindCellToPlace(int[][] backpackMatrix, StorableComponent item)
    {
      int[][] greedMatrix = InventorySorter.CreateGreedMatrix((IInventoryGridBase) item.Placeholder.Grid);
      int iPlace = 0;
      for (int length1 = backpackMatrix.Length; iPlace < length1; ++iPlace)
      {
        int jPlace = 0;
        for (int length2 = backpackMatrix[iPlace].Length; jPlace < length2; ++jPlace)
        {
          if (InventorySorter.CanPlace(backpackMatrix, greedMatrix, iPlace, jPlace))
            return new Cell()
            {
              Row = iPlace,
              Column = jPlace
            };
        }
      }
      return (Cell) null;
    }

    private static bool CanPlace(int[][] m1, int[][] m2, int iPlace, int jPlace)
    {
      bool flag = false;
      if (m1.Length >= iPlace + m2.Length && m1[0].Length >= jPlace + m2[0].Length)
      {
        int index1 = iPlace;
        for (int index2 = 0; index1 < m1.Length && index2 < m2.Length; ++index2)
        {
          int index3 = jPlace;
          for (int index4 = 0; index3 < m1[index1].Length && index4 < m2[index2].Length; ++index4)
          {
            if (m1[index1][index3] != 0)
              return false;
            flag = true;
            ++index3;
          }
          ++index1;
        }
      }
      return flag;
    }

    private static int[][] CreateGreedMatrix(IInventoryGridBase greed)
    {
      int[][] greedMatrix = new int[greed.Rows][];
      for (int index = 0; index < greed.Rows; ++index)
        greedMatrix[index] = new int[greed.Columns];
      return greedMatrix;
    }
  }
}
