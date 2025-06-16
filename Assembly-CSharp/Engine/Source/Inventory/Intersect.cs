using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Inventory
{
  public struct Intersect
  {
    [Inspected]
    public bool IsIntersected;
    [Inspected]
    public bool IsAllowed;
    [Inspected]
    public IStorageComponent Storage;
    [Inspected]
    public IInventoryComponent Container;
    [Inspected]
    public StorableComponent Storable;
    [Inspected]
    public IntCell Cell;
    [Inspected]
    public List<CellInfo> Cells;
    [Inspected]
    public HashSet<StorableComponent> Storables;

    public Intersect(bool isAllowed = false)
    {
      this.IsIntersected = false;
      this.IsAllowed = isAllowed;
      this.Storage = (IStorageComponent) null;
      this.Container = (IInventoryComponent) null;
      this.Storable = (StorableComponent) null;
      this.Cell = new IntCell();
      this.Cells = new List<CellInfo>();
      this.Storables = new HashSet<StorableComponent>();
    }
  }
}
