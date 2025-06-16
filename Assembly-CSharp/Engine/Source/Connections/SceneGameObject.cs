using System;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Connections
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SceneGameObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    protected Guid id;

    [Inspected]
    public Guid Id => id;
  }
}
