using System;
using Engine.Common.Components.Parameters;
using Inspectors;

namespace Engine.Source.Commons.Parameters;

public class ParameterValue<T> :
	IParameterValue<T>,
	IParameterValueSet<T>,
	IChangeParameterListener
	where T : struct {
	[Inspected] private IParameter<T> parameter;

	public void Set(IParameter<T> parameter) {
		if (this.parameter != null)
			this.parameter.RemoveListener(this);
		this.parameter = parameter;
		if (this.parameter == null)
			return;
		this.parameter.AddListener(this);
	}

	public void OnParameterChanged(IParameter parameter) {
		var changeValueEvent = ChangeValueEvent;
		if (changeValueEvent == null)
			return;
		changeValueEvent(((IParameter<T>)parameter).Value);
	}

	public T Value {
		get => parameter != null ? parameter.Value : default;
		set {
			if (parameter == null)
				return;
			parameter.Value = value;
		}
	}

	public T MinValue {
		get => parameter != null ? parameter.MinValue : default;
		set {
			if (parameter == null)
				return;
			parameter.MinValue = value;
		}
	}

	public T MaxValue {
		get => parameter != null ? parameter.MaxValue : default;
		set {
			if (parameter == null)
				return;
			parameter.MaxValue = value;
		}
	}

	public event Action<T> ChangeValueEvent;
}