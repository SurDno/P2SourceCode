using System;
using UnityEngine;

namespace Cinemachine
{
  public class CinemachineBlend
  {
    public ICinemachineCamera CamA { get; set; }

    public ICinemachineCamera CamB { get; set; }

    public AnimationCurve BlendCurve { get; set; }

    public float TimeInBlend { get; set; }

    public float BlendWeight
    {
      get => this.BlendCurve != null ? this.BlendCurve.Evaluate(this.TimeInBlend) : 0.0f;
    }

    public bool IsValid => this.CamA != null || this.CamB != null;

    public float Duration { get; set; }

    public bool IsComplete => (double) this.TimeInBlend >= (double) this.Duration;

    public string Description
    {
      get
      {
        string str = this.CamA != null ? "[" + this.CamA.Name + "]" : "(none)";
        return string.Format("{0} {1}% from {2}", this.CamB != null ? (object) ("[" + this.CamB.Name + "]") : (object) "(none)", (object) (int) ((double) this.BlendWeight * 100.0), (object) str);
      }
    }

    public bool Uses(ICinemachineCamera cam)
    {
      return cam == this.CamA || cam == this.CamB || this.CamA is BlendSourceVirtualCamera camA && camA.Blend.Uses(cam) || this.CamB is BlendSourceVirtualCamera camB && camB.Blend.Uses(cam);
    }

    public CinemachineBlend(
      ICinemachineCamera a,
      ICinemachineCamera b,
      AnimationCurve curve,
      float duration,
      float t)
    {
      this.CamA = a != null && b != null ? a : throw new ArgumentException("Blend cameras cannot be null");
      this.CamB = b;
      this.BlendCurve = curve;
      this.TimeInBlend = t;
      this.Duration = duration;
    }

    public void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      CinemachineCore.Instance.UpdateVirtualCamera(this.CamA, worldUp, deltaTime);
      CinemachineCore.Instance.UpdateVirtualCamera(this.CamB, worldUp, deltaTime);
    }

    public CameraState State
    {
      get => CameraState.Lerp(this.CamA.State, this.CamB.State, this.BlendWeight);
    }
  }
}
