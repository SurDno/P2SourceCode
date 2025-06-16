using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressCurve : ProgressView {
	[SerializeField] private FloatView view;
	[SerializeField] private AnimationCurve curve = new();

	public override void SkipAnimation() {
		if (!(view != null))
			return;
		view.SkipAnimation();
	}

	protected override void ApplyProgress() {
		if (!(view != null))
			return;
		view.FloatValue = curve.Evaluate(FloatValue);
	}
}