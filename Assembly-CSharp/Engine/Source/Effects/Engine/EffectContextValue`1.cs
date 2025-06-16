using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Expressions;
using Inspectors;

namespace Engine.Source.Effects.Engine;

public abstract class EffectContextValue<T> : IValueSetter<T>, IValue<T> where T : struct {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected EffectContextEnum effectContext;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum parameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterDataEnum parameterData;

	public T GetValue(IEffect context) {
		var entity = ExpressionEffectUtility.GetEntity(effectContext, context);
		if (entity != null) {
			var component = entity.GetComponent<ParametersComponent>();
			if (component != null) {
				var byName = component.GetByName<T>(parameterName);
				if (byName != null) {
					if (parameterData == ParameterDataEnum.BaseValue)
						return byName.BaseValue;
					if (parameterData == ParameterDataEnum.Value)
						return byName.Value;
					if (parameterData == ParameterDataEnum.MaxValue)
						return byName.MaxValue;
					if (parameterData == ParameterDataEnum.MinValue)
						return byName.MinValue;
				}
			}
		}

		return default;
	}

	public void SetValue(T value, IEffect context) {
		var entity = ExpressionEffectUtility.GetEntity(effectContext, context);
		if (entity == null)
			return;
		var component = entity.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<T>(parameterName);
			if (byName != null) {
				if (parameterData == ParameterDataEnum.BaseValue)
					byName.BaseValue = value;
				else if (parameterData == ParameterDataEnum.Value)
					byName.Value = value;
				else if (parameterData == ParameterDataEnum.MaxValue)
					byName.MaxValue = value;
				else if (parameterData == ParameterDataEnum.MinValue)
					byName.MinValue = value;
			}
		}
	}

	public string ValueView => effectContext + "." + parameterName + "." + parameterData;

	public string TypeView => TypeUtility.GetTypeName(GetType());
}