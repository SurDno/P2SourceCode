using Engine.Source.Services;
using SRDebugger.Services;
using SRF;
using SRF.Service;

namespace SRDebugger.UI.Profiler
{
  public class ProfilerMemoryLabel : SRMonoBehaviour
  {
    [SerializeField]
    private Text _text;
    private IProfilerService _profilerService;
    private float updateFrequency = 1f;
    private float nextUpdate;

    private void Update()
    {
      if ((double) Time.realtimeSinceStartup <= nextUpdate)
        return;
      Refresh();
    }

    private void Refresh()
    {
      nextUpdate = Time.realtimeSinceStartup + updateFrequency;
      long reservedMemoryLong = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
      _text.text = "Total : " + OptimizationUtility.GetMemoryText(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()) + " / " + OptimizationUtility.GetMemoryText(reservedMemoryLong);
    }

    protected void Awake()
    {
      _profilerService = SRServiceManager.GetService<IProfilerService>();
    }
  }
}
