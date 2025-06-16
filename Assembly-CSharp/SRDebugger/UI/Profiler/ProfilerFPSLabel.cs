using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Profiler
{
  public class ProfilerFPSLabel : SRMonoBehaviour
  {
    [SerializeField]
    private Text _text;
    private IProfilerService _profilerService;
    private float updateFrequency = 1f;
    private float nextUpdate;
    private int count;

    private void Update()
    {
      ++this.count;
      if ((double) Time.realtimeSinceStartup <= (double) this.nextUpdate)
        return;
      this.Refresh();
    }

    private void Refresh()
    {
      this._text.text = "FPS: " + (object) this.count;
      this.nextUpdate = Time.realtimeSinceStartup + this.updateFrequency;
      this.count = 0;
    }

    protected void Awake()
    {
      this._profilerService = SRServiceManager.GetService<IProfilerService>();
    }
  }
}
