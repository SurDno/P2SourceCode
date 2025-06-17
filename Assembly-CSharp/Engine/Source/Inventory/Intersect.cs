using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Inventory
{
  public struct Intersect(bool isAllowed = false) {
    [Inspected]
    public bool IsIntersected = false;
    [Inspected]
    public bool IsAllowed = isAllowed;
    [Inspected]
    public IStorageComponent Storage = null;
    [Inspected]
    public IInventoryComponent Container = null;
    [Inspected]
    public StorableComponent Storable = null;
    [Inspected]
    public IntCell Cell = new();
    [Inspected]
    public List<CellInfo> Cells = [];
    [Inspected]
    public HashSet<StorableComponent> Storables = [];
  }
}
