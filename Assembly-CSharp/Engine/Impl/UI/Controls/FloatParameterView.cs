using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Impl.UI.Controls;

public class FloatParameterView : EntityViewBase, IChangeParameterListener {
	[SerializeField] [FormerlySerializedAs("progressView")]
	private ProgressViewBase valueView;

	[SerializeField] private ParameterNameEnum parameterName;
	[SerializeField] private float defaultValue;
	[SerializeField] private bool normalized = true;
	private IParameter<float> parameter;

	protected override void ApplyValue() {
		if (parameter != null)
			parameter.RemoveListener(this);
		parameter = Value?.GetComponent<ParametersComponent>()?.GetByName<float>(parameterName);
		if (parameter != null)
			parameter.AddListener(this);
		ApplyParameter();
	}

	private void ApplyParameter() {
		if (!(valueView != null))
			return;
		valueView.Progress = parameter == null ? defaultValue :
			normalized ? parameter.Value / parameter.MaxValue : parameter.Value;
	}

	public override void SkipAnimation() {
		if (!(valueView != null))
			return;
		valueView.SkipAnimation();
	}

	public void OnParameterChanged(IParameter parameter) {
		if (parameter != this.parameter)
			return;
		ApplyParameter();
	}
}