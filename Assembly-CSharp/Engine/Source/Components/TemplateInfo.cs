using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Inspectors;
using System;

namespace Engine.Source.Components
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TemplateInfo
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mode = ExecuteMode.Edit)]
    public Guid Id = Guid.NewGuid();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public Typed<IEntity> InventoryTemplate;
  }
}
