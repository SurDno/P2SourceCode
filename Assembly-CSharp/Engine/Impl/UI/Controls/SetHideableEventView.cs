using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class SetHideableEventView : EventView {
	[SerializeField] private HideableView view;
	[SerializeField] private bool value;

	public override void Invoke() {
		if (!(view != null))
			return;
		view.Visible = value;
	}
}