using System;
using System.Diagnostics;
using UnityEngine;

namespace SRDebugger.Profiler
{
  [RequireComponent(typeof (Camera))]
  public class ProfilerCameraListener : MonoBehaviour
  {
    private Camera _camera;
    private Stopwatch _stopwatch;
    public Action<ProfilerCameraListener, double> RenderDurationCallback;

    public Camera Camera => this._camera;

    private void OnEnable()
    {
      this._camera = this.GetComponent<Camera>();
      this._stopwatch = new Stopwatch();
    }

    private void OnPreCull() => this._stopwatch.Start();

    private void OnPostRender()
    {
      double totalSeconds = this._stopwatch.Elapsed.TotalSeconds;
      this._stopwatch.Stop();
      this._stopwatch.Reset();
      if (this.RenderDurationCallback == null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
      else
        this.RenderDurationCallback(this, totalSeconds);
    }
  }
}
