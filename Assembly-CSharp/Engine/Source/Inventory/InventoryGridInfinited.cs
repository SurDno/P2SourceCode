using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Inventory
{
  [Factory(typeof (IInventoryGridInfinited))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class InventoryGridInfinited : 
    EngineObject,
    IInventoryGridInfinited,
    IInventoryGridBase,
    IObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected int columns;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected int rows;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected DirectionKind direction = DirectionKind.Vertical;

    public int Columns
    {
      get => columns;
      set => columns = value;
    }

    public int Rows
    {
      get => rows;
      set => rows = value;
    }

    public DirectionKind Direction => direction;
  }
}
