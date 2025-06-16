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
      if (Time.realtimeSinceStartup <= (double) nextUpdate)
        return;
      Refresh();
    }

    private void Refresh()
    {
      nextUpdate = Time.realtimeSinceStartup + updateFrequency;
      long monoHeapSizeLong = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
      _text.text = "Mono : " + OptimizationUtility.GetMemoryText(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong()) + " / " + OptimizationUtility.GetMemoryText(monoHeapSizeLong);
    }
  }
}
