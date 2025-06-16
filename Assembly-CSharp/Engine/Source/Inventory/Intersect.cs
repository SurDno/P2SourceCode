using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;

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
      IsIntersected = false;
      IsAllowed = isAllowed;
      Storage = null;
      Container = null;
      Storable = null;
      Cell = new IntCell();
      Cells = new List<CellInfo>();
      Storables = new HashSet<StorableComponent>();
    }
  }
}
