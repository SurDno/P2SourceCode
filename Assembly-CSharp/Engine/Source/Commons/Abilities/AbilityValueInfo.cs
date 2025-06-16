using Engine.Common.Generator;
using Engine.Source.Effects.Values;
using Inspectors;

namespace Engine.Source.Commons.Abilities
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class AbilityValueInfo
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    public AbilityValueNameEnum Name;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    public IAbilityValue Value;
  }
}
