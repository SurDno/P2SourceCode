using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;

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
