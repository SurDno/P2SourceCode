using System;
using System.Diagnostics;

namespace SRDebugger.Profiler
{
  [RequireComponent(typeof (Camera))]
  public class ProfilerCameraListener : MonoBehaviour
  {
    private Camera _camera;
    private Stopwatch _stopwatch;
    public Action<ProfilerCameraListener, double> RenderDurationCallback;

    public Camera Camera => _camera;

    private void OnEnable()
    {
      _camera = this.GetComponent<Camera>();
      _stopwatch = new Stopwatch();
    }

    private void OnPreCull() => _stopwatch.Start();

    private void OnPostRender()
    {
      double totalSeconds = _stopwatch.Elapsed.TotalSeconds;
      _stopwatch.Stop();
      _stopwatch.Reset();
      if (RenderDurationCallback == null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
      else
        RenderDurationCallback(this, totalSeconds);
    }
  }
}
