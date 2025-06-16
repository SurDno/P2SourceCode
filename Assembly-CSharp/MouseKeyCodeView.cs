using UnityEngine;
using UnityEngine.UI;

public class MouseKeyCodeView : KeyCodeViewBase {
	[SerializeField] private Image image;
	[SerializeField] private Sprite[] buttonIcons;

	protected override void ApplyValue(bool instant) {
		if (image == null)
			return;
		if (buttonIcons != null) {
			var index = (int)(GetValue() - 323);
			if (index >= 0 || index < buttonIcons.Length) {
				image.sprite = buttonIcons[index];
				return;
			}
		}

		image.sprite = null;
	}
}