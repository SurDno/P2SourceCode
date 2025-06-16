using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Connections;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class RemoveItemsNode : FlowControlNode
  {
    private ValueInput<IEntitySerializable> removeItem;
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
          StorageComponent component = player.GetComponent<StorageComponent>();
          if (component != null)
            RemoveItemsAmount(component, removeItem.value.Value, amount.value);
        }
        output.Call();
      });
      removeItem = AddValueInput<IEntitySerializable>("Remove Item");
      amount = AddValueInput<int>("Amount");
    }

    private int RemoveItemsAmount(IStorageComponent storage, IEntity item, int amount)
    {
      int num = amount;
      List<KeyValuePair<IStorableComponent, int>> keyValuePairList = new List<KeyValuePair<IStorableComponent, int>>();
      foreach (IStorableComponent key in storage.Items)
      {
        if (GetItemId(key.Owner) == GetItemId(item))
        {
          num -= Mathf.Min(key.Count, amount);
          keyValuePairList.Add(new KeyValuePair<IStorableComponent, int>(key, key.Count - Mathf.Min(key.Count, amount)));
        }
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
