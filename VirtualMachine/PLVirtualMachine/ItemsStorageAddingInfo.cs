// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.ItemsStorageAddingInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;

#nullable disable
namespace PLVirtualMachine
{
  public class ItemsStorageAddingInfo
  {
    public float MinRandInterval;
    public float MaxRandInterval;
    public IEntity StorageEntity;
    public VMStorage Storage;

    public ItemsStorageAddingInfo(
      float minRand,
      float maxRand,
      IEntity storageEntity,
      VMStorage storage)
    {
      this.MinRandInterval = minRand;
      this.MaxRandInterval = maxRand;
      this.StorageEntity = storageEntity;
      this.Storage = storage;
    }
  }
}
