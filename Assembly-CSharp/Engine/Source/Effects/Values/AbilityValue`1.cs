using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Values
{
  public class AbilityValue<T> : IAbilityValue<T>, IAbilityValue where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    protected T value;

    public T Value
    {
      get => this.value;
      set => this.value = value;
    }
  }
}
