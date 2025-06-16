using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Inspectors;

namespace Engine.Source.Components.Repairing
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RepairableCostItem
  {
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy]
    protected Typed<IEntity> template;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
    protected int count;

    [Inspected]
    public Typed<IEntity> Template => template;

    [Inspected]
    public int Count => count;
  }
}
