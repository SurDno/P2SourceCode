using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public interface IHierarchyObject : IEngineInstanced, IEngineTemplated, INamed
  {
    HierarchyGuid HierarchyGuid { get; }

    IEnumerable<IHierarchyObject> HierarchyChilds { get; }

    void AddHierarchyChild(IHierarchyObject child);

    void InitInstanceGuid(Guid instanceGuid);
  }
}
