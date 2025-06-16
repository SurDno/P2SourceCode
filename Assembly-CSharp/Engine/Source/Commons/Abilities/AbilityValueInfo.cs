using Engine.Common.Generator;
using Engine.Source.Effects.Values;
using Inspectors;

namespace Engine.Source.Commons.Abilities
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AbilityValueInfo
  {
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy]
    public AbilityValueNameEnum Name;
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy()]
    public IAbilityValue Value;
  }
}
