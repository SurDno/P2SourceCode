using System.Collections.Generic;
using System.Linq;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Crowds;

public static class CrowdContextUtility {
	public static void Store(
		ParametersComponent parameters,
		List<IParameter> states,
		params ParameterNameEnum[] names) {
		if (parameters == null)
			return;
		foreach (var name in names) {
			var byName = parameters.GetByName(name);
			if (byName != null) {
				var parameter = CloneableObjectUtility.Clone(byName);
				states.Add(parameter);
			}
		}
	}

	public static void Restore(
		ParametersComponent parameters,
		List<IParameter> states,
		params ParameterNameEnum[] names) {
		if (parameters == null)
			return;
		foreach (var state in states)
			if (names.Contains(state.Name)) {
				var byName = parameters.GetByName(state.Name);
				if (byName != null && byName.GetType() == state.GetType())
					((ICopyable)state).CopyTo(byName);
			}
	}
}