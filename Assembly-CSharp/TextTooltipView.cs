using UnityEngine;

public abstract class TextTooltipView : MonoBehaviour {
	public static TextTooltipView Current { get; protected set; }

	public abstract void Show(Vector2 screenPosition, string text);

	public abstract void Hide();
}