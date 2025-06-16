using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace PLVirtualMachine.Dynamic.Components
{
  public class CrowdStorageCommand : IStorageCommand
  {
    private Guid targetStorageEntityLoadedId = Guid.Empty;

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      IEntity containerEntity,
      [Template] IEntity template,
      int needCount,
      bool dropIfBusy = false)
    {
      this.StorageCommandType = commandType;
      this.TargetStorage = storage;
      this.TargetContainer = containerEntity;
      this.TargetItemTemplate = template;
      this.NeedItemsCount = needCount;
      this.DropIfBusyMode = dropIfBusy;
    }

    public void Clear()
    {
      this.TargetStorage = (VMStorage) null;
      this.TargetItemTemplate = (IEntity) null;
      this.TargetContainer = (IEntity) null;
    }

    public EStorageCommandType StorageCommandType { get; private set; }

    public VMStorage TargetStorage { get; private set; }

    public IEntity TargetContainer { get; private set; }

    public IEntity TargetItemTemplate { get; private set; }

    public int NeedItemsCount { get; private set; }

    public bool DropIfBusyMode { get; private set; }
  }
}
