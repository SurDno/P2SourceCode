using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class StringView : MonoBehaviour {
	[SerializeField] private string stringValue;

	public string StringValue {
		get => stringValue;
		set {
			if (stringValue == value)
				return;
			stringValue = value;
			ApplyStringValue();
		}
	}

	private void OnValidate() {
		if (Application.isPlaying)
			return;
		ApplyStringValue();
		SkipAnimation();
	}

	protected abstract void ApplyStringValue();

	public abstract void SkipAnimation();
}