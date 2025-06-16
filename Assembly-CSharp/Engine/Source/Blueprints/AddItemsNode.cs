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

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class AddItemsNode : FlowControlNode
  {
    private ValueInput<IEntitySerializable> addItem;
    private ValueInput<int> amount;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player != null)
        {
          StorageComponent component1 = player.GetComponent<StorageComponent>();
          if (component1 != null)
          {
            int num = amount.value;
            for (int index = 0; index < num; ++index)
            {
              IFactory service1 = ServiceLocator.GetService<IFactory>();
              IEntitySerializable ientitySerializable = addItem.value;
              IEntity template = ientitySerializable.Value;
              IEntity entity = service1.Instantiate(template);
              ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
              StorableComponent component2 = entity.GetComponent<StorableComponent>();
              if (StorageUtility.GetIntersect(component1, null, entity.GetComponent<StorableComponent>(), null).IsAllowed)
              {
                component1.AddItem(component2, null);
                NotificationService service2 = ServiceLocator.GetService<NotificationService>();
                object[] objArray = new object[1];
                ientitySerializable = addItem.value;
                objArray[0] = ientitySerializable.Value;
                service2.AddNotify(NotificationEnum.ItemRecieve, objArray);
              }
              else
                ServiceLocator.GetService<DropBagService>().DropBag(component2, component1.Owner);
            }
          }
        }
        output.Call();
      });
      addItem = AddValueInput<IEntitySerializable>("Add Item");
      amount = AddValueInput<int>("Amount");
    }

    private Guid GetItemId(IEntity item) => item.IsTemplate ? item.Id : item.TemplateId;
  }
}
