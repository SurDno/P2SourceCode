using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

[Factory(typeof (IStorableTooltipComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipSimple : IStorableTooltipComponent
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected bool isEnabled = true;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected StorableTooltipInfo info = new StorableTooltipInfo();

  [Inspected]
  public bool IsEnabled => this.isEnabled;

  public StorableTooltipInfo GetInfo(IEntity owner) => this.info;
}
