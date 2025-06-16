using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressAnimated : ProgressView {
	[SerializeField] private ProgressViewBase progressView;
	[SerializeField] private HideableView increasingEffect;
	[SerializeField] private HideableView decreasingEffect;
	[SerializeField] private float smoothTime = 1f;
	[SerializeField] private float effectThreshold;
	private float velocity;

	private void Update() {
		if (progressView == null)
			return;
		var progress = progressView.Progress;
		if (progress == (double)Progress)
			return;
		var num = Mathf.MoveTowards(Mathf.SmoothDamp(progress, Progress, ref velocity, smoothTime), Progress,
			Time.deltaTime * (1f / 1000f));
		progressView.Progress = num;
		if (increasingEffect == decreasingEffect) {
			if (!(increasingEffect != null))
				return;
			increasingEffect.Visible =
				Progress - (double)num > effectThreshold || num - (double)Progress > effectThreshold;
		} else {
			if (increasingEffect != null)
				increasingEffect.Visible = Progress - (double)num > effectThreshold;
			if (decreasingEffect != null)
				decreasingEffect.Visible = num - (double)Progress > effectThreshold;
		}
	}

	public override void SkipAnimation() {
		if (progressView != null)
			progressView.Progress = Progress;
		velocity = 0.0f;
		if (increasingEffect != null)
			increasingEffect.Visible = false;
		if (!(decreasingEffect != null))
			return;
		decreasingEffect.Visible = false;
	}

	protected override void ApplyProgress() {
		if (Application.isPlaying)
			return;
		SkipAnimation();
	}
}