using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class SingleColorView : MonoBehaviour, IValueView<Color> {
	[SerializeField] private Color value = Color.white;

	protected Color GetValue() {
		return value;
	}

	public Color GetValue(int id) {
		return value;
	}

	public void SetValue(int id, Color value, bool instant) {
		if (!instant && this.value == value)
			return;
		this.value = value;
		ApplyValue(instant);
	}

	protected abstract void ApplyValue(bool instant);
}