using Inspectors;

namespace Engine.Source.Inventory;

public struct CellInfo {
	[Inspected] public IntCell Cell;
	[Inspected] public CellState State;

	public CellInfo(IntCell cell, CellState state) {
		Cell = cell;
		State = state;
	}
}