using System;
using UnityEngine;

namespace Cinemachine
{
  public class CinemachineBlend(
    ICinemachineCamera a,
    ICinemachineCamera b,
    AnimationCurve curve,
    float duration,
    float t) {
    public ICinemachineCamera CamA { get; set; } = a != null && b != null ? a : throw new ArgumentException("Blend cameras cannot be null");

    public ICinemachineCamera CamB { get; set; } = b;

    public AnimationCurve BlendCurve { get; set; } = curve;

    public float TimeInBlend { get; set; } = t;

    public float BlendWeight => BlendCurve != null ? BlendCurve.Evaluate(TimeInBlend) : 0.0f;

    public bool IsValid => CamA != null || CamB != null;

    public float Duration { get; set; } = duration;

    public bool IsComplete => TimeInBlend >= (double) Duration;

    public string Description
    {
      get
      {
        string str = CamA != null ? "[" + CamA.Name + "]" : "(none)";
        return string.Format("{0} {1}% from {2}", CamB != null ? "[" + CamB.Name + "]" : (object) "(none)", (int) (BlendWeight * 100.0), str);
      }
    }

    public bool Uses(ICinemachineCamera cam)
    {
      return cam == CamA || cam == CamB || CamA is BlendSourceVirtualCamera camA && camA.Blend.Uses(cam) || CamB is BlendSourceVirtualCamera camB && camB.Blend.Uses(cam);
    }

    public void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      CinemachineCore.Instance.UpdateVirtualCamera(CamA, worldUp, deltaTime);
      CinemachineCore.Instance.UpdateVirtualCamera(CamB, worldUp, deltaTime);
    }

    public CameraState State => CameraState.Lerp(CamA.State, CamB.State, BlendWeight);
  }
}
