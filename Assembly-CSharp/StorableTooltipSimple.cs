using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

[Factory(typeof (IStorableTooltipComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipSimple : IStorableTooltipComponent
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected bool isEnabled = true;
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  protected StorableTooltipInfo info = new();

  [Inspected]
  public bool IsEnabled => isEnabled;

  public StorableTooltipInfo GetInfo(IEntity owner) => info;
}
