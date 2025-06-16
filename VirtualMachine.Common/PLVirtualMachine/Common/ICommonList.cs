// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.ICommonList
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;

#nullable disable
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
