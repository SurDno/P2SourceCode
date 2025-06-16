using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressRemapped : ProgressView {
	[SerializeField] private ProgressViewBase nestedView;
	[SerializeField] private Vector2 targetRange = new(0.0f, 1f);

	protected override void ApplyProgress() {
		if (!(nestedView != null))
			return;
		nestedView.Progress = Mathf.Lerp(targetRange.x, targetRange.y, Progress);
	}

	public void SetMin(float min) {
		targetRange.x = min;
		ApplyProgress();
	}

	public void SetMax(float max) {
		targetRange.y = max;
		ApplyProgress();
	}

	public override void SkipAnimation() {
		if (!(nestedView != null))
			return;
		nestedView.SkipAnimation();
	}
}