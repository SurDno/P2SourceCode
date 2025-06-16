using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Types;
using Engine.Source.Inventory;
using UnityEngine;

namespace Engine.Source.Components.Utilities;

public static class StorageUtility {
	private static List<IInventoryComponent> tmpContainers = new();
	private static List<IntCell> tmpCells = new();

	public static Intersect GetIntersect(
		IStorageComponent storage,
		IInventoryComponent container,
		StorableComponent storable,
		Cell cell) {
		if (container == null)
			return GetIntersectFindContainer(storage, storable);
		return cell == null
			? GetIntersectFindCell(storage, container, storable)
			: GetIntersectAndCheck(storage, container, storable, cell.To());
	}

	private static Intersect GetIntersectAndCheck(
		IStorageComponent storage,
		IInventoryComponent container,
		StorableComponent storable,
		IntCell cell) {
		var intersectAndCheck = new Intersect();
		if (container == null) {
			Debug.LogError("stored.Container == null , owner : " + storage.Owner.GetInfo());
			return intersectAndCheck;
		}

		if (storage == null || storage.IsDisposed) {
			Debug.LogError("stored.Storage == null || stored.Storage.IsDisposed() , owner : " +
			               storage.Owner.GetInfo());
			return intersectAndCheck;
		}

		intersectAndCheck.Storage = storage;
		intersectAndCheck.Container = container;
		intersectAndCheck.Storable = storable;
		intersectAndCheck.Cell = cell;
		IList<Cell> cellList;
		if (storable == null || storable.IsDisposed)
			cellList = new List<Cell> {
				cell.To()
			};
		else if (container.GetKind() == ContainerCellKind.OneCellToOneStorable) {
			if (storable == null) {
				Debug.LogError("stored.Storable == null , storage owner : " + storage.Owner.GetInfo());
				return new Intersect();
			}

			if (storable.Placeholder == null) {
				Debug.LogError("stored.Storable.Placeholder == null , owner : " + storable.Owner.GetInfo() +
				               " , storage owner : " + storage.Owner.GetInfo());
				return new Intersect();
			}

			cellList = new List<Cell> {
				((InventoryGridLimited)storable.Placeholder.Grid)[0, 0]
			};
		} else {
			if (storable == null) {
				Debug.LogError("stored.Storable == null , storage owner : " + storage.Owner.GetInfo());
				return new Intersect();
			}

			if (storable.Placeholder == null) {
				Debug.LogError("stored.Storable.Placeholder == null , owner : " + storable.Owner.GetInfo() +
				               " , storage owner : " + storage.Owner.GetInfo());
				return new Intersect();
			}

			var placeholder = storable.Placeholder;
			var grid = (InventoryGridLimited)placeholder.Grid;
			if (grid == null)
				Debug.LogError("grid == null, placeholder : " + placeholder.GetInfo());
			cellList = grid.Cells;
		}

		intersectAndCheck.IsAllowed = true;
		foreach (var cell1 in cellList) {
			Pair<int, int> pair;
			if (storable == null || storable.IsDisposed)
				pair = new Pair<int, int>(cell1.Column, cell1.Row);
			else {
				var vector2 = new Vector2(cell1.Column, cell1.Row) + new Vector2(cell.Column, cell.Row);
				pair = new Pair<int, int> {
					Item1 = (int)Math.Round(vector2.x),
					Item2 = (int)Math.Round(vector2.y)
				};
			}

			Cell cell2 = null;
			if (container.GetGrid() is InventoryGridInfinited) {
				var grid = (InventoryGridInfinited)container.GetGrid();
				if (pair.Item1 >= 0 && (pair.Item1 < grid.Columns || grid.Direction != DirectionKind.Vertical) &&
				    pair.Item2 >= 0 && (pair.Item2 < grid.Rows || grid.Direction != DirectionKind.Horizontal)) {
					cell2 = ProxyFactory.Create<Cell>();
					cell2.Column = pair.Item1;
					cell2.Row = pair.Item2;
				}
			} else {
				if (!(container.GetGrid() is IInventoryGridLimited))
					throw new Exception();
				cell2 = ((InventoryGridLimited)container.GetGrid())[pair.Item1, pair.Item2];
			}

			if (cell2 != null) {
				intersectAndCheck.IsIntersected = true;
				if (storable != null && !storable.IsDisposed && container.GetSlotKind() != SlotKind.None &&
				    container.GetSlotKind() != SlotKind.Universal) {
					var flag = false;
					foreach (var group in storable.Groups)
						if (container.GetLimitations().Contains(group) && !container.GetExcept().Contains(group)) {
							flag = true;
							break;
						}

					if (!flag) {
						intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Disabled));
						intersectAndCheck.IsAllowed = false;
						continue;
					}
				}

				IStorableComponent storableComponent1 = null;
				foreach (var storableComponent2 in storage.Items)
					if ((storable == null || storable.IsDisposed || storable.Owner != storableComponent2.Owner) &&
					    container == storableComponent2.Container) {
						if (container.GetKind() == ContainerCellKind.OneCellToOneStorable) {
							if (cell2.Column == ((StorableComponent)storableComponent2).Cell.Column &&
							    cell2.Row == ((StorableComponent)storableComponent2).Cell.Row)
								storableComponent1 = storableComponent2;
						} else
							foreach (var cell3 in ((InventoryGridLimited)((StorableComponent)storableComponent2)
								         .Placeholder.Grid).Cells) {
								var vector2 = new Vector2(cell3.Column, cell3.Row) +
								              new Vector2(((StorableComponent)storableComponent2).Cell.Column,
									              ((StorableComponent)storableComponent2).Cell.Row);
								if (cell2.Column == Mathf.RoundToInt(vector2.x) &&
								    cell2.Row == Mathf.RoundToInt(vector2.y)) {
									storableComponent1 = storableComponent2;
									break;
								}
							}

						if (storableComponent1 != null && !storableComponent1.IsDisposed)
							break;
					}

				if (storableComponent1 == null || storableComponent1.IsDisposed)
					intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Allowed));
				else {
					if (storable != null && !storable.IsDisposed &&
					    storable.Owner.TemplateId == storableComponent1.Owner.TemplateId &&
					    storableComponent1.Max - storableComponent1.Count >= storable.Count)
						intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Stack));
					else {
						intersectAndCheck.IsAllowed = false;
						intersectAndCheck.Cells.Add(new CellInfo(cell2.To(), CellState.Occupied));
					}

					intersectAndCheck.Storables.Add((StorableComponent)storableComponent1);
				}
			}
		}

		switch (intersectAndCheck.Storables.Count) {
			case 0:
				if (container.GetKind() != ContainerCellKind.OneCellToOneStorable &&
				    intersectAndCheck.Cells.Count != cellList.Count) intersectAndCheck.IsAllowed = false;
				break;
			case 1:
				var storableComponent = intersectAndCheck.Storables.FirstOrDefault();
				var num = storable == null || storable.IsDisposed ? 0 :
					storable.Owner == storableComponent.Owner ? 1 :
					!(storable.Owner.TemplateId == storableComponent.Owner.TemplateId) ? 0 :
					storableComponent.Max - storableComponent.Count >= storable.Count ? 1 : 0;
				intersectAndCheck.IsAllowed = num != 0;
				break;
			default:
				intersectAndCheck.IsAllowed = false;
				break;
		}

		return intersectAndCheck;
	}

	private static Intersect GetIntersectFindContainer(
		IStorageComponent storage,
		StorableComponent storable) {
		tmpContainers.Clear();
		tmpContainers.AddRange(storage.Containers);
		tmpContainers.Shuffle();
		foreach (var storableComponent in storage.Items)
			if (storableComponent != null && storableComponent.Owner != null && storable != null &&
			    storable.Owner != null && storableComponent.Owner.TemplateId == storable.Owner.TemplateId) {
				storable.Max = storableComponent.Max;
				if (storable.Max > 1) {
					var a = storableComponent.Max - storableComponent.Count;
					if (a > 0) {
						var num = Mathf.Min(a, storable.Count);
						storableComponent.Count += num;
						storable.Count -= num;
					}
				} else
					break;
			}

		if (storable == null || storable.IsDisposed)
			return new Intersect();
		if (storable.Count == 0) {
			storable.Owner.Dispose();
			return new Intersect();
		}

		foreach (var tmpContainer in tmpContainers)
			if ((!tmpContainer.GetLimitations().Any() ||
			     tmpContainer.GetLimitations().Intersect(storable.Groups).Any()) &&
			    !tmpContainer.GetExcept().Intersect(storable.Groups).Any() && tmpContainer.Enabled.Value) {
				var intersectFindCell = GetIntersectFindCell(storage, tmpContainer, storable);
				if (intersectFindCell.IsAllowed) {
					tmpContainers.Clear();
					return intersectFindCell;
				}
			}

		tmpContainers.Clear();
		return new Intersect();
	}

	private static Intersect GetIntersectFindCell(
		IStorageComponent storage,
		IInventoryComponent container,
		StorableComponent storable) {
		if (container.GetGrid() == null)
			return new Intersect();
		var storage1 = (StorageComponent)storage;
		foreach (var storableComponent in storage1.Items)
			if (storable.Owner.TemplateId == storableComponent.Owner.TemplateId &&
			    storableComponent.Container.Owner.Id == container.Owner.Id &&
			    storableComponent.Max - storableComponent.Count >= storable.Count)
				return GetIntersectAndCheck(storage, container, storable,
					((StorableComponent)storableComponent).Cell.To());
		var size = CalculateSize(storage1, container);
		var num1 = Mathf.RoundToInt(size.x);
		var num2 = Mathf.RoundToInt(size.y);
		tmpCells.Clear();
		for (var index1 = 0; index1 < num2; ++index1) {
			for (var index2 = 0; index2 < num1; ++index2)
				tmpCells.Add(new IntCell {
					Column = index2,
					Row = index1
				});
		}

		foreach (var tmpCell in tmpCells) {
			var intersectAndCheck = GetIntersectAndCheck(storage, container, storable, tmpCell);
			if (intersectAndCheck.IsAllowed) {
				tmpCells.Clear();
				return intersectAndCheck;
			}
		}

		tmpCells.Clear();
		return new Intersect();
	}

	private static Vector2 CalculateSize(IStorageComponent storage, IInventoryComponent container) {
		if (container.GetGrid() == null || !storage.Containers.Contains(container))
			return Vector2.zero;
		var size = new Vector2(container.GetGrid().Columns, container.GetGrid().Rows);
		if (container.GetGrid() is IInventoryGridInfinited)
			foreach (var storableComponent in storage.Items)
				if (storableComponent.Container == container) {
					var vector3 =
						(Vector3)new Vector2(((StorableComponent)storableComponent).Cell.Column,
							((StorableComponent)storableComponent).Cell.Row) + new Vector3(
							((StorableComponent)storableComponent).Placeholder.Grid.Columns,
							((StorableComponent)storableComponent).Placeholder.Grid.Rows);
					size.x = Mathf.Max(size.x, Mathf.RoundToInt(vector3.x));
					size.y = Mathf.Max(size.y, Mathf.RoundToInt(vector3.y));
				}

		return size;
	}

	public static IInventoryComponent GetContainerByTemplate(
		IStorageComponent storage,
		IEntity template) {
		return storage.Containers.FirstOrDefault(o => {
			if (o.Owner.TemplateId == template.Id)
				return true;
			return o.Owner.Template != null && o.Owner.Template.TemplateId == template.Id;
		});
	}

	public static int GetItemAmount(IEnumerable<IStorableComponent> items, IEntity resource) {
		var itemId = GetItemId(resource);
		var itemAmount = 0;
		foreach (var storableComponent in items)
			if (GetItemId(storableComponent.Owner) == itemId)
				itemAmount += storableComponent.Count;
		return itemAmount;
	}

	public static Guid GetItemId(IEntity item) {
		return item.IsTemplate ? item.Id : item.TemplateId;
	}
}