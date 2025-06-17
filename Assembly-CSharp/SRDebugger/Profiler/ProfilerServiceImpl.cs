using System;
using System.Diagnostics;
using CircularBuffer;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;

namespace SRDebugger.Profiler
{
  [Service(typeof (IProfilerService))]
  public class ProfilerServiceImpl : SRServiceBase<IProfilerService>, IProfilerService
  {
    private const int FrameBufferSize = 400;
    private readonly SRList<ProfilerCameraListener> _cameraListeners = [];
    private readonly CircularBuffer<ProfilerFrame> _frameBuffer = new(400);
    private Camera[] _cameraCache = new Camera[6];
    private int _expectedCameraCount;
    private ProfilerLateUpdateListener _lateUpdateListener;
    private double _renderDuration;
    private int _reportedCameras;
    private Stopwatch _stopwatch = new();
    private double _updateDuration;
    private double _updateToRenderDuration;
    private double _customDuration;

    public float AverageFrameTime { get; private set; }

    public float LastFrameTime { get; private set; }

    public void SetCustom(double value) => _customDuration = value;

    public CircularBuffer<ProfilerFrame> FrameBuffer => _frameBuffer;

    protected void PushFrame(
      double totalTime,
      double updateTime,
      double renderTime,
      double customTime)
    {
      _frameBuffer.PushBack(new ProfilerFrame {
        OtherTime = totalTime - updateTime - renderTime - customTime,
        UpdateTime = updateTime,
        RenderTime = renderTime,
        CustomTime = customTime
      });
    }

    protected override void Awake()
    {
      base.Awake();
      _lateUpdateListener = gameObject.AddComponent<ProfilerLateUpdateListener>();
      _lateUpdateListener.OnLateUpdate = OnLateUpdate;
      CachedGameObject.hideFlags = HideFlags.NotEditable;
      CachedTransform.SetParent(Hierarchy.Get("SRDebugger"), true);
    }

    protected void Update()
    {
      if (FrameBuffer.Size > 0)
        FrameBuffer[FrameBuffer.Size - 1] = FrameBuffer.Back() with
        {
          FrameTime = Time.deltaTime
        };
      LastFrameTime = Time.deltaTime;
      int num1 = Mathf.Min(20, FrameBuffer.Size);
      double num2 = 0.0;
      for (int index = 0; index < num1; ++index)
        num2 += FrameBuffer[index].FrameTime;
      AverageFrameTime = (float) num2 / num1;
      if (_reportedCameras == _cameraListeners.Count)
        ;
      if (_stopwatch.IsRunning)
      {
        _stopwatch.Stop();
        _stopwatch.Reset();
      }
      _updateDuration = _renderDuration = _updateToRenderDuration = 0.0;
      _reportedCameras = 0;
      CameraCheck();
      _expectedCameraCount = 0;
      for (int index = 0; index < _cameraListeners.Count; ++index)
      {
        if (_cameraListeners[index].isActiveAndEnabled && _cameraListeners[index].Camera.isActiveAndEnabled)
          ++_expectedCameraCount;
      }
      _stopwatch.Start();
    }

    private void OnLateUpdate() => _updateDuration = _stopwatch.Elapsed.TotalSeconds;

    private void EndFrame()
    {
      if (!_stopwatch.IsRunning)
        return;
      double customTime = Math.Min(_customDuration, _updateDuration);
      _customDuration = 0.0;
      PushFrame(_stopwatch.Elapsed.TotalSeconds, _updateDuration - customTime, _renderDuration, customTime);
      _stopwatch.Reset();
      _stopwatch.Start();
    }

    private void CameraDurationCallback(ProfilerCameraListener listener, double duration)
    {
      ++_reportedCameras;
      _renderDuration = _stopwatch.Elapsed.TotalSeconds - _updateDuration - _updateToRenderDuration;
      if (_reportedCameras < _expectedCameraCount)
        return;
      EndFrame();
    }

    private void CameraCheck()
    {
      for (int index = _cameraListeners.Count - 1; index >= 0; --index)
      {
        if (_cameraListeners[index] == null)
          _cameraListeners.RemoveAt(index);
      }
      if (Camera.allCamerasCount == _cameraListeners.Count)
        return;
      if (Camera.allCamerasCount > _cameraCache.Length)
        _cameraCache = new Camera[Camera.allCamerasCount];
      int allCameras = Camera.GetAllCameras(_cameraCache);
      for (int index1 = 0; index1 < allCameras; ++index1)
      {
        Camera camera = _cameraCache[index1];
        bool flag = false;
        for (int index2 = 0; index2 < _cameraListeners.Count; ++index2)
        {
          if (_cameraListeners[index2].Camera == camera)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          ProfilerCameraListener profilerCameraListener = camera.gameObject.AddComponent<ProfilerCameraListener>();
          profilerCameraListener.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
          profilerCameraListener.RenderDurationCallback = CameraDurationCallback;
          _cameraListeners.Add(profilerCameraListener);
        }
      }
    }
  }
}
