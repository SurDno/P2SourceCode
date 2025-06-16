// Decompiled with JetBrains decompiler
// Type: SRDebugger.Profiler.ProfilerCameraListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;

#nullable disable
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
