using UnityEngine;

public abstract class KeyCodeView : MonoBehaviour {
	public abstract void SetValue(KeyCode value, bool instant);

	public abstract KeyCode GetValue();
}