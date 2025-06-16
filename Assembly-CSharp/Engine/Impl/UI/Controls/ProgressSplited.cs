using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressSplited : ProgressView {
	[SerializeField] private FloatView[] nestedViews = new FloatView[0];

	protected override void ApplyProgress() {
		foreach (var nestedView in nestedViews)
			if (nestedView != null)
				nestedView.FloatValue = Progress;
	}

	public override void SkipAnimation() {
		foreach (var nestedView in nestedViews)
			if (nestedView != null)
				nestedView.SkipAnimation();
	}
}