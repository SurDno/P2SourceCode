// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Utilities.StorageUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Types;
using Engine.Source.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Utilities
{
  public static class StorageUtility
  {
    private static List<IInventoryComponent> tmpContainers = new List<IInventoryComponent>();
    private static List<IntCell> tmpCells = new List<IntCell>();

    public static Intersect GetIntersect(
      IStorageComponent storage,
      IInventoryComponent container,
      StorableComponent storable,
      Cell cell)
    {
      if (container == null)
        return StorageUtility.GetIntersectFindContainer(storage, storable);
      return cell == null ? StorageUtility.GetIntersectFindCell(storage, container, storable) : StorageUtility.GetIntersectAndCheck(storage, container, storable, cell.To());
    }

    private static Intersect GetIntersectAndCheck(
      IStorageComponent storage,
      IInventoryComponent container,
      StorableComponent storable,
      IntCell cell)
    {
      Intersect intersectAndCheck = new Intersect();
      if (container == null)
      {
        Debug.LogError((object) ("stored.Container == null , owner : " + storage.Owner.GetInfo()));
        return intersectAndCheck;
      }
      if (storage == null || storage.IsDisposed)
      {
        Debug.LogError((object) ("stored.Storage == null || stored.Storage.IsDisposed() , owner : " + storage.Owner.GetInfo()));
        return intersectAndCheck;
      }
      intersectAndCheck.Storage = storage;
      intersectAndCheck.Container = container;
      intersectAndCheck.Storable = storable;
      intersectAndCheck.Cell = cell;
      IList<Cell> cellList;
      if (storable == null || storable.IsDisposed)
        cellList = (IList<Cell>) new List<Cell>()
        {
          cell.To()
        };
      else if (container.GetKind() == ContainerCellKind.OneCellToOneStorable)
      {
        if (storable == null)
        {
          Debug.LogError((object) ("stored.Storable == null , storage owner : " + storage.Owner.GetInfo()));
          return new Intersect();
        }
        if (storable.Placeholder == null)
        {
          Debug.LogError((object) ("stored.Storable.Placeholder == null , owner : " + storable.Owner.GetInfo() + " , storage owner : " + storage.Owner.GetInfo()));
          return new Intersect();
        }
        cellList = (IList<Cell>) new List<Cell>()
        {
          ((InventoryGridLimited) storable.Placeholder.Grid)[0, 0]
        };
      }
      else
      {
        if (storable == null)
        {
          Debug.LogError((object) ("stored.Storable == null , storage owner : " + storage.Owner.GetInfo()));
          return new Intersect();
        }
        if (storable.Placeholder == null)
        {
          Debug.LogError((object) ("stored.Storable.Placeholder == null , owner : " + storable.Owner.GetInfo() + " , storage owner : " + storage.Owner.GetInfo()));
          return new Intersect();
        }
        InventoryPlaceholder placeholder = storable.Placeholder;
        InventoryGridLimited grid = (InventoryGridLimited) placeholder.Grid;
        if (grid == null)
          Debug.LogError((object) ("grid == null, placeholder : " + placeholder.GetInfo()));
        cellList = grid.Cells;
      }
      intersectAndCheck.IsAllowed = true;
      foreach (Cell cell1 in (IEnumerable<Cell>) cellList)
      {
        Pair<int, int> pair;
        if (storable == null || storable.IsDisposed)
        {
          pair = new Pair<int, int>(cell1.Column, cell1.Row);
        }
        else
        {
          Vector2 vector2 = new Vector2((float) cell1.Column, (float) cell1.Row) + new Vector2((float) cell.Column, (float) cell.Row);
          pair = new Pair<int, int>()
          {
            Item1 = (int) Math.Round((double) vector2.x),
            Item2 = (int) Math.Round((double) vector2.y)
          };
        }
        Cell cell2 = (Cell) null;
        if (container.GetGrid() is InventoryGridInfinited)
        {
          InventoryGridInfinited grid = (InventoryGridInfinited) container.GetGrid();
          if (pair.Item1 >= 0 && (pair.Item1 < grid.Columns || grid.Direction != DirectionKind.Vertical) && pair.Item2 >= 0 && (pair.Item2 < grid.Rows || grid.Direction != DirectionKind.Horizontal))
          {
            cell2 = ProxyFactory.Create<Cell>();
            cell2.Column = pair.Item1;
            cell2.Row = pair.Item2;
          }
        }
        else
        {
          if (!(container.GetGrid() is IInventoryGridLimited))
            throw new Exception();
          cell2 = ((InventoryGridLimited) container.GetGrid())[pair.Item1, pair.Item2];
        }
        if (cell2 != null)
        {
          intersectAndCheck.IsIntersected = true;
          if (storable != null && !storable.IsDisposed && container.GetSlotKind() != SlotKind.None && container.GetSlotKind() != SlotKind.Universal)
          {
            bool flag = false;
            foreach (StorableGroup group in storable.Groups)
            {
              if (container.GetLimitations().Contains<StorableGroup>(group) && !container.GetExcept().Contains<StorableGroup>(group))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Disabled));
              intersectAndCheck.IsAllowed = false;
              continue;
            }
          }
          IStorableComponent storableComponent1 = (IStorableComponent) null;
          foreach (IStorableComponent storableComponent2 in storage.Items)
          {
            if ((storable == null || storable.IsDisposed || storable.Owner != storableComponent2.Owner) && container == storableComponent2.Container)
            {
              if (container.GetKind() == ContainerCellKind.OneCellToOneStorable)
              {
                if (cell2.Column == ((StorableComponent) storableComponent2).Cell.Column && cell2.Row == ((StorableComponent) storableComponent2).Cell.Row)
                  storableComponent1 = storableComponent2;
              }
              else
              {
                foreach (Cell cell3 in (IEnumerable<Cell>) ((InventoryGridLimited) ((StorableComponent) storableComponent2).Placeholder.Grid).Cells)
                {
                  Vector2 vector2 = new Vector2((float) cell3.Column, (float) cell3.Row) + new Vector2((float) ((StorableComponent) storableComponent2).Cell.Column, (float) ((StorableComponent) storableComponent2).Cell.Row);
                  if (cell2.Column == Mathf.RoundToInt(vector2.x) && cell2.Row == Mathf.RoundToInt(vector2.y))
                  {
                    storableComponent1 = storableComponent2;
                    break;
                  }
                }
              }
              if (storableComponent1 != null && !storableComponent1.IsDisposed)
                break;
            }
          }
          if (storableComponent1 == null || storableComponent1.IsDisposed)
          {
            intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Allowed));
          }
          else
          {
            if (storable != null && !storable.IsDisposed && storable.Owner.TemplateId == storableComponent1.Owner.TemplateId && storableComponent1.Max - storableComponent1.Count >= storable.Count)
            {
              intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Stack));
            }
            else
            {
              intersectAndCheck.IsAllowed = false;
              intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Occupied));
            }
            intersectAndCheck.Storables.Add((StorableComponent) storableComponent1);
          }
        }
      }
      switch (intersectAndCheck.Storables.Count)
      {
        case 0:
          if (container.GetKind() != ContainerCellKind.OneCellToOneStorable && intersectAndCheck.Cells.Count != cellList.Count)
          {
            intersectAndCheck.IsAllowed = false;
            break;
          }
          break;
        case 1:
          StorableComponent storableComponent = intersectAndCheck.Storables.FirstOrDefault<StorableComponent>();
          int num = storable == null || storable.IsDisposed ? 0 : (storable.Owner == storableComponent.Owner ? 1 : (!(storable.Owner.TemplateId == storableComponent.Owner.TemplateId) ? 0 : (storableComponent.Max - storableComponent.Count >= storable.Count ? 1 : 0)));
          intersectAndCheck.IsAllowed = num != 0;
          break;
        default:
          intersectAndCheck.IsAllowed = false;
          break;
      }
      return intersectAndCheck;
    }

    private static Intersect GetIntersectFindContainer(
      IStorageComponent storage,
      StorableComponent storable)
    {
      StorageUtility.tmpContainers.Clear();
      StorageUtility.tmpContainers.AddRange(storage.Containers);
      StorageUtility.tmpContainers.Shuffle<IInventoryComponent>();
      foreach (IStorableComponent storableComponent in storage.Items)
      {
        if (storableComponent != null && storableComponent.Owner != null && storable != null && storable.Owner != null && storableComponent.Owner.TemplateId == storable.Owner.TemplateId)
        {
          storable.Max = storableComponent.Max;
          if (storable.Max > 1)
          {
            int a = storableComponent.Max - storableComponent.Count;
            if (a > 0)
            {
              int num = Mathf.Min(a, storable.Count);
              storableComponent.Count += num;
              storable.Count -= num;
            }
          }
          else
            break;
        }
      }
      if (storable == null || storable.IsDisposed)
        return new Intersect();
      if (storable.Count == 0)
      {
        storable.Owner.Dispose();
        return new Intersect();
      }
      foreach (IInventoryComponent tmpContainer in StorageUtility.tmpContainers)
      {
        if ((!tmpContainer.GetLimitations().Any<StorableGroup>() || tmpContainer.GetLimitations().Intersect<StorableGroup>(storable.Groups).Any<StorableGroup>()) && !tmpContainer.GetExcept().Intersect<StorableGroup>(storable.Groups).Any<StorableGroup>() && tmpContainer.Enabled.Value)
        {
          Intersect intersectFindCell = StorageUtility.GetIntersectFindCell(storage, tmpContainer, storable);
          if (intersectFindCell.IsAllowed)
          {
            StorageUtility.tmpContainers.Clear();
            return intersectFindCell;
          }
        }
      }
      StorageUtility.tmpContainers.Clear();
      return new Intersect();
    }

    private static Intersect GetIntersectFindCell(
      IStorageComponent storage,
      IInventoryComponent container,
      StorableComponent storable)
    {
      if (container.GetGrid() == null)
        return new Intersect();
      StorageComponent storage1 = (StorageComponent) storage;
      foreach (IStorableComponent storableComponent in storage1.Items)
      {
        if (storable.Owner.TemplateId == storableComponent.Owner.TemplateId && storableComponent.Container.Owner.Id == container.Owner.Id && storableComponent.Max - storableComponent.Count >= storable.Count)
          return StorageUtility.GetIntersectAndCheck(storage, container, storable, ((StorableComponent) storableComponent).Cell.To());
      }
      Vector2 size = StorageUtility.CalculateSize((IStorageComponent) storage1, container);
      int num1 = Mathf.RoundToInt(size.x);
      int num2 = Mathf.RoundToInt(size.y);
      StorageUtility.tmpCells.Clear();
      for (int index1 = 0; index1 < num2; ++index1)
      {
        for (int index2 = 0; index2 < num1; ++index2)
          StorageUtility.tmpCells.Add(new IntCell()
          {
            Column = index2,
            Row = index1
          });
      }
      foreach (IntCell tmpCell in StorageUtility.tmpCells)
      {
        Intersect intersectAndCheck = StorageUtility.GetIntersectAndCheck(storage, container, storable, tmpCell);
        if (intersectAndCheck.IsAllowed)
        {
          StorageUtility.tmpCells.Clear();
          return intersectAndCheck;
        }
      }
      StorageUtility.tmpCells.Clear();
      return new Intersect();
    }

    private static Vector2 CalculateSize(IStorageComponent storage, IInventoryComponent container)
    {
      if (container.GetGrid() == null || !storage.Containers.Contains<IInventoryComponent>(container))
        return Vector2.zero;
      Vector2 size = new Vector2((float) container.GetGrid().Columns, (float) container.GetGrid().Rows);
      if (container.GetGrid() is IInventoryGridInfinited)
      {
        foreach (IStorableComponent storableComponent in storage.Items)
        {
          if (storableComponent.Container == container)
          {
            Vector3 vector3 = (Vector3) new Vector2((float) ((StorableComponent) storableComponent).Cell.Column, (float) ((StorableComponent) storableComponent).Cell.Row) + new Vector3((float) ((StorableComponent) storableComponent).Placeholder.Grid.Columns, (float) ((StorableComponent) storableComponent).Placeholder.Grid.Rows);
            size.x = Mathf.Max(size.x, (float) Mathf.RoundToInt(vector3.x));
            size.y = Mathf.Max(size.y, (float) Mathf.RoundToInt(vector3.y));
          }
        }
      }
      return size;
    }

    public static IInventoryComponent GetContainerByTemplate(
      IStorageComponent storage,
      IEntity template)
    {
      return storage.Containers.FirstOrDefault<IInventoryComponent>((Func<IInventoryComponent, bool>) (o =>
      {
        if (o.Owner.TemplateId == template.Id)
          return true;
        return o.Owner.Template != null && o.Owner.Template.TemplateId == template.Id;
      }));
    }

    public static int GetItemAmount(IEnumerable<IStorableComponent> items, IEntity resource)
    {
      Guid itemId = StorageUtility.GetItemId(resource);
      int itemAmount = 0;
      foreach (IStorableComponent storableComponent in items)
      {
        if (StorageUtility.GetItemId(storableComponent.Owner) == itemId)
          itemAmount += storableComponent.Count;
      }
      return itemAmount;
    }

    public static Guid GetItemId(IEntity item) => item.IsTemplate ? item.Id : item.TemplateId;
  }
}
