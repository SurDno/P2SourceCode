using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Inventory;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class Cell {
	[DataReadProxy]
	[DataWriteProxy]
	[StateSaveProxy]
	[StateLoadProxy]
	[CopyableProxy]
	[Inspected(Header = true)]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public int Column;

	[DataReadProxy]
	[DataWriteProxy]
	[StateSaveProxy]
	[StateLoadProxy]
	[CopyableProxy()]
	[Inspected(Header = true)]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public int Row;
}