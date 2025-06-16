using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Expressions;
using Inspectors;

namespace Engine.Source.Effects.Values
{
  public abstract class EffectAbilityValue<T> : IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None, Name = "AbilityValueName")]
    [DataWriteProxy(MemberEnum.None, Name = "AbilityValueName")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected AbilityValueNameEnum valueName;

    public AbilityValueNameEnum ParameterName => this.valueName;

    public T GetValue(IEffect context)
    {
      if (context.AbilityItem.AbilityController is IAbilityValueContainer abilityController)
      {
        IAbilityValue<T> abilityValue = abilityController.GetAbilityValue<T>(this.valueName);
        if (abilityValue != null)
          return abilityValue.Value;
      }
      return default (T);
    }

    public string ValueView => this.valueName.ToString();

    public string TypeView => TypeUtility.GetTypeName(this.GetType());
  }
}
