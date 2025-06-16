using Inspectors;

namespace Engine.Source.Inventory
{
  public struct CellInfo
  {
    [Inspected]
    public IntCell Cell;
    [Inspected]
    public CellState State;

    public CellInfo(IntCell cell, CellState state)
    {
      this.Cell = cell;
      this.State = state;
    }
  }
}
