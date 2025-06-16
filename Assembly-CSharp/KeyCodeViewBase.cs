using UnityEngine;

public abstract class KeyCodeViewBase : KeyCodeView {
	[SerializeField] private KeyCode value;

	private void OnValidate() {
		if (Application.isPlaying)
			return;
		ApplyValue(true);
	}

	public override KeyCode GetValue() {
		return value;
	}

	public override void SetValue(KeyCode value, bool instant) {
		if (this.value == value)
			return;
		this.value = value;
		ApplyValue(instant);
	}

	protected abstract void ApplyValue(bool instant);
}