// Decompiled with JetBrains decompiler
// Type: Engine.Source.Inventory.Intersect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;

#nullable disable
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
