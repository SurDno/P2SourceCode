using Engine.Source.Services;
using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Profiler
{
  public class ProfilerMonoLabel : SRMonoBehaviour
  {
    [SerializeField]
    private Text _text;
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
      long monoHeapSizeLong = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
      this._text.text = "Mono : " + OptimizationUtility.GetMemoryText(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong()) + " / " + OptimizationUtility.GetMemoryText(monoHeapSizeLong);
    }
  }
}
