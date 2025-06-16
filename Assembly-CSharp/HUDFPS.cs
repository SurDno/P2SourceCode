using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
  private float accum;
  private Color color = Color.white;
  private int frames;
  public int frequency = 30;
  private string sFPS = "";
  private Rect startRect = new Rect(10f, 10f, 100f, 50f);
  private GUIStyle style;

  private void Update()
  {
    this.accum += Time.deltaTime;
    ++this.frames;
    if (this.frames < this.frequency)
      return;
    this.sFPS = Mathf.Round((float) this.frames / this.accum).ToString();
    this.accum = 0.0f;
    this.frames = 0;
  }

  private void OnGUI()
  {
    if (this.style == null)
    {
      this.style = new GUIStyle(GUI.skin.label);
      this.style.normal.textColor = Color.white;
      this.style.fontStyle = FontStyle.Bold;
      this.style.alignment = TextAnchor.MiddleCenter;
    }
    GUI.Label(new Rect(0.0f, 0.0f, this.startRect.width, this.startRect.height), this.sFPS + " FPS", this.style);
  }
}
