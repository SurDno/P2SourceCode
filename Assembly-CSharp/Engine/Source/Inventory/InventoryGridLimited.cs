using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Inventory
{
  [Factory(typeof (IInventoryGridLimited))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class InventoryGridLimited : 
    EngineObject,
    IInventoryGridLimited,
    IInventoryGridBase,
    IObject,
    IFactoryProduct
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<Cell> cells = new List<Cell>();
    private Cell[,] table = new Cell[0, 0];

    public IList<Cell> Cells => cells;

    public int Rows
    {
      get => table.GetLength(1);
      set => throw new Exception();
    }

    public int Columns
    {
      get => table.GetLength(0);
      set => throw new Exception();
    }

    public Cell this[int column, int row]
    {
      get
      {
        return column < 0 || row < 0 || column >= Columns || row >= Rows ? null : table[column, row];
      }
    }

    public Cell this[int index]
    {
      get => cells[index];
      set => cells[index] = value;
    }

    public virtual int Count => cells.Count;

    public virtual void Add(Cell cell)
    {
      int length1 = Math.Max(cell.Column + 1, Columns);
      int length2 = Math.Max(cell.Row + 1, Rows);
      if (length1 >= Columns || length2 >= Rows)
      {
        Cell[,] cellArray = new Cell[length1, length2];
        for (int index1 = 0; index1 < table.GetLength(0); ++index1)
        {
          for (int index2 = 0; index2 < table.GetLength(1); ++index2)
          {
            cellArray[index1, index2] = table[index1, index2];
            table[index1, index2] = null;
          }
        }
        table = cellArray;
      }
      else if (table[cell.Column, cell.Row] != null)
        throw new Exception("Error: Collection already have Cell at " + cell.Column + ";" + cell.Row + " !");
      cells.Add(cell);
      table[cell.Column, cell.Row] = cell;
    }

    public virtual void Clear()
    {
      foreach (Cell cell in cells)
        Remove(cell);
      cells.Clear();
    }

    public virtual bool Remove(Cell cell)
    {
      table[cell.Column, cell.Row] = null;
      return cells.Remove(cell);
    }

    private void Trim()
    {
      int val1_1 = -1;
      int val1_2 = -1;
      for (int index = cells.Count - 1; index >= 0; --index)
      {
        if (cells[index] == null)
        {
          cells.RemoveAt(index);
        }
        else
        {
          val1_1 = Math.Max(val1_1, cells[index].Column);
          val1_2 = Math.Max(val1_2, cells[index].Row);
        }
      }
      if (val1_1 < 0 || val1_2 < 0)
      {
        table = new Cell[0, 0];
      }
      else
      {
        int length = val1_2 + 1;
        table = new Cell[val1_1 + 1, length];
        for (int index = 0; index < cells.Count; ++index)
        {
          Cell cell = cells[index];
          table[cell.Column, cell.Row] = cell;
        }
      }
    }

    public void ConstructComplete() => Trim();
  }
}
