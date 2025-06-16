using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class ProgressView : ProgressViewBase {
	[SerializeField] [Range(0.0f, 1f)] private float progress;

	public override float Progress {
		get => progress;
		set {
			if (progress == (double)value)
				return;
			progress = value;
			ApplyProgress();
		}
	}

	protected virtual void OnValidate() {
		ApplyProgress();
	}

	protected abstract void ApplyProgress();
}