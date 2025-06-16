using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class FloatParameterMaxView : EntityViewBase {
	[SerializeField] private ProgressViewBase valueView;
	[SerializeField] private ParameterNameEnum parameterName;
	[SerializeField] private float defaultValue = 1f;
	private IParameter<float> parameter;

	private void Update() {
		ApplyParameter();
	}

	protected override void ApplyValue() {
		parameter = Value?.GetComponent<ParametersComponent>()?.GetByName<float>(parameterName);
		ApplyParameter();
	}

	private void ApplyParameter() {
		if (!(valueView != null))
			return;
		valueView.Progress = parameter == null ? defaultValue : parameter.MaxValue;
	}

	public override void SkipAnimation() {
		if (!(valueView != null))
			return;
		valueView.SkipAnimation();
	}
}