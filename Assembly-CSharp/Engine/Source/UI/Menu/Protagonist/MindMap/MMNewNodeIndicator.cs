using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.MindMap;

public class MMNewNodeIndicator : MonoBehaviour {
	[SerializeField] private CanvasGroup image0;
	[SerializeField] private CanvasGroup image1;
	[SerializeField] private float loopLength = 1f;
	[SerializeField] private float farScale = 1.2f;
	private float time;

	private void OnEnable() {
		time = Random.value * loopLength;
	}

	private void Update() {
		time += Time.unscaledDeltaTime;
		ApplyTime();
	}

	private void ApplyTime() {
		var f = time / loopLength;
		var phase1 = f - Mathf.Floor(f);
		ApplyPhase(image0, phase1);
		var phase2 = phase1 + 0.5f;
		if (phase2 >= 1.0)
			--phase2;
		ApplyPhase(image1, phase2);
	}

	private void ApplyPhase(CanvasGroup image, float phase) {
		var f = phase * 3.14159274f;
		var num1 = Mathf.Sin(f);
		var num2 = (float)(1.0 + (farScale - 1.0) * (Mathf.Cos(f) * 0.5 + 0.5));
		image.alpha = num1;
		image.transform.localScale = new Vector3(num2, num2, num2);
	}
}