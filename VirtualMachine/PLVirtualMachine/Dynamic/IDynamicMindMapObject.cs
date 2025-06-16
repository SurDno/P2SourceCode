using System;

namespace PLVirtualMachine.Dynamic
{
  public interface IDynamicMindMapObject
  {
    Guid DynamicGuid { get; }

    ulong StaticGuid { get; }
  }
}
