using System;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container;

[DisallowMultipleComponent]
public class LimitedInventoryContainerUI : InventoryContainerUI {
	public static LimitedInventoryContainerUI Instantiate(
		IInventoryComponent container,
		InventoryCellStyle style,
		GameObject prefab) {
		var gameObject = Instantiate(prefab);
		gameObject.name = "[Container] " + container.Owner.Name;
		var component1 = gameObject.GetComponent<LimitedInventoryContainerUI>();
		component1.content = component1;
		component1.InventoryContainer = container;
		var gridPosition = InventoryUtility.CalculateGridPosition(container.GetPosition().To(), style);
		component1.Transform.localPosition = gridPosition;
		component1.Transform.pivot = container.GetPivot().To();
		component1.Transform.anchorMax = container.GetAnchor().To();
		component1.Transform.anchorMin = container.GetAnchor().To();
		if (container.GetGrid() is IInventoryGridLimited) {
			var grid = (IInventoryGridLimited)container.GetGrid();
			foreach (var cell in ((InventoryGridLimited)grid).Cells) {
				var inventoryCellUi = InventoryCellUI.Instantiate(cell, style);
				inventoryCellUi.Transform.SetParent(component1.grid.transform, false);
				component1.cells.Add(cell, inventoryCellUi);
			}

			var outerSize = InventoryUtility.CalculateOuterSize(grid, style);
			component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, outerSize.x);
			component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, outerSize.y);
			if (style.OutlinePrefab != null) {
				var component2 = Instantiate(style.OutlinePrefab).GetComponent<RectTransform>();
				component2.sizeDelta = new Vector2(outerSize.x + style.OutlineOffset.x * 2f,
					outerSize.y + style.OutlineOffset.y * 2f);
				component2.SetParent(component1.grid.transform, false);
			}
		}

		if (component1.button != null) {
			component1.button.OpenBeginEvent += component1.FireOpenBegin;
			component1.button.OpenEndEvent += component1.FireOpenEnd;
		}

		return component1;
	}
}