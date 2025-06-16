using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<Cell> cells = new List<Cell>();
    private Cell[,] table = new Cell[0, 0];

    public IList<Cell> Cells => (IList<Cell>) this.cells;

    public int Rows
    {
      get => this.table.GetLength(1);
      set => throw new Exception();
    }

    public int Columns
    {
      get => this.table.GetLength(0);
      set => throw new Exception();
    }

    public Cell this[int column, int row]
    {
      get
      {
        return column < 0 || row < 0 || column >= this.Columns || row >= this.Rows ? (Cell) null : this.table[column, row];
      }
    }

    public Cell this[int index]
    {
      get => this.cells[index];
      set => this.cells[index] = value;
    }

    public virtual int Count => this.cells.Count;

    public virtual void Add(Cell cell)
    {
      int length1 = Math.Max(cell.Column + 1, this.Columns);
      int length2 = Math.Max(cell.Row + 1, this.Rows);
      if (length1 >= this.Columns || length2 >= this.Rows)
      {
        Cell[,] cellArray = new Cell[length1, length2];
        for (int index1 = 0; index1 < this.table.GetLength(0); ++index1)
        {
          for (int index2 = 0; index2 < this.table.GetLength(1); ++index2)
          {
            cellArray[index1, index2] = this.table[index1, index2];
            this.table[index1, index2] = (Cell) null;
          }
        }
        this.table = cellArray;
      }
      else if (this.table[cell.Column, cell.Row] != null)
        throw new Exception("Error: Collection already have Cell at " + (object) cell.Column + ";" + (object) cell.Row + " !");
      this.cells.Add(cell);
      this.table[cell.Column, cell.Row] = cell;
    }

    public virtual void Clear()
    {
      foreach (Cell cell in this.cells)
        this.Remove(cell);
      this.cells.Clear();
    }

    public virtual bool Remove(Cell cell)
    {
      this.table[cell.Column, cell.Row] = (Cell) null;
      return this.cells.Remove(cell);
    }

    private void Trim()
    {
      int val1_1 = -1;
      int val1_2 = -1;
      for (int index = this.cells.Count - 1; index >= 0; --index)
      {
        if (this.cells[index] == null)
        {
          this.cells.RemoveAt(index);
        }
        else
        {
          val1_1 = Math.Max(val1_1, this.cells[index].Column);
          val1_2 = Math.Max(val1_2, this.cells[index].Row);
        }
      }
      if (val1_1 < 0 || val1_2 < 0)
      {
        this.table = new Cell[0, 0];
      }
      else
      {
        int length = val1_2 + 1;
        this.table = new Cell[val1_1 + 1, length];
        for (int index = 0; index < this.cells.Count; ++index)
        {
          Cell cell = this.cells[index];
          this.table[cell.Column, cell.Row] = cell;
        }
      }
    }

    public void ConstructComplete() => this.Trim();
  }
}
