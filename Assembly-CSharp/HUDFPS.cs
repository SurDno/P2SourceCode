using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour {
	private float accum;
	private Color color = Color.white;
	private int frames;
	public int frequency = 30;
	private string sFPS = "";
	private Rect startRect = new(10f, 10f, 100f, 50f);
	private GUIStyle style;

	private void Update() {
		accum += Time.deltaTime;
		++frames;
		if (frames < frequency)
			return;
		sFPS = Mathf.Round(frames / accum).ToString();
		accum = 0.0f;
		frames = 0;
	}

	private void OnGUI() {
		if (style == null) {
			style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
		}

		GUI.Label(new Rect(0.0f, 0.0f, startRect.width, startRect.height), sFPS + " FPS", style);
	}
}