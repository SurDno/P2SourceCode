using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class HideableEnabling : HideableView {
	[SerializeField] private Behaviour component;

	protected override void ApplyVisibility() {
		if (!(component != null))
			return;
		component.enabled = Visible;
	}
}