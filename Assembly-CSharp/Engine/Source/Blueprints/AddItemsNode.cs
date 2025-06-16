// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.AddItemsNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;

#nullable disable
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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player != null)
        {
          StorageComponent component1 = player.GetComponent<StorageComponent>();
          if (component1 != null)
          {
            int num = this.amount.value;
            for (int index = 0; index < num; ++index)
            {
              IFactory service1 = ServiceLocator.GetService<IFactory>();
              IEntitySerializable ientitySerializable = this.addItem.value;
              IEntity template = ientitySerializable.Value;
              IEntity entity = service1.Instantiate<IEntity>(template);
              ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
              StorableComponent component2 = entity.GetComponent<StorableComponent>();
              if (StorageUtility.GetIntersect((IStorageComponent) component1, (IInventoryComponent) null, entity.GetComponent<StorableComponent>(), (Cell) null).IsAllowed)
              {
                component1.AddItem((IStorableComponent) component2, (IInventoryComponent) null);
                NotificationService service2 = ServiceLocator.GetService<NotificationService>();
                object[] objArray = new object[1];
                ientitySerializable = this.addItem.value;
                objArray[0] = (object) ientitySerializable.Value;
                service2.AddNotify(NotificationEnum.ItemRecieve, objArray);
              }
              else
                ServiceLocator.GetService<DropBagService>().DropBag((IStorableComponent) component2, component1.Owner);
            }
          }
        }
        output.Call();
      }));
      this.addItem = this.AddValueInput<IEntitySerializable>("Add Item");
      this.amount = this.AddValueInput<int>("Amount");
    }

    private Guid GetItemId(IEntity item) => item.IsTemplate ? item.Id : item.TemplateId;
  }
}
