using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class SwapItemsNode : FlowControlNode {
	private ValueInput<IEntitySerializable> removeItem;
	private ValueInput<IEntitySerializable> addItem;
	private ValueInput<int> amount;

	protected override void RegisterPorts() {
		base.RegisterPorts();
		var output = AddFlowOutput("Out");
		AddFlowInput("In", () => {
			var player = ServiceLocator.GetService<ISimulation>().Player;
			if (player != null) {
				var component1 = player.GetComponent<StorageComponent>();
				if (component1 != null) {
					var storage = component1;
					var ientitySerializable = removeItem.value;
					var entity1 = ientitySerializable.Value;
					var amount = this.amount.value;
					var num = RemoveItemsAmount(storage, entity1, amount);
					for (var index = 0; index < num; ++index) {
						var service1 = ServiceLocator.GetService<IFactory>();
						ientitySerializable = addItem.value;
						var template = ientitySerializable.Value;
						var entity2 = service1.Instantiate(template);
						ServiceLocator.GetService<ISimulation>()
							.Add(entity2, ServiceLocator.GetService<ISimulation>().Storables);
						var component2 = entity2.GetComponent<StorableComponent>();
						var intersect = StorageUtility.GetIntersect(component1, null, component2, null);
						if (entity2.IsDisposed) {
							var service2 = ServiceLocator.GetService<NotificationService>();
							var objArray = new object[1];
							ientitySerializable = addItem.value;
							objArray[0] = ientitySerializable.Value;
							service2.AddNotify(NotificationEnum.ItemRecieve, objArray);
						} else if (intersect.IsAllowed) {
							component1.AddItem(component2, null);
							var service3 = ServiceLocator.GetService<NotificationService>();
							var objArray = new object[1];
							ientitySerializable = addItem.value;
							objArray[0] = ientitySerializable.Value;
							service3.AddNotify(NotificationEnum.ItemRecieve, objArray);
						} else
							ServiceLocator.GetService<DropBagService>().DropBag(component2, component1.Owner);
					}
				}
			}

			output.Call();
		});
		removeItem = AddValueInput<IEntitySerializable>("Remove Item");
		addItem = AddValueInput<IEntitySerializable>("Add Item");
		this.amount = AddValueInput<int>("Amount");
	}

	private int RemoveItemsAmount(IStorageComponent storage, IEntity item, int amount) {
		var num = amount;
		var keyValuePairList = new List<KeyValuePair<IStorableComponent, int>>();
		var storableComponentList = new List<IStorableComponent>();
		foreach (var storableComponent in storage.Items)
			if (GetItemId(storableComponent.Owner) == GetItemId(item))
				storableComponentList.Add(storableComponent);
		storableComponentList.Sort((a, b) => a.Count.CompareTo(b.Count));
		foreach (var key in storableComponentList) {
			num -= Mathf.Min(key.Count, amount);
			keyValuePairList.Add(
				new KeyValuePair<IStorableComponent, int>(key, key.Count - Mathf.Min(key.Count, amount)));
			if (num <= 0)
				break;
		}

		foreach (var keyValuePair in keyValuePairList) {
			var k = keyValuePair;
			var storableComponent = storage.Items.First(x => x.Equals(k.Key));
			storableComponent.Count = k.Value;
			if (storableComponent.Count <= 0)
				storableComponent.Owner.Dispose();
		}

		return amount - num;
	}

	private Guid GetItemId(IEntity item) {
		return item.IsTemplate ? item.Id : item.TemplateId;
	}
}