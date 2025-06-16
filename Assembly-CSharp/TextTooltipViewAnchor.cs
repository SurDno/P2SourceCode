using UnityEngine;

public class TextTooltipViewAnchor : TextTooltipView {
	[SerializeField] private TextTooltipView prefab;
	private TextTooltipView view;

	public override void Hide() {
		view?.Hide();
	}

	private void OnEnable() {
		if (!(Current == null))
			return;
		Current = this;
	}

	private void OnDisable() {
		if (!(Current == this))
			return;
		Current = null;
	}

	public override void Show(Vector2 screenPosition, string text) {
		if (view == null)
			view = Instantiate(prefab, transform, false);
		view.Show(screenPosition, text);
	}
}