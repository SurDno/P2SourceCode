using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;

namespace PLVirtualMachine
{
  public class ItemsStorageAddingInfo(
    float minRand,
    float maxRand,
    IEntity storageEntity,
    VMStorage storage) {
    public float MinRandInterval = minRand;
    public float MaxRandInterval = maxRand;
    public IEntity StorageEntity = storageEntity;
    public VMStorage Storage = storage;
  }
}
