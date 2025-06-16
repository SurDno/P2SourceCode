using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls;

public class ProgressMinLayoutHeight : ProgressView {
	[SerializeField] private LayoutElement element;
	[SerializeField] private float min;
	[SerializeField] private float max;

	public override void SkipAnimation() { }

	protected override void ApplyProgress() {
		if (!(element != null))
			return;
		element.minHeight = Mathf.Lerp(min, max, Progress);
	}
}