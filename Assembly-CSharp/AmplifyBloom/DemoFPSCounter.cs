using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
  public class DemoFPSCounter : MonoBehaviour
  {
    public float UpdateInterval = 0.5f;
    private Text m_fpsText;
    private float m_accum = 0.0f;
    private int m_frames = 0;
    private float m_timeleft;
    private float m_fps;
    private string m_format;

    private void Start()
    {
      this.m_fpsText = this.GetComponent<Text>();
      this.m_timeleft = this.UpdateInterval;
    }

    private void Update()
    {
      this.m_timeleft -= Time.deltaTime;
      this.m_accum += Time.timeScale / Time.deltaTime;
      ++this.m_frames;
      if ((double) this.m_timeleft > 0.0)
        return;
      this.m_fps = this.m_accum / (float) this.m_frames;
      this.m_format = string.Format("{0:F2} FPS", (object) this.m_fps);
      this.m_fpsText.text = this.m_format;
      if ((double) this.m_fps < 50.0)
        this.m_fpsText.color = Color.yellow;
      else if ((double) this.m_fps < 30.0)
        this.m_fpsText.color = Color.red;
      else
        this.m_fpsText.color = Color.green;
      this.m_timeleft = this.UpdateInterval;
      this.m_accum = 0.0f;
      this.m_frames = 0;
    }
  }
}
