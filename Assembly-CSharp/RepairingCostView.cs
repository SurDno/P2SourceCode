using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
using Engine.Source.Components.Repairing;
using Engine.Source.Components.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class RepairingCostView : MonoBehaviour {
	[SerializeField] private List<ItemView> itemViews = new();
	private IStorageComponent actor;
	private List<RepairableCostItem> cost;

	public IStorageComponent Actor {
		get => actor;
		set {
			if (actor == value)
				return;
			if (actor != null) {
				actor.OnAddItemEvent -= OnActorContentChange;
				actor.OnChangeItemEvent -= OnActorContentChange;
				actor.OnRemoveItemEvent -= OnActorContentChange;
			}

			actor = value;
			if (actor != null) {
				actor.OnAddItemEvent += OnActorContentChange;
				actor.OnChangeItemEvent += OnActorContentChange;
				actor.OnRemoveItemEvent += OnActorContentChange;
			}

			Rebuild();
		}
	}

	public List<RepairableCostItem> Cost {
		get => cost;
		set {
			if (cost == value)
				return;
			cost = value;
			Rebuild();
		}
	}

	private void OnActorContentChange(IStorableComponent item, IInventoryComponent container) {
		Rebuild();
	}

	private void Rebuild() {
		if (Actor == null || Cost == null)
			PrepareItemViews(0);
		else {
			PrepareItemViews(Cost.Count);
			for (var index = 0; index < Cost.Count && index < itemViews.Count; ++index) {
				var resource = Cost[index].Template.Value;
				if (resource == null)
					itemViews[index].gameObject.SetActive(false);
				else {
					itemViews[index].Storable = resource.GetComponent<StorableComponent>();
					var componentInChildren = itemViews[index].GetComponentInChildren<Text>();
					if (componentInChildren != null) {
						var count = Cost[index].Count;
						var itemAmount = StorageUtility.GetItemAmount(Actor.Items, resource);
						componentInChildren.text = Math.Min(count, itemAmount) + " / " + count;
					}
				}
			}
		}
	}

	private void PrepareItemViews(int count) {
		if (itemViews.Count == 0 || itemViews[0] == null)
			return;
		while (itemViews.Count < count) {
			var itemView1 = itemViews[0];
			var itemView2 = Instantiate(itemView1);
			itemView2.transform.SetParent(itemView1.transform.parent, false);
			itemViews.Add(itemView2);
		}

		for (var index = 0; index < itemViews.Count; ++index) {
			itemViews[index].gameObject.SetActive(index < count);
			itemViews[index].Storable = null;
		}
	}
}