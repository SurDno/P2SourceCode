using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressCanvasAlpha : ProgressView {
	[SerializeField] private CanvasGroup canvasGroup;

	public override void SkipAnimation() { }

	protected override void ApplyProgress() {
		if (!(canvasGroup != null))
			return;
		canvasGroup.alpha = Progress;
		var flag = Progress > 0.0;
		canvasGroup.interactable = flag;
		canvasGroup.blocksRaycasts = flag;
	}
}