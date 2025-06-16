using System;

namespace PLVirtualMachine.Common
{
  public interface IEngineInstanced : IEngineTemplated
  {
    Guid EngineGuid { get; }

    IBlueprint EditorTemplate { get; }
  }
}
