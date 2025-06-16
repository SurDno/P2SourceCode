using System;

namespace PLVirtualMachine.Common
{
  public interface IWorldObject : IEngineTemplated
  {
    HierarchyGuid WorldPositionGuid { get; }

    bool IsPhysic { get; }

    Guid EngineBaseTemplateGuid { get; }

    bool DirectEngineCreated { get; }

    bool IsEngineRoot { get; }

    bool Instantiated { get; }
  }
}
