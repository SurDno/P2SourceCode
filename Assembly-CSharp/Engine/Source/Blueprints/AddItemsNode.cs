using System;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints;

[Category("Engine")]
public class AddItemsNode : FlowControlNode {
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
					var num = amount.value;
					for (var index = 0; index < num; ++index) {
						var service1 = ServiceLocator.GetService<IFactory>();
						var ientitySerializable = addItem.value;
						var template = ientitySerializable.Value;
						var entity = service1.Instantiate(template);
						ServiceLocator.GetService<ISimulation>()
							.Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
						var component2 = entity.GetComponent<StorableComponent>();
						if (StorageUtility
						    .GetIntersect(component1, null, entity.GetComponent<StorableComponent>(), null).IsAllowed) {
							component1.AddItem(component2, null);
							var service2 = ServiceLocator.GetService<NotificationService>();
							var objArray = new object[1];
							ientitySerializable = addItem.value;
							objArray[0] = ientitySerializable.Value;
							service2.AddNotify(NotificationEnum.ItemRecieve, objArray);
						} else
							ServiceLocator.GetService<DropBagService>().DropBag(component2, component1.Owner);
					}
				}
			}

			output.Call();
		});
		addItem = AddValueInput<IEntitySerializable>("Add Item");
		amount = AddValueInput<int>("Amount");
	}

	private Guid GetItemId(IEntity item) {
		return item.IsTemplate ? item.Id : item.TemplateId;
	}
}