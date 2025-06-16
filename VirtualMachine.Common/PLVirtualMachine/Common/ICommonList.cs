using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;

namespace PLVirtualMachine.Common
{
  [VMType("CommonList")]
  public interface ICommonList : IVMStringSerializable
  {
    int ObjectsCount { get; }

    object GetObject(int objIndex);

    void SetObject(int objIndex, object obj);

    VMType GetType(int objIndex);

    void Clear();

    void AddObject(object obj);

    void RemoveObjectByIndex(int objIndex);

    int RemoveObjectInstanceByGuid(Guid objGuid);

    void Merge(ICommonList mergeList);

    bool CheckObjectExist(object obj);

    int GetListIndexOfMaxValue();
  }
}
