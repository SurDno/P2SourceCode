using SRDebugger.Services;
using SRF;
using SRF.Service;

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
      ++count;
      if ((double) Time.realtimeSinceStartup <= nextUpdate)
        return;
      Refresh();
    }

    private void Refresh()
    {
      _text.text = "FPS: " + count;
      nextUpdate = Time.realtimeSinceStartup + updateFrequency;
      count = 0;
    }

    protected void Awake()
    {
      _profilerService = SRServiceManager.GetService<IProfilerService>();
    }
  }
}
