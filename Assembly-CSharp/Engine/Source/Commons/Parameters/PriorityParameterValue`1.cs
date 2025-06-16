using System;
using Engine.Common.Components.Parameters;
using Inspectors;

namespace Engine.Source.Commons.Parameters;

public class PriorityParameterValue<T> :
	IPriorityParameterValue<T>,
	IParameterValue<T>,
	IParameterValueSet<T>,
	IChangeParameterListener
	where T : struct {
	[Inspected] private PriorityParameter<T> parameter;

	public void Set(IParameter<T> parameter) {
		if (this.parameter != null)
			this.parameter.RemoveListener(this);
		this.parameter = parameter as PriorityParameter<T>;
		if (this.parameter == null)
			return;
		this.parameter.AddListener(this);
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

	public void SetValue(PriorityParameterEnum priority, T value) {
		if (parameter == null)
			return;
		parameter.SetValue(priority, value);
	}

	public bool TryGetValue(PriorityParameterEnum priority, out T result) {
		if (parameter != null)
			return parameter.TryGetValue(priority, out result);
		result = default;
		return false;
	}

	public void ResetValue(PriorityParameterEnum priority) {
		if (parameter == null)
			return;
		parameter.ResetValue(priority);
	}

	public void OnParameterChanged(IParameter parameter) {
		var changeValueEvent = ChangeValueEvent;
		if (changeValueEvent == null)
			return;
		changeValueEvent(((IParameter<T>)parameter).Value);
	}
}