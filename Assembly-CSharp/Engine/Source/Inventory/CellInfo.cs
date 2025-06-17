using Inspectors;

namespace Engine.Source.Inventory
{
  public struct CellInfo(IntCell cell, CellState state) {
    [Inspected]
    public IntCell Cell = cell;
    [Inspected]
    public CellState State = state;
  }
}
