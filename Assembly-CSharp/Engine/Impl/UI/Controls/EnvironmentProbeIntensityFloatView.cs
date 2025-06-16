using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class EnvironmentProbeIntensityFloatView : FloatViewBase {
	[SerializeField] private EnvironmentProbe view;

	protected override void ApplyFloatValue() {
		if (!(view != null))
			return;
		view.AmbientIntensity = FloatValue;
	}

	public override void SkipAnimation() { }
}