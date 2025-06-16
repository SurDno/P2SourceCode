using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Expressions;
using Inspectors;

namespace Engine.Source.Effects.Values;

public abstract class EffectAbilityValue<T> : IValue<T> where T : struct {
	[DataReadProxy(Name = "AbilityValueName")]
	[DataWriteProxy(Name = "AbilityValueName")]
	[CopyableProxy()]
	[Inspected]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected AbilityValueNameEnum valueName;

	public AbilityValueNameEnum ParameterName => valueName;

	public T GetValue(IEffect context) {
		if (context.AbilityItem.AbilityController is IAbilityValueContainer abilityController) {
			var abilityValue = abilityController.GetAbilityValue<T>(valueName);
			if (abilityValue != null)
				return abilityValue.Value;
		}

		return default;
	}

	public string ValueView => valueName.ToString();

	public string TypeView => TypeUtility.GetTypeName(GetType());
}