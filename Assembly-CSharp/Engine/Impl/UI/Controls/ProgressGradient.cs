using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class ProgressGradient : ProgressView {
	[SerializeField] private Gradient endGradient;
	[SerializeField] private Gradient startGradient;

	public override void SkipAnimation() { }

	protected override void ApplyProgress() {
		if (endGradient != null)
			endGradient.EndPosition = Progress;
		if (!(startGradient != null))
			return;
		startGradient.StartPosition = Progress;
	}
}