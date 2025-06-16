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
      MinRandInterval = minRand;
      MaxRandInterval = maxRand;
      StorageEntity = storageEntity;
      Storage = storage;
    }
  }
}
