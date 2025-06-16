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

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SwapItemsNode : FlowControlNode
  {
    private ValueInput<IEntitySerializable> removeItem;
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
            StorageComponent storage = component1;
            IEntitySerializable ientitySerializable = removeItem.value;
            IEntity entity1 = ientitySerializable.Value;
            int amount = this.amount.value;
            int num = RemoveItemsAmount(storage, entity1, amount);
            for (int index = 0; index < num; ++index)
            {
              IFactory service1 = ServiceLocator.GetService<IFactory>();
              ientitySerializable = addItem.value;
              IEntity template = ientitySerializable.Value;
              IEntity entity2 = service1.Instantiate(template);
              ServiceLocator.GetService<ISimulation>().Add(entity2, ServiceLocator.GetService<ISimulation>().Storables);
              StorableComponent component2 = entity2.GetComponent<StorableComponent>();
              Intersect intersect = StorageUtility.GetIntersect(component1, null, component2, null);
              if (entity2.IsDisposed)
              {
                NotificationService service2 = ServiceLocator.GetService<NotificationService>();
                object[] objArray = new object[1];
                ientitySerializable = addItem.value;
                objArray[0] = ientitySerializable.Value;
                service2.AddNotify(NotificationEnum.ItemRecieve, objArray);
              }
              else if (intersect.IsAllowed)
              {
                component1.AddItem(component2, null);
                NotificationService service3 = ServiceLocator.GetService<NotificationService>();
                object[] objArray = new object[1];
                ientitySerializable = addItem.value;
                objArray[0] = ientitySerializable.Value;
                service3.AddNotify(NotificationEnum.ItemRecieve, objArray);
              }
              else
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

    private int RemoveItemsAmount(IStorageComponent storage, IEntity item, int amount)
    {
      int num = amount;
      List<KeyValuePair<IStorableComponent, int>> keyValuePairList = new List<KeyValuePair<IStorableComponent, int>>();
      List<IStorableComponent> storableComponentList = new List<IStorableComponent>();
      foreach (IStorableComponent storableComponent in storage.Items)
      {
        if (GetItemId(storableComponent.Owner) == GetItemId(item))
          storableComponentList.Add(storableComponent);
      }
      storableComponentList.Sort((a, b) => a.Count.CompareTo(b.Count));
      foreach (IStorableComponent key in storableComponentList)
      {
        num -= Mathf.Min(key.Count, amount);
        keyValuePairList.Add(new KeyValuePair<IStorableComponent, int>(key, key.Count - Mathf.Min(key.Count, amount)));
        if (num <= 0)
          break;
      }
      foreach (KeyValuePair<IStorableComponent, int> keyValuePair in keyValuePairList)
      {
        KeyValuePair<IStorableComponent, int> k = keyValuePair;
        IStorableComponent storableComponent = storage.Items.First(x => x.Equals(k.Key));
        storableComponent.Count = k.Value;
        if (storableComponent.Count <= 0)
          storableComponent.Owner.Dispose();
      }
      return amount - num;
    }

    private Guid GetItemId(IEntity item) => item.IsTemplate ? item.Id : item.TemplateId;
  }
}
