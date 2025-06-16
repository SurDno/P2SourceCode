using System.Collections.Generic;
using Engine.Behaviours.Localization;
using Engine.Common.Components;
using Engine.Impl.UI;
using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Container;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Components;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using UnityEngine;
using UnityEngine.UI;

public class ItemsSlidingContainer : UIControl {
	[SerializeField] private GameObject inventoryContainerPrefab;
	[SerializeField] private GameObject groupTitlePrefab;
	[SerializeField] private InventoryCellStyle cellStyle;
	[SerializeField] private RectTransform ContentArea;
	[SerializeField] private RectTransform Mask;
	[SerializeField] private GameObject inventoryStorablePrefab;
	[SerializeField] private ScrollRect scrollRect;
	private List<StorableUI> items = new();
	private List<GameObject> itemGroupTitles = new();
	private List<InventoryContainerUI> containerViews = new();

	public void Clear(
		Dictionary<InventoryContainerUI, IStorageComponent> containers,
		Dictionary<IStorableComponent, StorableUI> storables) {
		foreach (Object itemGroupTitle in itemGroupTitles)
			DestroyImmediate(itemGroupTitle);
		itemGroupTitles.Clear();
		foreach (var containerView in containerViews) {
			if (containerView is ComplexInventoryContainerUI)
				foreach (var key in (containerView as ComplexInventoryContainerUI).Items)
					if (key != null)
						storables.Remove(key);
			containers.Remove(containerView);
			DestroyImmediate(containerView.gameObject);
		}

		containerViews.Clear();
		scrollRect.verticalNormalizedPosition = 1f;
		items.Clear();
	}

	public List<StorableUI> ItemsUI => items;

	public void CreateSlots(
		List<List<StorableComponent>> itemsList,
		List<string> groupSignatures,
		StorageComponent storage,
		Dictionary<InventoryContainerUI, IStorageComponent> containers,
		Dictionary<IStorableComponent, StorableUI> storables) {
		for (var index = 0; index < itemsList.Count; ++index) {
			var gameObject = Instantiate(groupTitlePrefab, ContentArea, false);
			gameObject.GetComponent<Localizer>().Signature = groupSignatures[index];
			itemGroupTitles.Add(gameObject);
			CreateSlots(itemsList[index], storage, containers, storables);
		}
	}

	public void CreateSlots(
		List<StorableComponent> itemsList,
		StorageComponent storage,
		Dictionary<InventoryContainerUI, IStorageComponent> containers,
		Dictionary<IStorableComponent, StorableUI> storables) {
		var key = ComplexInventoryContainerUI.Instantiate(cellStyle, inventoryContainerPrefab, itemsList, Mask);
		key.transform.SetParent(ContentArea, false);
		containerViews.Add(key);
		var collection = new List<StorableUI>();
		foreach (var items in itemsList) {
			var storableUi = StorableUI.Instantiate(items, inventoryStorablePrefab, cellStyle.imageStyle);
			storableUi.Style = cellStyle;
			var cellUi = key.GetCellUi(items.Cell);
			storableUi.transform.SetParent(cellUi.Transform, false);
			storableUi.transform.localPosition = Vector2.zero;
			storables[items] = storableUi;
			collection.Add(storableUi);
		}

		this.items.AddRange(collection);
		containers.Add(key, storage);
	}

	public void FillForSelected(int index) {
		var height1 = scrollRect.gameObject.GetComponent<RectTransform>().rect.height;
		var height2 = inventoryStorablePrefab.GetComponent<RectTransform>().rect.height;
		var num = (float)((index + 1) * (double)height2 * 2.0);
		var anchoredPosition = ContentArea.anchoredPosition;
		if (num - (double)anchoredPosition.y > height1)
			anchoredPosition.y = num + (double)anchoredPosition.y > height1 ? num - height1 : 0.0f;
		else if (num - height2 * 2.0 - anchoredPosition.y < 0.0)
			anchoredPosition.y = num - height2 * 2f;
		ContentArea.anchoredPosition = anchoredPosition;
	}

	public void ScrollTo(int index, int storableCount) {
		scrollRect.verticalNormalizedPosition = (float)(1.0 - index / (double)(storableCount - 1));
	}
}