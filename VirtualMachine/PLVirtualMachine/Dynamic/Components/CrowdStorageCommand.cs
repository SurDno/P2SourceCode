// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.CrowdStorageCommand
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
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
