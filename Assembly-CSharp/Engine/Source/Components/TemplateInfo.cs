using System;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TemplateInfo
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mode = ExecuteMode.Edit)]
    public Guid Id = Guid.NewGuid();
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public Typed<IEntity> InventoryTemplate;
  }
}
