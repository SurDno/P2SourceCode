using Cofe.Proxies;

namespace Engine.Source.Inventory;

public static class CellUtility {
	public static IntCell To(this Cell cell) {
		return new IntCell {
			Column = cell.Column,
			Row = cell.Row
		};
	}

	public static Cell To(this IntCell vector) {
		var cell = ProxyFactory.Create<Cell>();
		cell.Column = vector.Column;
		cell.Row = vector.Row;
		return cell;
	}
}