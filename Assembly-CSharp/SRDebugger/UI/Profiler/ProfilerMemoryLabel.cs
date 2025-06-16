using Engine.Source.Services;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;
using UnityEngine.UI;

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
      if ((double) Time.realtimeSinceStartup <= (double) this.nextUpdate)
        return;
      this.Refresh();
    }

    private void Refresh()
    {
      this.nextUpdate = Time.realtimeSinceStartup + this.updateFrequency;
      long reservedMemoryLong = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
      this._text.text = "Total : " + OptimizationUtility.GetMemoryText(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()) + " / " + OptimizationUtility.GetMemoryText(reservedMemoryLong);
    }

    protected void Awake()
    {
      this._profilerService = SRServiceManager.GetService<IProfilerService>();
    }
  }
}
