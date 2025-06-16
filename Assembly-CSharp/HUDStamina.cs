using UnityEngine;
using Gradient = Engine.Impl.UI.Controls.Gradient;

public class HUDStamina : MonoBehaviour {
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Gradient leftBar;
	[SerializeField] private Gradient middleBar;
	[SerializeField] private Gradient rightBar;
	[SerializeField] private CanvasGroup marker;
	[SerializeField] private CanvasGroup markerLabel;
	[SerializeField] private float markerVisibilityThreshold;
	[SerializeField] private Vector2 mainFade = Vector2.one;
	[Header("Alert")] [SerializeField] private Color blinkColor = Color.red;
	[SerializeField] private float blinkRate = 1f;
	private Color baseColor;
	private float lastMaxValue;
	private bool labelFadingIn;
	private float blinkPhase;

	public float Value { get; set; }

	public float MaxValue { get; set; }

	public bool Alert { get; set; }

	private void Start() {
		baseColor = rightBar.color;
	}

	private void Update() {
		var flag1 = Value < (double)MaxValue;
		canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, flag1 ? 1f : 0.0f,
			Time.unscaledDeltaTime / (flag1 ? mainFade.x : mainFade.y));
		if (canvasGroup.alpha > 0.0) {
			canvasGroup.gameObject.SetActive(true);
			var x = 1f - MaxValue;
			leftBar.EndPosition = x;
			middleBar.StartPosition = x;
			var vector2 = new Vector2(x, 0.5f);
			var transform = (RectTransform)marker.transform;
			transform.anchorMin = vector2;
			transform.anchorMax = vector2;
			middleBar.EndPosition = x + Value;
			var flag2 = MaxValue < (double)markerVisibilityThreshold;
			marker.alpha = Mathf.MoveTowards(marker.alpha, flag2 ? 1f : 0.0f,
				Time.unscaledDeltaTime / (flag2 ? mainFade.x : mainFade.y));
			var flag3 = lastMaxValue != (double)MaxValue;
			lastMaxValue = MaxValue;
			if (flag3)
				labelFadingIn = true;
			markerLabel.alpha = Mathf.MoveTowards(markerLabel.alpha, labelFadingIn ? 1f : 0.0f,
				Time.unscaledDeltaTime / (labelFadingIn ? mainFade.x : mainFade.y));
			if (markerLabel.alpha == 1.0)
				labelFadingIn = false;
			if (Alert)
				SetBlinkPhase(blinkPhase + Time.deltaTime * blinkRate);
			else if (blinkPhase > 1.0)
				SetBlinkPhase(Mathf.MoveTowards(blinkPhase, 2f, Time.deltaTime * blinkRate));
			else {
				if (blinkPhase <= 0.0)
					return;
				SetBlinkPhase(Mathf.MoveTowards(blinkPhase, 0.0f, Time.deltaTime * blinkRate));
			}
		} else {
			marker.alpha = 0.0f;
			markerLabel.alpha = 0.0f;
			labelFadingIn = false;
			canvasGroup.gameObject.SetActive(false);
		}
	}

	private void SetBlinkPhase(float value) {
		if (value >= 2.0)
			value %= 2f;
		if (blinkPhase == (double)value)
			return;
		blinkPhase = value;
		rightBar.color = Color.Lerp(blinkColor, baseColor, Mathf.Abs(1f - value));
	}
}