using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class HideableSplited : HideableView {
	[SerializeField] private HideableView[] views = new HideableView[0];

	public override void SkipAnimation() {
		foreach (var view in views)
			if (view != null)
				view.SkipAnimation();
	}

	protected override void ApplyVisibility() {
		foreach (var view in views)
			if (view != null)
				view.Visible = Visible;
	}
}