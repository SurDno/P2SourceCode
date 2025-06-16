using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressRemappedSettings : ProgressView {
	[SerializeField] private ProgressRemapped view;
	[SerializeField] private bool max;

	protected override void ApplyProgress() {
		if (view == null)
			return;
		if (max)
			view.SetMax(Progress);
		else
			view.SetMin(Progress);
	}

	public override void SkipAnimation() {
		if (!(view != null))
			return;
		view.SkipAnimation();
	}
}