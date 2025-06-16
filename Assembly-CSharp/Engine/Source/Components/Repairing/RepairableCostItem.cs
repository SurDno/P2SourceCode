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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected Typed<IEntity> template;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected int count;

    [Inspected]
    public Typed<IEntity> Template => this.template;

    [Inspected]
    public int Count => this.count;
  }
}
