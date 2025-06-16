using System.Collections.Generic;
using System.Linq;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Source.Inventory;

namespace Engine.Source.Components.Utilities;

public static class InventorySorter {
	public static void Sort(IStorageComponent storage) {
		ConcatSameItems(storage);
		MoveItems(storage);
	}

	private static void ConcatSameItems(IStorageComponent storage) {
		var storableComponentList = new List<IStorableComponent>(storage.Items);
		for (var index1 = storableComponentList.Count - 1; index1 > 0; --index1) {
			var storableComponent1 = storableComponentList[index1];
			for (var index2 = index1 - 1; index2 >= 0; --index2) {
				var storableComponent2 = storableComponentList[index2];
				if (storableComponent1 != storableComponent2 && storableComponent1 != null &&
				    storableComponent1.Owner != null && storableComponent2 != null &&
				    storableComponent2.Owner != null &&
				    storableComponent1.Owner.TemplateId == storableComponent2.Owner.TemplateId &&
				    storableComponent2.Count > 0) {
					var num1 = storableComponent1.Max - storableComponent1.Count;
					var num2 = storableComponent2.Count > num1 ? num1 : storableComponent2.Count;
					storableComponent1.Count += num2;
					storableComponent2.Count -= num2;
					if (storableComponent2.Count == 0)
						storableComponent2.Owner.Dispose();
				}
			}
		}
	}

	private static void MoveItems(IStorageComponent storage) {
		var orderedEnumerable = storage.Containers
			.Where(c => c.Enabled.Value && c.GetGroup() == InventoryGroup.Backpack)
			.OrderBy(c => c.GetGrid().Rows * c.GetGrid().Columns);
		var itemsLeft =
			new List<IStorableComponent>(storage.Items.Where(item =>
				item.Container.GetGroup() == InventoryGroup.Backpack)).ConvertAll(item => item as StorableComponent);
		foreach (var inventoryComponent in orderedEnumerable) {
			if (itemsLeft.Count == 0)
				break;
			var greedMatrix = CreateGreedMatrix(inventoryComponent.GetGrid());
			foreach (var storableComponent in GetItemsToFillContainer(inventoryComponent, greedMatrix, itemsLeft))
				itemsLeft.Remove(storableComponent);
		}
	}

	private static List<StorableComponent> GetItemsToFillContainer(
		IInventoryComponent backpack,
		int[][] backpackMatrix,
		List<StorableComponent> itemsLeft) {
		var storableComponents = itemsLeft
			.Where(item => item.Placeholder.Grid.Rows <= backpack.GetGrid().Rows &&
			               item.Placeholder.Grid.Columns <= backpack.GetGrid().Columns).OrderBy(item =>
				item.Placeholder.Grid.Columns * item.Placeholder.Grid.Rows).Reverse();
		var itemsToFillContainer = new List<StorableComponent>();
		foreach (var storableComponent in storableComponents) {
			var cellToPlace = FindCellToPlace(backpackMatrix, storableComponent);
			if (cellToPlace != null) {
				Place(backpack, backpackMatrix, storableComponent, cellToPlace);
				itemsToFillContainer.Add(storableComponent);
			}
		}

		return itemsToFillContainer;
	}

	private static void Place(
		IInventoryComponent backpack,
		int[][] backpackMatrix,
		StorableComponent item,
		Cell cell) {
		item.Container = backpack;
		item.Cell.Column = cell.Column;
		item.Cell.Row = cell.Row;
		var greedMatrix = CreateGreedMatrix(item.Placeholder.Grid);
		var row = cell.Row;
		var column = cell.Column;
		for (var index1 = 0; index1 < greedMatrix.Length; ++index1) {
			for (var index2 = 0; index2 < greedMatrix[index1].Length; ++index2)
				backpackMatrix[row + index1][column + index2] = 1;
		}
	}

	private static Cell FindCellToPlace(int[][] backpackMatrix, StorableComponent item) {
		var greedMatrix = CreateGreedMatrix(item.Placeholder.Grid);
		var iPlace = 0;
		for (var length1 = backpackMatrix.Length; iPlace < length1; ++iPlace) {
			var jPlace = 0;
			for (var length2 = backpackMatrix[iPlace].Length; jPlace < length2; ++jPlace)
				if (CanPlace(backpackMatrix, greedMatrix, iPlace, jPlace))
					return new Cell {
						Row = iPlace,
						Column = jPlace
					};
		}

		return null;
	}

	private static bool CanPlace(int[][] m1, int[][] m2, int iPlace, int jPlace) {
		var flag = false;
		if (m1.Length >= iPlace + m2.Length && m1[0].Length >= jPlace + m2[0].Length) {
			var index1 = iPlace;
			for (var index2 = 0; index1 < m1.Length && index2 < m2.Length; ++index2) {
				var index3 = jPlace;
				for (var index4 = 0; index3 < m1[index1].Length && index4 < m2[index2].Length; ++index4) {
					if (m1[index1][index3] != 0)
						return false;
					flag = true;
					++index3;
				}

				++index1;
			}
		}

		return flag;
	}

	private static int[][] CreateGreedMatrix(IInventoryGridBase greed) {
		var greedMatrix = new int[greed.Rows][];
		for (var index = 0; index < greed.Rows; ++index)
			greedMatrix[index] = new int[greed.Columns];
		return greedMatrix;
	}
}