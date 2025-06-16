using SRDebugger.Services;
using SRF;
using SRF.Service;
using System;
using System.Diagnostics;
using UnityEngine;

namespace SRDebugger.Profiler
{
  [SRF.Service.Service(typeof (IProfilerService))]
  public class ProfilerServiceImpl : SRServiceBase<IProfilerService>, IProfilerService
  {
    private const int FrameBufferSize = 400;
    private readonly SRList<ProfilerCameraListener> _cameraListeners = new SRList<ProfilerCameraListener>();
    private readonly CircularBuffer.CircularBuffer<ProfilerFrame> _frameBuffer = new CircularBuffer.CircularBuffer<ProfilerFrame>(400);
    private Camera[] _cameraCache = new Camera[6];
    private int _expectedCameraCount;
    private ProfilerLateUpdateListener _lateUpdateListener;
    private double _renderDuration;
    private int _reportedCameras;
    private Stopwatch _stopwatch = new Stopwatch();
    private double _updateDuration;
    private double _updateToRenderDuration;
    private double _customDuration;

    public float AverageFrameTime { get; private set; }

    public float LastFrameTime { get; private set; }

    public void SetCustom(double value) => this._customDuration = value;

    public CircularBuffer.CircularBuffer<ProfilerFrame> FrameBuffer => this._frameBuffer;

    protected void PushFrame(
      double totalTime,
      double updateTime,
      double renderTime,
      double customTime)
    {
      this._frameBuffer.PushBack(new ProfilerFrame()
      {
        OtherTime = totalTime - updateTime - renderTime - customTime,
        UpdateTime = updateTime,
        RenderTime = renderTime,
        CustomTime = customTime
      });
    }

    protected override void Awake()
    {
      base.Awake();
      this._lateUpdateListener = this.gameObject.AddComponent<ProfilerLateUpdateListener>();
      this._lateUpdateListener.OnLateUpdate = new Action(this.OnLateUpdate);
      this.CachedGameObject.hideFlags = HideFlags.NotEditable;
      this.CachedTransform.SetParent(Hierarchy.Get("SRDebugger"), true);
    }

    protected void Update()
    {
      if (this.FrameBuffer.Size > 0)
        this.FrameBuffer[this.FrameBuffer.Size - 1] = this.FrameBuffer.Back() with
        {
          FrameTime = (double) Time.deltaTime
        };
      this.LastFrameTime = Time.deltaTime;
      int num1 = Mathf.Min(20, this.FrameBuffer.Size);
      double num2 = 0.0;
      for (int index = 0; index < num1; ++index)
        num2 += this.FrameBuffer[index].FrameTime;
      this.AverageFrameTime = (float) num2 / (float) num1;
      if (this._reportedCameras == this._cameraListeners.Count)
        ;
      if (this._stopwatch.IsRunning)
      {
        this._stopwatch.Stop();
        this._stopwatch.Reset();
      }
      this._updateDuration = this._renderDuration = this._updateToRenderDuration = 0.0;
      this._reportedCameras = 0;
      this.CameraCheck();
      this._expectedCameraCount = 0;
      for (int index = 0; index < this._cameraListeners.Count; ++index)
      {
        if (this._cameraListeners[index].isActiveAndEnabled && this._cameraListeners[index].Camera.isActiveAndEnabled)
          ++this._expectedCameraCount;
      }
      this._stopwatch.Start();
    }

    private void OnLateUpdate() => this._updateDuration = this._stopwatch.Elapsed.TotalSeconds;

    private void EndFrame()
    {
      if (!this._stopwatch.IsRunning)
        return;
      double customTime = Math.Min(this._customDuration, this._updateDuration);
      this._customDuration = 0.0;
      this.PushFrame(this._stopwatch.Elapsed.TotalSeconds, this._updateDuration - customTime, this._renderDuration, customTime);
      this._stopwatch.Reset();
      this._stopwatch.Start();
    }

    private void CameraDurationCallback(ProfilerCameraListener listener, double duration)
    {
      ++this._reportedCameras;
      this._renderDuration = this._stopwatch.Elapsed.TotalSeconds - this._updateDuration - this._updateToRenderDuration;
      if (this._reportedCameras < this._expectedCameraCount)
        return;
      this.EndFrame();
    }

    private void CameraCheck()
    {
      for (int index = this._cameraListeners.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this._cameraListeners[index] == (UnityEngine.Object) null)
          this._cameraListeners.RemoveAt(index);
      }
      if (Camera.allCamerasCount == this._cameraListeners.Count)
        return;
      if (Camera.allCamerasCount > this._cameraCache.Length)
        this._cameraCache = new Camera[Camera.allCamerasCount];
      int allCameras = Camera.GetAllCameras(this._cameraCache);
      for (int index1 = 0; index1 < allCameras; ++index1)
      {
        Camera camera = this._cameraCache[index1];
        bool flag = false;
        for (int index2 = 0; index2 < this._cameraListeners.Count; ++index2)
        {
          if ((UnityEngine.Object) this._cameraListeners[index2].Camera == (UnityEngine.Object) camera)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          ProfilerCameraListener profilerCameraListener = camera.gameObject.AddComponent<ProfilerCameraListener>();
          profilerCameraListener.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
          profilerCameraListener.RenderDurationCallback = new Action<ProfilerCameraListener, double>(this.CameraDurationCallback);
          this._cameraListeners.Add(profilerCameraListener);
        }
      }
    }
  }
}
