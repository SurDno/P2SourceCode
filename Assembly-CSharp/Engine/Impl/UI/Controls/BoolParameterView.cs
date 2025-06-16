using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Impl.UI.Controls;

public class BoolParameterView : EntityViewBase, IChangeParameterListener {
	[SerializeField] [FormerlySerializedAs("progressView")]
	private HideableView view;

	[SerializeField] private ParameterNameEnum parameterName;
	[SerializeField] private bool defaultValue;
	private IParameter<bool> parameter;

	protected override void ApplyValue() {
		if (parameter != null)
			parameter.RemoveListener(this);
		parameter = Value?.GetComponent<ParametersComponent>()?.GetByName<bool>(parameterName);
		if (parameter != null)
			parameter.AddListener(this);
		ApplyParameter();
	}

	private void ApplyParameter() {
		if (!(view != null))
			return;
		view.Visible = parameter != null ? parameter.Value : defaultValue;
	}

	public override void SkipAnimation() {
		if (!(view != null))
			return;
		view.SkipAnimation();
	}

	public void OnParameterChanged(IParameter parameter) {
		if (parameter != this.parameter)
			return;
		ApplyParameter();
	}
}