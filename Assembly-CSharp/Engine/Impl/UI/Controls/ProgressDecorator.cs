using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class ProgressDecorator : ProgressView {
	[SerializeField] private ProgressViewBase progressView;

	protected override void ApplyProgress() {
		if (!(progressView != null))
			return;
		progressView.Progress = Progress;
	}

	public override void SkipAnimation() {
		if (!(progressView != null))
			return;
		progressView.SkipAnimation();
	}
}