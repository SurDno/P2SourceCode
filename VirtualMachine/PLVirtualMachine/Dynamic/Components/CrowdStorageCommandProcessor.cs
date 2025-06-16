// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.CrowdStorageCommandProcessor
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  public class CrowdStorageCommandProcessor : StorageCommandProcessor
  {
    protected override void ProcessCommand(IStorageCommand storageCommand)
    {
      CrowdStorageCommand crowdStorageCommand = (CrowdStorageCommand) storageCommand;
      if (crowdStorageCommand.StorageCommandType != EStorageCommandType.StorageCommandTypeAddItem)
        return;
      this.ProcessPickUpToInentoryByTemplate(crowdStorageCommand.TargetStorage, crowdStorageCommand.TargetContainer, crowdStorageCommand.TargetItemTemplate, crowdStorageCommand.NeedItemsCount);
    }

    private void ProcessPickUpToInentoryByTemplate(
      VMStorage storage,
      IEntity targetContainer,
      IEntity itemTemplate,
      int needItemsCount)
    {
      int num = 0;
      if (storage.Parent.IsDisposed)
        return;
      do
      {
        int iNeedCount = needItemsCount - num;
        int inventoryByTemplate = this.DoAddItemsToInventoryByTemplate(storage, targetContainer, itemTemplate, iNeedCount);
        if (inventoryByTemplate != 0)
          num += inventoryByTemplate;
        else
          goto label_1;
      }
      while (num < needItemsCount);
      goto label_5;
label_1:
      return;
label_5:;
    }

    private int DoAddItemsToInventoryByTemplate(
      VMStorage storage,
      IEntity targetContainer,
      IEntity template,
      int iNeedCount)
    {
      IStorableComponent storableComponent = VMStorable.MakeStorableByTemplate(storage.Parent, template);
      IEntity owner = storableComponent.Owner;
      IStorageComponent component1 = storage.Component;
      IStorableComponent component2 = owner.GetComponent<IStorableComponent>();
      int inventoryByTemplate = component2.Max;
      if (inventoryByTemplate > iNeedCount)
        inventoryByTemplate = iNeedCount;
      component2.Count = inventoryByTemplate;
      if (VMStorage.DoAddItemToStorage(storage.Component, component2, targetContainer.GetComponent<IInventoryComponent>(), true))
        return inventoryByTemplate;
      storableComponent.Owner.Dispose();
      return 0;
    }
  }
}
